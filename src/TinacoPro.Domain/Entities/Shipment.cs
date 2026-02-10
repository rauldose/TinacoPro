namespace TinacoPro.Domain.Entities;

public enum ShipmentStatus
{
    Pending,
    InTransit,
    Delivered,
    Cancelled
}

public class Shipment
{
    public int Id { get; set; }
    public string ShipmentNumber { get; set; } = string.Empty;
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
    public DateTime? ActualDeliveryDate { get; set; }
    public ShipmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
    public FinishedGood? FinishedGood { get; set; }
}
