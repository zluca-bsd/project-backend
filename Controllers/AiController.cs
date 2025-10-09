using Microsoft.AspNetCore.Mvc;
using project_backend.Dtos.AiDtos;


namespace project_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly AiService _aiService;

        public AiController(AiService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost]
        public async Task<ActionResult<AiResponse>> GetAiResponse([FromBody] AiQuestion question)
        {
            string response = await _aiService.GetAiResponseAsync(question.Question);

            return Ok(new AiResponse { Response = response});
        }
    }
}