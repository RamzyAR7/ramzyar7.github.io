using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom;
using UserIdentity.Entities;
using UserIdentity.Models;

namespace UserIdentity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        public RoleController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(RoleVM roleVM)
        {
            if(ModelState.IsValid)
            {
                var existingRole = await _roleManager.FindByNameAsync(roleVM.RoleName);
                if(existingRole != null)
                {
                    ModelState.AddModelError("", "Role already exists.");
                    return View(roleVM);
                }
                var role = new ApplicationRole
                {
                    Name = roleVM.RoleName,
                };

                var state = await _roleManager.CreateAsync(role);

                if(state.Succeeded)
                {
                    return RedirectToAction("Index", "Role");
                }
                else
                {
                    foreach (var error in state.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                        return View(roleVM);
                    }
                }
            }
            return View(roleVM);
        }
    }
}
