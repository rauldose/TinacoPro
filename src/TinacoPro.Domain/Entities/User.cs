using TinacoPro.Domain.Common;

namespace TinacoPro.Domain.Entities;

public enum UserRole
{
    Admin,
    Manager,
    Operator
}

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Operator;
    public bool IsActive { get; set; } = true;
}
