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
        private readonly AppDbContext _context;
        public LoginController(IOptions<JwtSettings> jwtSettings, AppDbContext context)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }

        [HttpPost]
        public IActionResult Login([FromBody] Login login)
        {
            Console.WriteLine($"Received login attempt: '{login.Email} : {login.Password}'");

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid client request");
            }

            var customer = _context.Customers.FirstOrDefault(c => c.Email == login.Email);

            if (customer == null)
            {
                Console.WriteLine("No customer found with that email.");
                return Unauthorized("Wrong email or password");
            }

            if (!BCrypt.Net.BCrypt.Verify(login.Password, customer.Password))
            {
                return Unauthorized("Wrong email or password");
            }

            var tokenString = GenerateJwtToken(login.Email);

            return Ok(new AuthenticatedResponse { Token = tokenString });
        }

        private string GenerateJwtToken(string email)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
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