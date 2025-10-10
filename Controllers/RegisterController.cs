using Microsoft.AspNetCore.Mvc;
using project_backend.Dtos.CustomerDtos;
using project_backend.Services.Interfaces;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _registerService;
        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        /// <summary>
        /// Registers a new customer account.
        /// </summary>
        /// <param name="register">The registration details including name, email, date of birth, and password.</param>
        /// <returns>
        /// <list type="bullet">
        ///   <item><description><see cref="StatusCodes.Status200OK"/> - Registration was successful</description></item>
        ///   <item><description><see cref="StatusCodes.Status400BadRequest"/> - User with the given email is already registered</description></item>
        /// </list>
        /// </returns>
        /// <response code="200">Registration was successful</response>
        /// <response code="400">User with the given email is already registered</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            var registred = await _registerService.RegisterAsync(register);

            if (!registred)
            {
                return BadRequest("User is already registered.");
            }

            return Ok("Registration successful.");
        }
    }
}