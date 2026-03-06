using AIJobMatch.Application.IServices;
using AIJobMatch.Application.Services;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using AIJobMatch.Infrastructure.Filter;
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
        private readonly IValidateService _validateService;
        public AiCvSuggestController(IAiCvService aiCvService, IValidateService validateService)
        {
            _aiCvService = aiCvService;
            _validateService = validateService;
        }   
        [HttpPost("suggest-cv")]
        [PremiumDateCheck]
        public async Task<IActionResult> SuggestCv([FromBody] CVRequest cVRequest)
        {
            string checkResult = await _validateService.ValidateCandidateSubcription("Plus");
            if (checkResult == "Success")
            {
                var result = await _aiCvService.SuggestCvDataAsync(cVRequest);
                return HandleResult(result);
            }
            else
            {
                return BadRequest(checkResult);
            }
        }
        [HttpPost("suggest-cv-by-Id")]
        [PremiumDateCheck]
        public async Task<IActionResult> SuggestCvById([FromBody] Guid id)
        {
            string checkResult = await _validateService.ValidateCandidateSubcription("Plus");
            if (checkResult == "Success")
            {
                var result = await _aiCvService.SuggestCvDataByIdAsync(id);
                return HandleResult(result);
            }
            else
            {
                return BadRequest(checkResult);
            }
        }
    }
}
