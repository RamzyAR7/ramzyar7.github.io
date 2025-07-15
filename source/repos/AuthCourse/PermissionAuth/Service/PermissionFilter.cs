using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PermissionAuth.Dtos;
using System.Security.Claims;

namespace PermissionAuth.Service
{
    public class PermissionFilter(Permission _permission, Authorization _authrization) : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userIdClaim = context.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var hasPermission = await _authrization.HasPermissionAsync(userId, _permission);

            if(!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionFilterFactory : Attribute, IFilterFactory
    {
        public bool IsReusable => false;
        private readonly Permission _permission;

        public PermissionFilterFactory(Permission permission)
        {
            _permission = permission;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var authService = serviceProvider.GetRequiredService<Authorization>();
            return new PermissionFilter(_permission, authService);
        }
    }

}
