using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public class ProductMaterial : BaseEntity
{
    public int ProductId { get; set; }
    public int RawMaterialId { get; set; }
    public decimal QuantityRequired { get; set; }
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public RawMaterial RawMaterial { get; set; } = null!;
}
