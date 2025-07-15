using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PermissionBasedAuth.Entities;
using System.Security.Claims;

namespace PermissionBasedAuth.Filter
{
    public class PermissionFilter
        (string _permission, AuthorizationHelper _auth,
        UserManager<ApplicationUser> _userManager,
        RoleManager<IdentityRole> _roleManager)
        :IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // var userId = _signInManager.Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // or
            var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null)
            {
                context.Result = new ForbidResult();
                return;
            }
            bool hasPermission = await _auth.HasPermissionAsync(_permission, roles.ToList());

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
