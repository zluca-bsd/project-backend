using Microsoft.AspNetCore.Mvc;
using project_backend.Dtos.AiDtos;
using project_backend.Services.Interfaces;

namespace project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("books")]
        public async Task<ActionResult<AiResponse>> AskAiBooks([FromBody] AiRequest request)
        {
            var response = await _aiService.AskAi(request.Request);
            return Ok(response);
        }

        // [HttpPost("customers")]
        // public async Task<ActionResult<AiResponse>> AskAiCustomers([FromBody] AiRequest request)
        // {
        //     var response = await _aiService.AskAi<Customer>(request.Request);
        //     return Ok(response);
        // }
    }
}