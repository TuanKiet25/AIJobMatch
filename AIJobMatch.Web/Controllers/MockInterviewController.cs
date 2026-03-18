using AIJobMatch.Application.IServices;
using AIJobMatch.Application.Services;
using AIJobMatch.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIJobMatch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MockInterviewController : MyBaseController
    {
        private readonly IMockInterviewService _mockInterviewService;
        private readonly IValidateService _validateService;
        public MockInterviewController(IMockInterviewService mockInterviewService, IValidateService validateService)
        {
            _mockInterviewService = mockInterviewService;
            _validateService = validateService;
        }
        [HttpPost("start-Interview")]
        public async Task<IActionResult> StartMockInterview([FromBody] StartInterviewRequest request)
        {
            string checkResult = await _validateService.ValidateCandidateSubcription("Pro");
            if (checkResult == "Success")
            {
                var result = await _mockInterviewService.StartMockInterviewAsync(request);
                return HandleResult(result);
            }
            else
            {
                return BadRequest(checkResult);
            }
            
        }
        [HttpPost("chat-Interview")]
        public async Task<IActionResult> ChatInterview([FromBody] MockInterviewRequest request)
        {
            string checkResult = await _validateService.ValidateCandidateSubcription("Pro");
            if (checkResult == "Success")
            {
                var result = await _mockInterviewService.ChatInterviewAsync(request);
                return HandleResult(result);
            }
            else
            {
                return BadRequest(checkResult);
            }
            
        }
        [HttpPost("evaluate-Interview/{mockInterviewId}")]
        public async Task<IActionResult> EvaluateInterview(Guid mockInterviewId)
        {
            string checkResult = await _validateService.ValidateCandidateSubcription("Pro");
            if (checkResult == "Success")
            {
                var result = await _mockInterviewService.EvaluateInterviewAsync(mockInterviewId);
                return HandleResult(result);
            }
            else
            {
                return BadRequest(checkResult);
            }
            
        }
        [HttpGet("get-Interview-Result/{mockInterviewId}")]
        public async Task<IActionResult> GetInterviewResult(Guid mockInterviewId)
        {
            string checkResult = await _validateService.ValidateCandidateSubcription("Pro");
            if (checkResult == "Success")
            {
                var result = await _mockInterviewService.GetMockInterviewResultAsync(mockInterviewId);
                return HandleResult(result);
            }
            else
            {
                return BadRequest(checkResult);
            }
            
        }
    }
}
