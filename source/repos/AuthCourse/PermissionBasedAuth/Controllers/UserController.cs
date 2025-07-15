using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermissionBasedAuth.Entities;
using PermissionBasedAuth.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using PermissionBasedAuth.Filter;
using PermissionBasedAuth.Contants;

namespace PermissionBasedAuth.Controllers
{
    [Authorize]
    public class UserController(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager, IWebHostEnvironment _webHostEnvironment) : Controller
    {
        [PermissionFilterFactory(Permission.User.Read)]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UsersVM>();
            
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UsersVM
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return View(userViewModels);
        }

        [HttpGet]
        [PermissionFilterFactory(Permission.User.Create)]
        public async Task<IActionResult> Create()
        {
            return View(new RegisterVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionFilterFactory(Permission.User.Create)]
        public async Task<IActionResult> Create(RegisterVM registerVM)
        {
            if(ModelState.IsValid)
            {
                var IsUserNameExisted = await _userManager.FindByNameAsync(registerVM.UserName);
                if (IsUserNameExisted != null)
                {
                    ModelState.AddModelError("", "User Name Already Exists");
                    return View(registerVM);
                }
                var IsUserEmailExisted = await _userManager.FindByEmailAsync(registerVM.Email);
                if (IsUserEmailExisted != null)
                {
                    ModelState.AddModelError("", "User Email Already Exists");
                    return View(registerVM);
                }

                var fileName = String.Empty;
                string[] allowedExtentions = { ".jpg", ".jpeg", ".png" };
                if (registerVM.PhotoUrl != null && allowedExtentions.Contains(Path.GetExtension(registerVM.PhotoUrl.FileName))) 
                {
                    var folder = Path.Combine(_webHostEnvironment.WebRootPath,"Images");
                    fileName = $"{Guid.NewGuid().ToString()}_{registerVM.PhotoUrl.FileName}";

                    var fullPath = Path.Combine(folder, fileName);

                    using(var file  = new FileStream(fullPath, FileMode.Create))
                    {
                        await registerVM.PhotoUrl.CopyToAsync(file);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Unsupported file type.");
                    return View(registerVM);
                }


                var user = new ApplicationUser
                {
                    UserName = registerVM.UserName,
                    Email = registerVM.Email,
                    Address = registerVM.Address ?? "NotFound",
                    PhotoUrl = fileName?? "NotFound"
                };

                var state = await _userManager.CreateAsync(user, registerVM.Password);

                if (state.Succeeded)
                {
                    TempData["Created"] = "User Created Successfully.";
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var error in state.Errors)
                    {
                        ModelState.AddModelError("", error.Description);

                    }
                    return View(registerVM);
                }
            }
            return View(registerVM);
        }
        [PermissionFilterFactory(Permission.User.Update)]
        public async Task<IActionResult> ManageRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();

            var userRoles = new UserRolesVM { 
                UserId= user.Id,
                UserName = user.UserName,
                Roles = roles.Select(r => new CheckBoxVM
                {
                    Name = r.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, r.Name).Result
                }).ToList()
            };
            return View(userRoles);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionFilterFactory(Permission.User.Update)]
        public async Task<IActionResult> ManageRole(string userId, UserRolesVM userRoles)
        {
            if (userId != userRoles.UserId)
            {
                return BadRequest("User ID mismatch.");
            }
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }
                var roles = await _userManager.GetRolesAsync(user);

                if (roles != null && roles.Count > 0)
                {
                    await _userManager.RemoveFromRolesAsync(user, roles);
                }

                await _userManager.AddToRolesAsync(user, userRoles.Roles.Where(r => r.IsSelected == true).Select(r => r.Name));
                return RedirectToAction("Index");
            }
            return View(userRoles);
        }

    }
}
