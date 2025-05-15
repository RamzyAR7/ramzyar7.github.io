using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Service.ViewModels
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "يجب ادخال رقم الهاتف")]
        [Display(Name = "UserNameLabel")]
        public string Username { get; set; }
        [Required(ErrorMessage = "يجب ادخال كله المرور"), DataType(DataType.Password)]
        [Display(Name = "PasswordLabel")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
