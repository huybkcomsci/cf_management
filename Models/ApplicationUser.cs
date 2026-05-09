using Microsoft.AspNetCore.Identity;

namespace CafeManagement.Models;

/// <summary>
/// Application user extends IdentityUser with Guid primary key
/// Stores additional user profile information
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>Display name for UI</summary>
    public string? DisplayName { get; set; }

    /// <summary>User is active flag</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Creation timestamp</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Last update timestamp</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
