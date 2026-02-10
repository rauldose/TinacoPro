using TinacoPro.Application.DTOs;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;

namespace TinacoPro.Application.Services;

public class DashboardService
{
    private readonly IProductRepository _productRepository;
    private readonly IRawMaterialRepository _materialRepository;
    private readonly IProductionOrderRepository _orderRepository;

    public DashboardService(
        IProductRepository productRepository,
        IRawMaterialRepository materialRepository,
        IProductionOrderRepository orderRepository)
    {
        _productRepository = productRepository;
        _materialRepository = materialRepository;
        _orderRepository = orderRepository;
    }

    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var materials = await _materialRepository.GetAllAsync();
        var orders = await _orderRepository.GetAllAsync();

        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        return new DashboardDto
        {
            TotalProducts = products.Count(),
            ActiveProducts = products.Count(p => p.IsActive),
            TotalRawMaterials = materials.Count(),
            LowStockMaterials = materials.Count(m => m.CurrentStock <= m.MinimumStock),
            PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending),
            InProgressOrders = orders.Count(o => o.Status == OrderStatus.InProgress),
            CompletedOrdersToday = orders.Count(o => o.Status == OrderStatus.Completed && o.CompletedDate?.Date == today),
            CompletedOrdersThisWeek = orders.Count(o => o.Status == OrderStatus.Completed && o.CompletedDate >= weekStart),
            CompletedOrdersThisMonth = orders.Count(o => o.Status == OrderStatus.Completed && o.CompletedDate >= monthStart),
            TotalFinishedGoodsStock = 0 // Will be calculated from finished goods
        };
    }
}
