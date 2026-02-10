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
    private readonly IFinishedGoodRepository _finishedGoodRepository;
    private readonly IMaterialConsumptionLogRepository _consumptionLogRepository;

    public ProductionOrderService(
        IProductionOrderRepository orderRepository,
        IProductRepository productRepository,
        IRawMaterialRepository materialRepository,
        IProductTemplateRepository templateRepository,
        IFinishedGoodRepository finishedGoodRepository,
        IMaterialConsumptionLogRepository consumptionLogRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _materialRepository = materialRepository;
        _templateRepository = templateRepository;
        _finishedGoodRepository = finishedGoodRepository;
        _consumptionLogRepository = consumptionLogRepository;
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
            Shift = o.Shift.ToString(),
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
            Shift = order.Shift.ToString(),
            OrderDate = order.OrderDate,
            CompletedDate = order.CompletedDate,
            Notes = order.Notes
        };
    }

    private static readonly Random _random = new Random();

    public async Task<ProductionOrderDto> CreateOrderAsync(CreateProductionOrderDto dto)
    {
        var orderNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{_random.Next(1000, 9999)}";
        
        // Parse shift from string
        Shift shift = Enum.TryParse<Shift>(dto.Shift, true, out var parsedShift) 
            ? parsedShift 
            : Shift.Morning;
        
        var order = new ProductionOrder
        {
            OrderNumber = orderNumber,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            Status = OrderStatus.Pending,
            Shift = shift,
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
            Shift = created.Shift.ToString(),
            OrderDate = created.OrderDate,
            CompletedDate = created.CompletedDate,
            Notes = created.Notes
        };
    }

    /// <summary>
    /// Creates a production order from daily production entry with automation:
    /// - Automatically sets the order to InProgress
    /// - Auto-starts any existing Pending orders for the same product
    /// - Checks existing InProgress orders for the same product to auto-complete if daily production meets target
    /// Returns the created order, whether any orders were auto-completed, list of auto-started order IDs, and low-stock material warnings
    /// </summary>
    public async Task<(ProductionOrderDto Order, bool AutoCompleted, List<string> AutoStartedOrders, List<string> LowStockWarnings)> CreateDailyProductionOrderAsync(CreateProductionOrderDto dto)
    {
        // First create the order normally
        var orderDto = await CreateOrderAsync(dto);
        
        // Auto-start the order (Pending -> InProgress)
        await StartOrderAsync(orderDto.Id);
        orderDto.Status = "InProgress";

        // Auto-start any existing Pending orders for the same product
        var autoStartedOrders = new List<string>();
        var allOrders = await _orderRepository.GetAllAsync();
        var pendingOrders = allOrders
            .Where(o => o.ProductId == dto.ProductId && o.Status == OrderStatus.Pending && o.Id != orderDto.Id)
            .OrderBy(o => o.OrderDate)
            .ToList();

        foreach (var pendingOrder in pendingOrders)
        {
            await StartOrderAsync(pendingOrder.Id);
            autoStartedOrders.Add(pendingOrder.OrderNumber);
        }

        // Check existing InProgress orders for this product to see if any should be auto-completed
        // An existing order is auto-completed when the daily production quantity (dto.Quantity) meets
        // or exceeds that order's target quantity
        bool autoCompleted = false;
        // Re-fetch orders since we may have changed some statuses
        allOrders = await _orderRepository.GetAllAsync();
        var inProgressOrders = allOrders
            .Where(o => o.ProductId == dto.ProductId && o.Status == OrderStatus.InProgress && o.Id != orderDto.Id)
            .OrderBy(o => o.OrderDate)
            .ToList();

        foreach (var existingOrder in inProgressOrders)
        {
            // If the daily production quantity meets or exceeds this order's target, auto-complete it
            if (dto.Quantity >= existingOrder.Quantity)
            {
                try
                {
                    await CompleteOrderAsync(existingOrder.Id);
                    autoCompleted = true;
                }
                catch
                {
                    // If completion fails (e.g., insufficient materials), keep it InProgress
                }
            }
        }
        
        // Check for low-stock materials after depletion
        var lowStockWarnings = await GetLowStockWarningsAsync();

        // Refresh the order DTO to get updated status
        var refreshedOrder = await GetOrderByIdAsync(orderDto.Id);
        return (refreshedOrder ?? orderDto, autoCompleted, autoStartedOrders, lowStockWarnings);
    }

    /// <summary>
    /// Returns warnings for materials that are at or below minimum stock levels.
    /// </summary>
    private async Task<List<string>> GetLowStockWarningsAsync()
    {
        var warnings = new List<string>();
        var materials = await _materialRepository.GetAllAsync();
        foreach (var material in materials.Where(m => m.IsActive && m.CurrentStock <= m.MinimumStock))
        {
            warnings.Add($"{material.Name}: {material.CurrentStock:F1} {material.Unit} ({material.MinimumStock:F1} {material.Unit} min)");
        }
        return warnings;
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

        // Create finished goods entry
        await CreateFinishedGoodsAsync(order);

        order.Status = OrderStatus.Completed;
        order.CompletedDate = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order);
    }

    private async Task CreateFinishedGoodsAsync(ProductionOrder order)
    {
        // Get product to capture template and cost information
        var product = await _productRepository.GetByIdAsync(order.ProductId);
        if (product == null)
        {
            throw new InvalidOperationException($"Product with ID {order.ProductId} not found");
        }

        // Generate batch number
        var batchNumber = $"BATCH-{DateTime.UtcNow:yyyyMMdd}-{order.OrderNumber}";

        var finishedGood = new FinishedGood
        {
            ProductId = order.ProductId,
            ProductionOrderId = order.Id,
            TemplateId = product.TemplateId, // Capture template at production time
            Quantity = order.Quantity,
            CurrentStock = order.Quantity,
            ProductionDate = DateTime.UtcNow,
            BatchNumber = batchNumber,
            Notes = $"Produced from order {order.OrderNumber}",
            ActualMaterialCost = product.MaterialCost, // Capture actual costs at production time
            ActualLaborCost = product.LaborCost,
            CreatedAt = DateTime.UtcNow
        };

        await _finishedGoodRepository.AddAsync(finishedGood);
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
        CollectMaterialRequirements(template.Parts.Where(p => p.ParentPartId == null), materialRequirements);

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

                // Log material consumption
                var consumptionLog = new MaterialConsumptionLog
                {
                    ProductionOrderId = order.Id,
                    RawMaterialId = material.Id,
                    QuantityConsumed = quantityToDeplete,
                    UnitCostAtConsumption = material.UnitCost,
                    TotalCost = quantityToDeplete * material.UnitCost,
                    ConsumedAt = DateTime.UtcNow,
                    Notes = $"Auto-consumed from template for order {order.OrderNumber}",
                    CreatedAt = DateTime.UtcNow
                };
                await _consumptionLogRepository.AddAsync(consumptionLog);
            }
        }
    }

    private void CollectMaterialRequirements(IEnumerable<TemplatePart> parts, Dictionary<int, decimal> requirements)
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
                CollectMaterialRequirements(part.Children, requirements);
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

                // Log material consumption
                var consumptionLog = new MaterialConsumptionLog
                {
                    ProductionOrderId = order.Id,
                    RawMaterialId = material.Id,
                    QuantityConsumed = requiredQuantity,
                    UnitCostAtConsumption = material.UnitCost,
                    TotalCost = requiredQuantity * material.UnitCost,
                    ConsumedAt = DateTime.UtcNow,
                    Notes = $"Auto-consumed from BOM for order {order.OrderNumber}",
                    CreatedAt = DateTime.UtcNow
                };
                await _consumptionLogRepository.AddAsync(consumptionLog);
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
