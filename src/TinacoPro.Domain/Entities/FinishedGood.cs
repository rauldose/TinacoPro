using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public class FinishedGood : BaseEntity
{
    public int ProductId { get; set; }
    public int ProductionOrderId { get; set; }
    public decimal Quantity { get; set; }
    public decimal CurrentStock { get; set; }
    public DateTime ProductionDate { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public ProductionOrder ProductionOrder { get; set; } = null!;
}
