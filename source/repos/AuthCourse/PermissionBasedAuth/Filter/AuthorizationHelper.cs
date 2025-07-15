using Microsoft.AspNetCore.Identity;
using PermissionBasedAuth.Entities;
using System.Security.Claims;

namespace PermissionBasedAuth.Filter
{
    public class AuthorizationHelper(RoleManager<IdentityRole> _roleManager)
    {
        public async Task<bool> HasPermissionAsync(string permission, List<string> roles)
        {
            bool hasPermission = false;
            foreach (var roleName in roles)   
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                var claims = await _roleManager.GetClaimsAsync(role);
                if(claims.Any(c => c.Type == "Permission" && c.Value == permission))
                {
                    return true;
                }
            }
            return hasPermission;
        }
    }
}
