using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace table.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; } = default!;
        public UserStatus Status { get; set; } = UserStatus.Unverified;

        // important: time when the user registered (UTC)
        public DateTime RegistrationTime { get; set; } = DateTime.UtcNow;

        // important: last time the user was seen active (optional)
        public DateTime? LastActivityTime { get; set; }

        public DateTime? LastLoginTime { get; set; }
    }
}
