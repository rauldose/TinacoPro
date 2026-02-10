using TinacoPro.Application.DTOs;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;

namespace TinacoPro.Application.Services;

public class ProductionOrderService
{
    private readonly IProductionOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IRawMaterialRepository _materialRepository;
    private readonly IProductTemplateRepository _templateRepository;

    public ProductionOrderService(
        IProductionOrderRepository orderRepository,
        IProductRepository productRepository,
        IRawMaterialRepository materialRepository,
        IProductTemplateRepository templateRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _materialRepository = materialRepository;
        _templateRepository = templateRepository;
    }

    public async Task<IEnumerable<ProductionOrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        var products = await _productRepository.GetAllAsync();
        var productDict = products.ToDictionary(p => p.Id, p => p.Name);

        return orders.Select(o => new ProductionOrderDto
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            ProductId = o.ProductId,
            ProductName = productDict.GetValueOrDefault(o.ProductId, "Unknown"),
            Quantity = o.Quantity,
            Status = o.Status.ToString(),
            OrderDate = o.OrderDate,
            CompletedDate = o.CompletedDate,
            Notes = o.Notes
        });
    }

    public async Task<ProductionOrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) return null;

        var product = await _productRepository.GetByIdAsync(order.ProductId);

        return new ProductionOrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            ProductId = order.ProductId,
            ProductName = product?.Name ?? "Unknown",
            Quantity = order.Quantity,
            Status = order.Status.ToString(),
            OrderDate = order.OrderDate,
            CompletedDate = order.CompletedDate,
            Notes = order.Notes
        };
    }

    private static readonly Random _random = new Random();

    public async Task<ProductionOrderDto> CreateOrderAsync(CreateProductionOrderDto dto)
    {
        var orderNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{_random.Next(1000, 9999)}";
        
        var order = new ProductionOrder
        {
            OrderNumber = orderNumber,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            Status = OrderStatus.Pending,
            OrderDate = DateTime.UtcNow,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _orderRepository.AddAsync(order);
        var product = await _productRepository.GetByIdAsync(created.ProductId);

        return new ProductionOrderDto
        {
            Id = created.Id,
            OrderNumber = created.OrderNumber,
            ProductId = created.ProductId,
            ProductName = product?.Name ?? "Unknown",
            Quantity = created.Quantity,
            Status = created.Status.ToString(),
            OrderDate = created.OrderDate,
            CompletedDate = created.CompletedDate,
            Notes = created.Notes
        };
    }

    public async Task StartOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order != null && order.Status == OrderStatus.Pending)
        {
            order.Status = OrderStatus.InProgress;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);
        }
    }

    public async Task CompleteOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || order.Status != OrderStatus.InProgress)
        {
            return;
        }

        var product = await _productRepository.GetByIdAsync(order.ProductId);
        if (product == null)
        {
            throw new InvalidOperationException($"Product with ID {order.ProductId} not found");
        }

        // If product has a template, use template-based depletion
        if (product.TemplateId.HasValue)
        {
            await DepleteMaterialsFromTemplateAsync(order, product.TemplateId.Value);
        }
        else
        {
            // Fall back to ProductMaterials-based depletion (legacy method)
            await DepleteMaterialsFromProductBOMAsync(order, product);
        }

        order.Status = OrderStatus.Completed;
        order.CompletedDate = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order);
    }

    private async Task DepleteMaterialsFromTemplateAsync(ProductionOrder order, int templateId)
    {
        // Load template with all parts
        var template = await _templateRepository.GetByIdWithPartsAsync(templateId);
        if (template == null)
        {
            throw new InvalidOperationException($"Template with ID {templateId} not found");
        }

        // Calculate material requirements from template hierarchy
        var materialRequirements = new Dictionary<int, decimal>();
        await CollectMaterialRequirementsAsync(template.Parts.Where(p => p.ParentPartId == null), materialRequirements);

        // Validate sufficient stock before depletion
        var insufficientMaterials = new List<string>();
        foreach (var requirement in materialRequirements)
        {
            var material = await _materialRepository.GetByIdAsync(requirement.Key);
            if (material != null)
            {
                var totalRequired = requirement.Value * order.Quantity;
                if (material.CurrentStock < totalRequired)
                {
                    insufficientMaterials.Add($"{material.Name} (Required: {totalRequired:F2} {material.Unit}, Available: {material.CurrentStock:F2} {material.Unit})");
                }
            }
        }

        if (insufficientMaterials.Any())
        {
            throw new InvalidOperationException($"Insufficient stock for materials:\n{string.Join("\n", insufficientMaterials)}");
        }

        // Deplete materials and log consumption
        foreach (var requirement in materialRequirements)
        {
            var material = await _materialRepository.GetByIdAsync(requirement.Key);
            if (material != null)
            {
                var quantityToDeplete = requirement.Value * order.Quantity;
                material.CurrentStock -= quantityToDeplete;
                await _materialRepository.UpdateAsync(material);

                // Note: MaterialConsumptionLog creation would go here if we had a repository for it
                // For now, we're focusing on the core depletion logic
            }
        }
    }

    private async Task CollectMaterialRequirementsAsync(IEnumerable<TemplatePart> parts, Dictionary<int, decimal> requirements)
    {
        foreach (var part in parts)
        {
            // If this part has a linked raw material, add its quantity
            if (part.RawMaterialId.HasValue)
            {
                if (requirements.ContainsKey(part.RawMaterialId.Value))
                {
                    requirements[part.RawMaterialId.Value] += part.Quantity;
                }
                else
                {
                    requirements[part.RawMaterialId.Value] = part.Quantity;
                }
            }

            // Recursively process children
            if (part.Children != null && part.Children.Any())
            {
                await CollectMaterialRequirementsAsync(part.Children, requirements);
            }
        }
    }

    private async Task DepleteMaterialsFromProductBOMAsync(ProductionOrder order, Product product)
    {
        // Legacy method: Consume raw materials from ProductMaterials
        foreach (var pm in product.ProductMaterials)
        {
            var material = await _materialRepository.GetByIdAsync(pm.RawMaterialId);
            if (material != null)
            {
                var requiredQuantity = pm.QuantityRequired * order.Quantity;
                material.CurrentStock -= requiredQuantity;
                await _materialRepository.UpdateAsync(material);
            }
        }
    }

    public async Task CancelOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order != null)
        {
            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);
        }
    }
}
