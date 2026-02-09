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
    public int? TemplateId { get; set; } // Optional template assignment
    
    // Cost tracking
    public decimal MaterialCost { get; set; } = 0; // Total material cost
    public decimal LaborCost { get; set; } = 0; // Total labor cost
    public decimal OverheadCost { get; set; } = 0; // Overhead/indirect costs
    public decimal SellingPrice { get; set; } = 0; // Selling price
    public decimal ProfitMargin { get; set; } = 0; // Profit margin percentage
    
    // Navigation properties
    public ICollection<ProductMaterial> ProductMaterials { get; set; } = new List<ProductMaterial>();
    public ProductTemplate? Template { get; set; }
}
