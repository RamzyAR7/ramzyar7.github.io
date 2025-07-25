using Microsoft.AspNetCore.Mvc;
using PermissionAuth.Dtos;

namespace PermissionAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigrationController:ControllerBase
    {
        //configration OBJ LIKE DICTIONARY
        private readonly IConfiguration _configuration;
        public ConfigrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet("GetConnectionString")]
        public IActionResult GetConnectionString()
        {
           var connectionString = _configuration.GetConnectionString("DefaultConnection");
            //var connectionString = _configuration["ConnectionStrings:DefaultConnection"];
            return Ok(new { ConnectionString = connectionString });
        }
        [HttpGet("GetJwtSettings")]
        public IActionResult GetJwtSettings()
        {
            var jwtSettings = _configuration.GetSection("jwt").Get<Jwt>();
            return Ok(jwtSettings);
        }
        [HttpGet("GetAllowedHosts")]
        public IActionResult GetAllowedHosts()
        {
            //var res = _configuration["AllowedHosts"];   
            var allowedHosts = _configuration.GetSection("AllowedHosts").Get<string[]>();
            return Ok(allowedHosts);
        }
        [HttpGet("GetLoggingLevel")]
        public IActionResult GetLoggingLevel()
        {
            //var loggingLevel = _configuration["Logging:LogLevel:Default"];
            var loggingLevel = _configuration.GetSection("Logging:LogLevel:Default").Get<string>();
            return Ok(new { LoggingLevel = loggingLevel }); 
        }
        [HttpGet("GetEnvironment")]
        public IActionResult GetEnvironment()
        {
            var environment = _configuration["ASPNETCORE_ENVIRONMENT"];
            return Ok(new { Environment = environment });
        }
    }
}
