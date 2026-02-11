using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public class Site : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? ManagerName { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
}
