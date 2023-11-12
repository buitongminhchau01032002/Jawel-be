using Jawel_be.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jawel_be.Utils
{
    public class Jwt
    {
        private readonly IConfiguration _config;
        public Jwt(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateToken(UserAccount user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username)
            };
            var token = new JwtSecurityToken(
                null,
                null,
                claims,
                expires: DateTime.Now.AddYears(10),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public string GenerateToken(CustomerAccount user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Phone)
            };
            var token = new JwtSecurityToken(
                null,
                null,
                claims,
                expires: DateTime.Now.AddYears(10),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
