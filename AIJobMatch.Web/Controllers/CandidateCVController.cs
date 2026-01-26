using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIJobMatch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateCVController : MyBaseController
    {
        private readonly ICVService _cvService;
        public CandidateCVController(ICVService cvService)
        {
            _cvService = cvService;
        }
        [HttpPost("create-cv")]
        public async Task<IActionResult> CreateCV([FromBody] CVRequest cvRequest)
        {
            var result = await _cvService.CreateCVAsync(cvRequest);
            return HandleResult(result);
        }
        [HttpGet("get-cv-by-id/{cvId}")]
        public async Task<IActionResult> GetCVById(Guid cvId)
        {
            var result = await _cvService.GetCVByIdAsync(cvId);
            return HandleResult(result);
        }
        [HttpPut("update-cv/{cvId}")]
        public async Task<IActionResult> UpdateCV(Guid cvId, [FromBody] CVRequest cvRequest)
        {
            var result = await _cvService.UpdateCVAsync(cvId, cvRequest);
            return HandleResult(result);
        }
        [HttpDelete("delete-cv/{cvId}")]
        public async Task<IActionResult> DeleteCV(Guid cvId)
        {
            var result = await _cvService.DeleteCVAsync(cvId);
            return HandleResult(result);
        }
        [HttpGet("get-all-cvs-by-candidate")]
        public async Task<IActionResult> GetAllCVsByCandidateId()
        {
            var result = await _cvService.GetAllCVsByCandidateIdAsync();
            return HandleResult(result);
        }
        [HttpPost("active-cv/{cvId}")]
        public async Task<IActionResult> ActiveCv(Guid cvId, [FromQuery] bool isActive)
        {
            var result = await _cvService.ActiveCvAsync(cvId, isActive);
            return HandleResult(result);
        }
    }
}
