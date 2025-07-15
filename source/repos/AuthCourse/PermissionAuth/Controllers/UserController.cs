using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermissionAuth.Context;
using PermissionAuth.Dtos;
using PermissionAuth.Models;
using PermissionAuth.Service;

namespace PermissionAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(ApplicationDbContext _dbContext) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _dbContext.Users.ToList().Where(u => u.Name == "Admin");

            return Ok(users);
        }
    }
}
