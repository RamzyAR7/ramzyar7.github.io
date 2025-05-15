using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Service.ViewModels
{
    public class ChangePasswordDTO
    {
        [Required]
        [Display(Name = "UserNameLabel")]
        public string UserName { set; get; }
        [Display(Name = "OldPasswordLabel")]
        [Required(ErrorMessage = "OldPasswordError")]
        public string OldPassword { set; get; }
        [Required(ErrorMessage = "NewPasswordError")]
        [Display(Name = "NewPasswordLabel")]
        public string NewPassword { set; get; }
        [Required(ErrorMessage = "ConfirmPasswordError")]
        [Compare("NewPassword", ErrorMessage = "CompareConfirmPasswordError")]
        [Display(Name = "ConfirmPasswordLabel")]
        public string ConfirmPassword { set; get; }
    }

}
