using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public class RawMaterial : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty; // kg, liters, units, etc.
    public decimal CurrentStock { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal UnitCost { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
}
