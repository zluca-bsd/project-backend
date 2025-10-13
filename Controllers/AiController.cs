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

        [HttpPost]
        public async Task<ActionResult<AiResponseDto>> ChatWithAi([FromBody] AiRequestDto request)
        {
            var response = await _aiService.GetChatResponseAsync(request);
            return Ok(response);
        }
    }
}
