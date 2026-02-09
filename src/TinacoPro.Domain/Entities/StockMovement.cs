using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public enum MovementType
{
    In,
    Out,
    Adjustment
}

public class StockMovement : BaseEntity
{
    public int RawMaterialId { get; set; }
    public MovementType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal PreviousStock { get; set; }
    public decimal NewStock { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty; // PO number, invoice, etc.
    public int? SupplierId { get; set; }
    public int? ProductionOrderId { get; set; }
    
    // Navigation properties
    public RawMaterial RawMaterial { get; set; } = null!;
    public Supplier? Supplier { get; set; }
    public ProductionOrder? ProductionOrder { get; set; }
}
