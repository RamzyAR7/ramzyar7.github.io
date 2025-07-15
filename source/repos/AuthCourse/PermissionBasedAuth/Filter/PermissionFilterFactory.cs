using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using PermissionBasedAuth.Entities;

namespace PermissionBasedAuth.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionFilterFactory : Attribute, IFilterFactory
    {
        public bool IsReusable => false;
        private readonly string _permission;

        public PermissionFilterFactory(string Permission)
        {
            _permission = Permission;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var auth = serviceProvider.GetRequiredService<AuthorizationHelper>();

            return new PermissionFilter(_permission, auth, userManager, roleManager);
        }
    }
}
