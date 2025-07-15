using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermissionBasedAuth.Contants;
using PermissionBasedAuth.Filter;
using PermissionBasedAuth.Models;
using System.Security.Claims;

namespace PermissionBasedAuth.Controllers
{
    [Authorize]
    public class RoleController(RoleManager<IdentityRole> _roleManager) : Controller
    {
        [PermissionFilterFactory(Permission.Role.Read)]
        public async Task<IActionResult> Index()
        {
            var Roles = await _roleManager.Roles.ToListAsync();
            
            return View(Roles);
        }

        [HttpGet]
        [PermissionFilterFactory(Permission.Role.Create)]
        public IActionResult Create()
        {
            return View(new RoleVM());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionFilterFactory(Permission.Role.Create)]
        public async Task<IActionResult> Create(RoleVM model)
        {
            if (ModelState.IsValid)
            {
                var IsRoleExist = await _roleManager.FindByNameAsync(model.Name);

                if (IsRoleExist != null)
                {
                    ModelState.AddModelError("", "The role name already exists");
                    return View(model);
                }

                var newRole = new IdentityRole
                {
                    Name = model.Name
                };
                var state = await _roleManager.CreateAsync(newRole);
                if(state.Succeeded)
                {
                    TempData["Created"] = "Role created successfully";
                    return RedirectToAction("Index", "Role");
                }
                else
                {
                    foreach (var error in state.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        [PermissionFilterFactory(Permission.Role.Update)]
        public async Task<IActionResult> AddClaims(string Id)
        {
            var role = await _roleManager.FindByIdAsync(Id);

            if (role == null)
            {
                ModelState.AddModelError("", "there is no Role");
                return View("Index");
            }

            var parentClaimNames = Enum.GetNames(typeof(Forms));

            var roleClaims = new RoleClaimsVM
            {
                Id= role.Id,
                RoleName= role.Name,
                ParentClaims= new List<ParentClaimVM>()
            };

            var claimsFromRole = await _roleManager.GetClaimsAsync(role);
            foreach (var parentName in parentClaimNames)
            {
                var parentClaim = new ParentClaimVM
                {
                    ParentName = parentName,
                    ChildClaims = new List<CheckBoxVM>()
                };
                roleClaims.ParentClaims.Add(parentClaim);

                var permissions = Permission.GenratePermissionList(parentName);
                foreach (var permission in permissions)
                {
                    bool checkBox = HasClaim(claimsFromRole, permission);
                    parentClaim.ChildClaims.Add(new CheckBoxVM
                    {
                        Name = permission,
                        IsSelected = checkBox,
                    });
                }
            }
            return View(roleClaims); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionFilterFactory(Permission.Role.Update)]
        public async Task<IActionResult> AddClaims(string Id, RoleClaimsVM roleClaimsVM)
        {
            if (ModelState.IsValid)
            {
                if(roleClaimsVM.Id != Id)
                {
                    return BadRequest("Hacking haaaaaa");
                }
                var role = await _roleManager.FindByIdAsync(roleClaimsVM.Id);

                if (role == null) {
                    ModelState.AddModelError("", "there is no role");
                    return View(roleClaimsVM);
                }

                var claims = await _roleManager.GetClaimsAsync(role);

                foreach (var claim in claims)
                {
                    var removeResult = await _roleManager.RemoveClaimAsync(role, claim);
                    if (!removeResult.Succeeded)
                    {
                        foreach (var error in removeResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(roleClaimsVM);
                    }
                }

                foreach (var parent in roleClaimsVM.ParentClaims)
                {
                    foreach (var chiledClaim in parent.ChildClaims)
                    {
                        if(chiledClaim.IsSelected)
                        {
                            var AddResult = await _roleManager.AddClaimAsync(role, new Claim("Permission", chiledClaim.Name));
                            if (!AddResult.Succeeded)
                            {
                                foreach (var error in AddResult.Errors)
                                {
                                    ModelState.AddModelError("", error.Description);
                                }
                                return View(roleClaimsVM);
                            }
                        }
                    }
                }
                TempData["Created"] = "Claims updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}");
                    foreach (var e in error.Value.Errors)
                    {
                        Console.WriteLine($"Error: {e.ErrorMessage}");
                    }
                }
            }
            return View(roleClaimsVM);
        }
        private bool HasClaim(IEnumerable<Claim> claims, string permission)
        {
            return claims.Any(c => c.Type == "Permission" && c.Value == permission);
        }
    }
}
