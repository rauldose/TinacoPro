namespace TinacoPro.Application.DTOs;

public class ShipmentDto
{
    public int Id { get; set; }
    public string ShipmentNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int? FinishedGoodId { get; set; }
    public decimal Quantity { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerContact { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public string DestinationZone { get; set; } = string.Empty;
    public DateTime ShipmentDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateShipmentDto
{
    public int ProductId { get; set; }
    public int? FinishedGoodId { get; set; }
    public decimal Quantity { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerContact { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public string DestinationZone { get; set; } = string.Empty;
    public DateTime ShipmentDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Notes { get; set; }
}
