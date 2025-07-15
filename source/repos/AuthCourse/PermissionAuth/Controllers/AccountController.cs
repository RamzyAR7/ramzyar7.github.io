using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermissionAuth.Context;
using PermissionAuth.Dtos;
using PermissionAuth.Service;

namespace PermissionAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(ApplicationDbContext _context, CheckPassword _check, GenrateToken _token) : ControllerBase
    {
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            try
            {
                if (login == null)
                {
                    throw new ArgumentNullException(nameof(login));
                }
                var user = await _check.IsVaild(login.UserName, login.Password);
                string token = _token.GenrateJWTToken(user);

                return Ok(token);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
