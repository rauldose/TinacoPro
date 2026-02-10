using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public class MaterialConsumptionLog : BaseEntity
{
    public int ProductionOrderId { get; set; }
    public int RawMaterialId { get; set; }
    public decimal QuantityConsumed { get; set; }
    public decimal UnitCostAtConsumption { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime ConsumedAt { get; set; }
    public string Notes { get; set; } = string.Empty;
    
    // Navigation properties
    public ProductionOrder ProductionOrder { get; set; } = null!;
    public RawMaterial RawMaterial { get; set; } = null!;
}
