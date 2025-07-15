using Microsoft.EntityFrameworkCore;
using PermissionAuth.Context;
using PermissionAuth.Models;

namespace PermissionAuth.Service
{
    public class CheckPassword(ApplicationDbContext _dbContext)
    {
        public async Task<User> IsVaild(string username, string password)
        {
            var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Name == username && u.Password == password);
            if (user == null)
            {
                throw new  UnauthorizedAccessException("Invalid username or password.");
            }

            return user;    
        }
    }
}
