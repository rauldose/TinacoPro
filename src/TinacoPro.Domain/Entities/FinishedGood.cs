using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public class FinishedGood : BaseEntity
{
    public int ProductId { get; set; }
    public int ProductionOrderId { get; set; }
    public int? TemplateId { get; set; } // Capture template at production time
    public decimal Quantity { get; set; }
    public decimal CurrentStock { get; set; }
    public DateTime ProductionDate { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    
    // Capture actual costs at production time (may differ from current template/product costs)
    public decimal ActualMaterialCost { get; set; }
    public decimal ActualLaborCost { get; set; }
    public decimal TotalActualCost => ActualMaterialCost + ActualLaborCost;
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public ProductionOrder ProductionOrder { get; set; } = null!;
    public ProductTemplate? Template { get; set; }
}
