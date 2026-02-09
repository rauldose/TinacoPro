using TinacoPro.Application.DTOs;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;

namespace TinacoPro.Application.Services;

public class ProductionOrderService
{
    private readonly IProductionOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IRawMaterialRepository _materialRepository;

    public ProductionOrderService(
        IProductionOrderRepository orderRepository,
        IProductRepository productRepository,
        IRawMaterialRepository materialRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _materialRepository = materialRepository;
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
        if (order != null && order.Status == OrderStatus.InProgress)
        {
            // Consume raw materials automatically
            var product = await _productRepository.GetByIdAsync(order.ProductId);
            if (product != null)
            {
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

            order.Status = OrderStatus.Completed;
            order.CompletedDate = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);
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
