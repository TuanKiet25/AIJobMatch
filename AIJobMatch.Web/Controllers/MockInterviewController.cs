using AIJobMatch.Application.IServices;
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
        public MockInterviewController(IMockInterviewService mockInterviewService)
        {
            _mockInterviewService = mockInterviewService;
        }
        [HttpPost("start-Interview")]
        public async Task<IActionResult> StartMockInterview([FromBody] StartInterviewRequest request)
        {
            var result = await _mockInterviewService.StartMockInterviewAsync(request);
            return HandleResult(result);
        }
        [HttpPost("chat-Interview")]
        public async Task<IActionResult> ChatInterview([FromBody] MockInterviewRequest request)
        {
            var result = await _mockInterviewService.ChatInterviewAsync(request);
            return HandleResult(result);
        }
        [HttpPost("evaluate-Interview/{mockInterviewId}")]
        public async Task<IActionResult> EvaluateInterview(Guid mockInterviewId)
        {
            var result = await _mockInterviewService.EvaluateInterviewAsync(mockInterviewId);
            return HandleResult(result);
        }
        [HttpGet("get-Interview-Result/{mockInterviewId}")]
        public async Task<IActionResult> GetInterviewResult(Guid mockInterviewId)
        {
            var result = await _mockInterviewService.GetMockInterviewResultAsync(mockInterviewId);
            return HandleResult(result);
        }
    }
}
