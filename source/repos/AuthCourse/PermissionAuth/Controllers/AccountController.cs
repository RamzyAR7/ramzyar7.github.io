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
    [ApiExplorerSettings(GroupName = "Account")]
    public class AccountController(ApplicationDbContext _context, CheckPassword _check, GenrateToken _token) : ControllerBase
    {
        /// <summary>
        /// This Endpoint is for Login
        /// </summary>
        /// <param name="login"> LoginDto </param>
        /// <returns>Generate Jwt Bearer Token</returns>
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
