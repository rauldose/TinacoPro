namespace TinacoPro.Application.DTOs;

public class FinishedGoodsDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int ProductionOrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int? TemplateId { get; set; }
    public string? TemplateName { get; set; }
    public decimal Quantity { get; set; }
    public decimal CurrentStock { get; set; }
    public DateTime ProductionDate { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public decimal ActualMaterialCost { get; set; }
    public decimal ActualLaborCost { get; set; }
    public decimal TotalActualCost => ActualMaterialCost + ActualLaborCost;
}

public class CreateFinishedGoodsDto
{
    public int ProductId { get; set; }
    public int ProductionOrderId { get; set; }
    public decimal Quantity { get; set; }
    public decimal CurrentStock { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
