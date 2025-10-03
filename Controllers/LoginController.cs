using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using project_backend.Models;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        public LoginController(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost]
        public IActionResult Login([FromBody] Login user)
        {
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }

            if (user.Username == "johndoe" && user.Password == "def@123")
            {
                if (string.IsNullOrWhiteSpace(_jwtSettings.Secret))
                {
                    Console.WriteLine(_jwtSettings.Secret);
                    Console.WriteLine("_jwtS123123123113232ettings.Secret");
                    Console.WriteLine($"Secret: '{_jwtSettings.Secret}'");
                    Console.WriteLine($"Issuer: '{_jwtSettings.Issuer}'");
                    Console.WriteLine($"Audience: '{_jwtSettings.Audience}'");

                    return StatusCode(500, "JWT secret is not configured.");
                }

                var tokenString = GenerateJwtToken(user.Username);

                return Ok(new AuthenticatedResponse { Token = tokenString });
            }
            return Unauthorized("Wrong username or password");
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}