using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime LastLoginAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string GetFullName()
    {
        return LastName is null ? FirstName : $"{FirstName} {LastName}";
    }
}
