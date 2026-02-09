namespace TinacoPro.Application.DTOs;

public class DashboardDto
{
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int TotalRawMaterials { get; set; }
    public int LowStockMaterials { get; set; }
    public int PendingOrders { get; set; }
    public int InProgressOrders { get; set; }
    public int CompletedOrdersToday { get; set; }
    public int CompletedOrdersThisWeek { get; set; }
    public int CompletedOrdersThisMonth { get; set; }
    public decimal TotalFinishedGoodsStock { get; set; }
}
