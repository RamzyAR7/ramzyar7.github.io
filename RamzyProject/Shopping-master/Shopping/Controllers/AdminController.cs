using A_Service.Models;
using A_Service.ViewModels;
using B_EF;
using B_EF.Business.Managers;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;
        private readonly IAdminBusiness adminBusiness;
        public AdminController(
           ApplicationDbContext db,
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
          IAdminBusiness _adminBusiness
          )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            adminBusiness = _adminBusiness;

            _db = db;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserDTO userDTO)
        {

            //var userRoles = Request.Form["roles"];
            if (string.IsNullOrEmpty(userDTO.idrole))
            {
                return View();
            }
           // RoleDTO[] roleDTO = new RoleDTO[1];
            RoleDTO role = adminBusiness.GetRoleByID(userDTO.idrole);
            if (role == null)
            {
                return View();
            }
            userDTO.RoleName = role.Name;
            //roleDTO.Append(role);

            //userDTO.Roles = roleDTO;
           // userDTO.Username = userDTO.PhoneNumber.ToString();
            if (ModelState.IsValid)
            {
                var IsExist = _db.Users.FirstOrDefault(a => a.PhoneNumber == userDTO.PhoneNumber);
                if (IsExist != null)
                {
                    TempData["error"] = "رقم الهاتف مدخل من قبل";
                    return RedirectToAction("AllUsers", "Admin");

                }
                var res = await adminBusiness.CreateUser(userDTO);
                if (res.Succeeded)
                {
                    TempData["success"] = "تم الحفظ بنجاح";
                    return RedirectToAction("AllUsers", "Admin");
                }
                else
                {
                    ViewBag.Roles = adminBusiness.GetAllRoles();
                    var message = "";
                    foreach (var error in res.Errors)
                    {
                        if (error.Code.Contains("DuplicateUserName"))
                        {
                            message = "تم اختيار الرقم من قبل";
                        }
                        else
                        {
                            message += "," + error.Description;
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        
                    }

                    //string messages = string.Join("; ", ModelState.Values
                    //                    .SelectMany(x => x.Errors)
                    //                    .Select(x => x.ErrorMessage));
                    TempData["error"] = message;

                    //return PartialView("_AddUser", userDTO);
                    return RedirectToAction("AllUsers", "Admin");
                }
            }

            TempData["error"] = "تم اختيار الرقم من قبل";
            ViewBag.Roles = adminBusiness.GetAllRoles();
            return RedirectToAction("AllUsers", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> AllUsers()
        {
            List<RoleDTO> roles = new List<RoleDTO>();
            var rolesDb = adminBusiness.GetAllRoles();
            if (rolesDb.Count() > 0)
            {
                foreach (var item in rolesDb)
                {
                    var roleName = "";
                    if (item.Name == "Admin")
                        roleName = "ادمن";
                    else
                        roleName = "مستخدم";

                    roles.Add(new RoleDTO() { Id = item.Id, Name = roleName });
                }
            }
            ViewBag.Roles = new SelectList(roles.ToList(), nameof(RoleDTO.Id), nameof(RoleDTO.Name));

            //TempData["error"] = "sucss";
            // ViewBag.Roles = adminBusiness.GetAllRoles();
            var users = await adminBusiness.GetAllUsersAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string PUserName, string NewPassword,string newName)
        {
            var usr = await _userManager.FindByNameAsync(PUserName);
            if (usr != null)
            {
                if (newName != null)
                {
                    usr.UserName = newName;
                }
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(usr);
                IdentityResult passwordChangeResult = await _userManager.ResetPasswordAsync(usr, resetToken, NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                        TempData["error"] = error.Code + " " + error.Description;

                    }
                    return View();
                }
                TempData["success"] = "تم تغير كلمه المرور";
            }
            return RedirectToAction("AllUsers");
        }
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var Currentuser = await _userManager.GetUserAsync(HttpContext.User);
            if (Currentuser ==null)
            {
                TempData["error"] = "خطا...لم يتم الحذف";
                return RedirectToAction("AllUsers", "Admin");
            }
            else
            {
                if (Currentuser.Id == id)
                {
                    TempData["error"] = "خطا...لم يمكن جذف المستخدم الحالى  ";
                    return RedirectToAction("AllUsers", "Admin");
                }
            }
            if (ModelState.IsValid)
            {
                //if (id == null)
                //{
                //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                //}

                var user = await _userManager.FindByIdAsync(id);
                // var logins = user.Logins;
                var rolesForUser = await _userManager.GetRolesAsync(user);

                using (var transaction = _db.Database.BeginTransaction())
                {
                    //foreach (var login in logins.ToList())
                    //{
                    //    await _userManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                    //}

                    if (rolesForUser.Count() > 0)
                    {
                        foreach (var item in rolesForUser.ToList())
                        {
                            // item should be the name of the role
                            _ = await _userManager.RemoveFromRoleAsync(user, item);
                        }
                    }

                    await _userManager.DeleteAsync(user);
                    transaction.Commit();
                }
                //TempData["success"] = "success";
                TempData["success"] = "تم الحذف   بنجاح";

                // return Json(new { message = "تم الحذف بنجاح" });
                return RedirectToAction("AllUsers", "Admin");
            }
            else
            {
                TempData["error"] = "خطا...لم يتم الحذف";
               // return Json(new { message = "خطا...لم يتم الحذف" });
                return RedirectToAction("AllUsers", "Admin");
            }
        }
    }
}
