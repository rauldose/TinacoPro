using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public enum MaterialCategory
{
    Resina,
    Aditivo,
    Accesorio,
    Empaque
}

public class RawMaterial : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty; // kg, liters, pz (pieces), rollo (roll), etc.
    public MaterialCategory Category { get; set; } = MaterialCategory.Resina;
    public decimal CurrentStock { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal UnitCost { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
}
