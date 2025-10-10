using Microsoft.AspNetCore.Mvc;
using project_backend.Dtos.CustomerDtos;
using project_backend.Services.Interfaces;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
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
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var tokenString = await _loginService.Login(login);

            if (tokenString == null)
            {
                return Unauthorized("Wrong email or password");
            }

            return Ok(new AuthenticatedResponse { Token = tokenString });
        }
    }
}