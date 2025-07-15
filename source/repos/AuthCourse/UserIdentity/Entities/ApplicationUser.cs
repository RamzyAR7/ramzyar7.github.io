using Microsoft.AspNetCore.Identity;

namespace UserIdentity.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string? Address { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
