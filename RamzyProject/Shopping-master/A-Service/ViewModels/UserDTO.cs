using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_Service.ViewModels
{
	public class UserDTO
	{
		public string Id { set; get; }
        [Display(Name = "اسم المستخدم")]
        [Required(ErrorMessage = "يجب ادخال اسم المستخدم")]

        public string Username { get; set; }



		[Display(Name = "رقم الهاتف")]
		[Required(ErrorMessage = "يجب ادخال الهاتف  ")]
		[Phone]
		public string PhoneNumber { get; set; }



		[Required(ErrorMessage = "يجب ادخال كلمه المرور"), DataType(DataType.Password)]
		[Display(Name = "كلمه المرور")]
		public string Password { get; set; }
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "كلمه المرور غير متطابقه !")]
		[Required(ErrorMessage = "يجب ادخال كلمه المرور  ")]

		public string ConfirmPassword { get; set; }
		//public bool? IsAdmin { get; set; }

		[Required(ErrorMessage = "يجب اختيار مجموعه  ")]
		public string idrole { get; set; }

		public string RoleName { get; set; }
		public string Role { get; set; }

		public RoleDTO[] Roles { get; set; }
	}

}
