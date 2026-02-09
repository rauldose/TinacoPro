using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public decimal Capacity { get; set; } // Liters
    public string Color { get; set; } = string.Empty; // Color of the tinaco
    public int Layers { get; set; } = 3; // Number of layers (typically 3)
    public decimal Weight { get; set; } // Weight in kg
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
}
