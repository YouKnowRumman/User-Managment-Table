using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public DateTime RegistrationTime { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginTime { get; set; }
    public bool IsBlocked { get; set; } = false;
}
