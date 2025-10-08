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

        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="login">The login credentials including email and password.</param>
        /// <returns>
        /// <list type="bullet">
        ///   <item><description><see cref="StatusCodes.Status200OK"/> - Returns a JWT token on successful login</description></item>
        ///   <item><description><see cref="StatusCodes.Status400BadRequest"/> - Invalid login request</description></item>
        ///   <item><description><see cref="StatusCodes.Status401Unauthorized"/> - Authentication failed due to invalid credentials</description></item>
        /// </list>
        /// </returns>
        /// <response code="200">Returns a JWT token on successful login</response>
        /// <response code="400">Invalid login request</response>
        /// <response code="401">Authentication failed due to invalid credentials</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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