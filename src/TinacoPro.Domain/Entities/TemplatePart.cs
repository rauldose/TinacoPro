using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public class TemplatePart : BaseEntity
{
    public int TemplateId { get; set; }
    public int? ParentPartId { get; set; } // Null for root parts
    
    public string Name { get; set; } = string.Empty;
    public string PartType { get; set; } = "Component"; // Assembly, Component, Material, Process
    public decimal Quantity { get; set; } = 1;
    public string Unit { get; set; } = "unit";
    public int? RawMaterialId { get; set; } // Optional link to raw material
    public int Position { get; set; } = 0; // Order within siblings
    public decimal UnitCost { get; set; } = 0; // Cost per unit
    public decimal LaborCost { get; set; } = 0; // Labor cost for this part/assembly
    public int EstimatedMinutes { get; set; } = 0; // Time required for assembly/process
    public string Notes { get; set; } = string.Empty;
    
    // Navigation properties
    public ProductTemplate Template { get; set; } = null!;
    public TemplatePart? ParentPart { get; set; }
    public ICollection<TemplatePart> Children { get; set; } = new List<TemplatePart>();
    public RawMaterial? RawMaterial { get; set; }
}
