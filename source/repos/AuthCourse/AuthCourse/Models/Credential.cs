using System.ComponentModel.DataAnnotations;

namespace AuthCourse.Models
{
    public class Credential
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }

        public bool RememberMe { get; set; } = false;
    }
}
