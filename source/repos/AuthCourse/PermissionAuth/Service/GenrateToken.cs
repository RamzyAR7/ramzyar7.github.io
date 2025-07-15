using Microsoft.IdentityModel.Tokens;
using PermissionAuth.Dtos;
using PermissionAuth.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PermissionAuth.Service
{
    public class GenrateToken(Jwt _options)
    {

        public string GenrateJWTToken(User _user)
        {
            var TokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.SigningKey)), SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, _user.Id.ToString()),
                    new Claim(ClaimTypes.Name, _user.Name),
                })
            };

            var securitytoken = TokenHandler.CreateToken(tokenDescriptor);
            var token = TokenHandler.WriteToken(securitytoken);
            return token;
        }
    }
}
