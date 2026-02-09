using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public class ProductTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ModelType { get; set; } = string.Empty; // e.g., "Standard", "Premium", "Industrial"
    public bool IsActive { get; set; } = true;
    
    // Cost summary (calculated from parts)
    public decimal TotalMaterialCost { get; set; } = 0;
    public decimal TotalLaborCost { get; set; } = 0;
    public int TotalEstimatedMinutes { get; set; } = 0;
    
    // Navigation properties
    public ICollection<TemplatePart> Parts { get; set; } = new List<TemplatePart>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
