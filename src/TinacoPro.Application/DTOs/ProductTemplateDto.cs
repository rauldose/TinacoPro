namespace TinacoPro.Application.DTOs;

public class ProductTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ModelType { get; set; }
    public bool IsActive { get; set; }
    public decimal TotalMaterialCost { get; set; }
    public decimal TotalLaborCost { get; set; }
    public int TotalEstimatedMinutes { get; set; }
    public List<TemplatePartDto> RootParts { get; set; } = new();
    public int ProductCount { get; set; }
}

public class TemplatePartDto
{
    public int Id { get; set; }
    public int ProductTemplateId { get; set; }
    public int? ParentPartId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PartType { get; set; } = "Component"; // Assembly, Component, Material, Process
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "unit";
    public decimal UnitCost { get; set; }
    public decimal LaborCost { get; set; }
    public int EstimatedMinutes { get; set; }
    public int Position { get; set; }
    public int? RawMaterialId { get; set; }
    public string? RawMaterialName { get; set; }
    public List<TemplatePartDto> Children { get; set; } = new();
    public decimal TotalCost => (UnitCost * Quantity) + LaborCost;
}
