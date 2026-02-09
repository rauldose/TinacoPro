using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public enum OrderStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}

public class ProductionOrder : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime OrderDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public ICollection<FinishedGood> FinishedGoods { get; set; } = new List<FinishedGood>();
}
