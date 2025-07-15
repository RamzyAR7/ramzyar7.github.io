using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UserIdentity.Entities;

namespace UserIdentity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> AddRole([FromRoute] string UserId)
        {
            var existingUser = await _userManager.FindByIdAsync(UserId);
            if (existingUser == null)
            {
                return NotFound();
            }

            RolesSelectList();
            return View(existingUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole([FromRoute] string userId,[FromForm] string RoleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (ModelState.IsValid)
            {
                if (user == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return RedirectToAction("Index");
                }

                var existingRole = await _roleManager.RoleExistsAsync(RoleName);
                if (!existingRole)
                {
                    ModelState.AddModelError("", "Role does not exist.");
                    RolesSelectList();
                    return View(user);
                }

                var state = await _userManager.AddToRoleAsync(user, RoleName);
                if (state.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    foreach (var error in state.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                        RolesSelectList();
                    }
                    return View(user);
                }
            }
            RolesSelectList();
            return View(user);
        }
        public void RolesSelectList()
        {
            ViewBag.Roles = _roleManager.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name,
            }).ToList();


            // or

            /*
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = new SelectList(roles.Select(r => new {
                Value = r.Name,
                Text = r.Name
            }), "Value", "Text");
            */
        }
    }
}
