using AIJobMatch.Application.IServices;
using AIJobMatch.Application.Services;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AIJobMatch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiCvSuggestController : MyBaseController
    {
        private readonly IAiCvService _aiCvService;
        public AiCvSuggestController(IAiCvService aiCvService)
        {
            _aiCvService = aiCvService;
        }   
        [HttpPost("suggest-cv")]
        public async Task<IActionResult> SuggestCv([FromBody] CVRequest cVRequest)
        {
            var result = await _aiCvService.SuggestCvDataAsync(cVRequest);
            return HandleResult(result);
        }
        [HttpPost("suggest-cv-by-Id")]
        public async Task<IActionResult> SuggestCvById([FromBody] Guid id)
        {
            var result = await _aiCvService.SuggestCvDataByIdAsync(id);
            return HandleResult(result);
        }
    }
}
