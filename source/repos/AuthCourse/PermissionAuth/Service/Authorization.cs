using Microsoft.EntityFrameworkCore;
using PermissionAuth.Context;
using PermissionAuth.Dtos;
using PermissionAuth.Models;

namespace PermissionAuth.Service
{
    public class Authorization
    {
        private readonly ApplicationDbContext _dbContext;

        public Authorization(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> HasPermissionAsync(int userId, Permission permission)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null) return false;

            if (user.Role == Role.Admin) return true; // Admins have all permissions

            return user.Permissions.Any(p => p.Permission == permission);
        }
        private async Task<User> GetUserByIdAsync(int userId)
        {
            return await _dbContext.Users
                .Include(u => u.Permissions)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
