namespace TinacoPro.Application.DTOs;

public class ProductionOrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class CreateProductionOrderDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Notes { get; set; } = string.Empty;
}
