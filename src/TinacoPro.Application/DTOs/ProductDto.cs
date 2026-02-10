namespace TinacoPro.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public decimal Capacity { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? TemplateId { get; set; }
    public string? TemplateName { get; set; }
    public decimal MaterialCost { get; set; }
    public decimal LaborCost { get; set; }
    public decimal TotalCost => MaterialCost + LaborCost;
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public decimal Capacity { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? TemplateId { get; set; }
}
