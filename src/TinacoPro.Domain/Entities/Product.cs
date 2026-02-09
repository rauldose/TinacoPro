using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public decimal Capacity { get; set; } // Liters
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
}
