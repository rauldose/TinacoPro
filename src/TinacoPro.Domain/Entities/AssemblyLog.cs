using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public class AssemblyLog : BaseEntity
{
    public int ProductionOrderId { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string Station { get; set; } = string.Empty; // e.g., ENS-01, ENS-02
    public DateTime AssemblyDate { get; set; }
    public int TimeMinutes { get; set; } // Assembly time in minutes
    public bool HasDefects { get; set; } = false;
    public string? DefectNotes { get; set; }
    public bool Passed { get; set; } = true;
    
    // Navigation properties
    public ProductionOrder ProductionOrder { get; set; } = null!;
}
