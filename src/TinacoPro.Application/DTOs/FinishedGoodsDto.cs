namespace TinacoPro.Application.DTOs;

public class FinishedGoodsDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int ProductionOrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal CurrentStock { get; set; }
    public DateTime ProductionDate { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class CreateFinishedGoodsDto
{
    public int ProductId { get; set; }
    public int ProductionOrderId { get; set; }
    public int Quantity { get; set; }
    public decimal CurrentStock { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
