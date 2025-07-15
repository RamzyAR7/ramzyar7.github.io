using Microsoft.AspNetCore.Identity;
using PermissionBasedAuth.Contants;
using PermissionBasedAuth.Entities;
using System.Linq;
using System.Security.Claims;

namespace PermissionBasedAuth.Seeding
{
    public static class IdentitySeeding
    {
        public static async Task SeedingRolesAsync(RoleManager<IdentityRole> _roleManager)
        {
            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole(Role.SuperAdmin.ToString()));
                await _roleManager.CreateAsync(new IdentityRole(Role.Admin.ToString()));
                await _roleManager.CreateAsync(new IdentityRole(Role.User.ToString()));
            }
        }

        public static async Task SeedingUsersAsync(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {
            var SuperAdmin = new ApplicationUser
            {

                UserName = "superadmin@example.com",
                Email = "superadmin@example.com",
                Address = "123 Super Admin St, Admin City",
            };

            var existingSuperAdmin = await _userManager.FindByEmailAsync(SuperAdmin.Email);

            if (existingSuperAdmin == null)
            {
                // create super admin user
                await _userManager.CreateAsync(SuperAdmin, "SuperAdmin@123");
                // adding role to user 
                await _userManager.SeedRoleToUserAsync(SuperAdmin.Email, Role.SuperAdmin.ToString());
                // adding claims to role
                await _roleManager.SeedClaimsToRoleAsync(Role.SuperAdmin.ToString(), "User");
            }
        }

        private static async Task SeedRoleToUserAsync(this UserManager<ApplicationUser> _userManager, string userEmail, string role)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (!await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }
        private static async Task SeedClaimsToRoleAsync(this RoleManager<IdentityRole> _roleManager, string roleName, string Model)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            if (role != null)
            {
                var permissions = Permission.GenratePermissionList(Model);
                foreach (var permission in permissions)
                {
                    
                    if (!roleClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                    {
                        await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                    }
                }

                //or
              
                //foreach (var permission in permissions)
                //{
                //    var claim =  new  Claim("Permission", permission);

                //    if (!roleClaims.Contains(claim))
                //    {
                //        await _roleManager.AddClaimAsync(role, claim);
                //    }
                //}
            }
        }
    }
}
