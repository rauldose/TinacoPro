using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public enum MachineType
{
    Rotomoldeo,
    Enfriamiento,
    Rebabeo,
    Ensamble,
    Prueba
}

public enum MachineStatus
{
    Running,
    Idle,
    Maintenance
}

public class Machine : BaseEntity
{
    public string Code { get; set; } = string.Empty; // e.g., ROT-01
    public string Name { get; set; } = string.Empty;
    public MachineType Type { get; set; } = MachineType.Rotomoldeo;
    public MachineStatus Status { get; set; } = MachineStatus.Idle;
    public int? CurrentProductionOrderId { get; set; }
    public string? CurrentModel { get; set; }
    public int CycleTime { get; set; } // minutes
    public decimal Temperature { get; set; } // Celsius
    public decimal RPM { get; set; } // Revolutions per minute
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ProductionOrder? CurrentProductionOrder { get; set; }
}
