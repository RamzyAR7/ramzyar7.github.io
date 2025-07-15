using Microsoft.AspNetCore.Identity;

namespace PermissionBasedAuth.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? Address { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
