using AIJobMatch.Application.IServices;
using AIJobMatch.Application.Services;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIJobMatch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : MyBaseController
    {
        private readonly IJobApplicationService _jobApplicationService;
        public ApplicationController(IJobApplicationService jobApplicationService)
        {
            _jobApplicationService = jobApplicationService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateJobApplication(JobApplicationRequest request)
        {
            var result = await _jobApplicationService.CreateJobApplicationAsync(request);
            return HandleResult(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobApplication(Guid id)
        {
            var result = await _jobApplicationService.DeleteJobApplicationAsync(id);
            return HandleResult(result);
        }
        [HttpGet("Get_JobApplication_By_Id/{id}")]
        public async Task<IActionResult> GetJobApplicationById(Guid id)
        {
            var result = await _jobApplicationService.GetJobApplicationByIdAsync(id);
            return HandleResult(result);
        }
        [HttpGet("Get_JobApplications_By_CandidateId/{candidateId}")]
        public async Task<IActionResult> GetJobApplicationsByCandidateId(Guid candidateId)
        {
            var result = await _jobApplicationService.GetJobApplicationsByCandidateIdAsync(candidateId);
            return HandleResult(result);
        }
        [HttpGet("Get_JobApplications_By_JobPostingId/{jobPostingId}")]
        public async Task<IActionResult> GetJobApplicationsByJobPostingId(Guid jobPostingId)
        {
            var result = await _jobApplicationService.GetJobApplicationsByJobPostingIdAsync(jobPostingId);
            return HandleResult(result);
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateJobApplication(Guid id, JobApplicationUpdateRequest request)
        {
            var result = await _jobApplicationService.UpdateJobApplicationAsync(id, request);
            return HandleResult(result);
        }
        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateJobApplicationStatus(Guid id, JobApplicationStatus status)
        {
            var result = await _jobApplicationService.UpdateJobApplicationStatusAsync(id, status);
            return HandleResult(result);
        }

    }
}
