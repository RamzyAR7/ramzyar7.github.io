using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserIdentity.Models
{
    public class LoginVM
    {
        [DisplayName("User Name")]
        [Required(ErrorMessage ="*")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage ="*")]
        public string Password { get; set; }

        [DisplayName("Remember Me?")]
        public bool RememberMe { get; set; }
    }
}
