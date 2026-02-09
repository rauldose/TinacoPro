using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public enum OrderStatus
{
    Pending,
    InProgress,
    Molding,
    Cooling,
    Trimming,
    Assembly,
    Testing,
    Completed,
    Cancelled
}

public enum OrderPriority
{
    Low,
    Medium,
    High
}

public class ProductionOrder : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int CompletedQuantity { get; set; } = 0;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public OrderPriority Priority { get; set; } = OrderPriority.Medium;
    public DateTime OrderDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int? AssignedMachineId { get; set; }
    public string Notes { get; set; } = string.Empty;
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public Machine? AssignedMachine { get; set; }
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public ICollection<FinishedGood> FinishedGoods { get; set; } = new List<FinishedGood>();
    public ICollection<AssemblyLog> AssemblyLogs { get; set; } = new List<AssemblyLog>();
}
