using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIJobMatch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostingController : ControllerBase
    {
        private readonly IJobPostingService _jobPostingService;

        public JobPostingController(IJobPostingService jobPostingService)
        {
            _jobPostingService = jobPostingService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateJobPosting([FromBody] JobPostingRequest request)
        {
            try
            {
               

                var result = await _jobPostingService.CreateJobPostingAsync(request);
                return CreatedAtAction(nameof(GetJobPostingById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetJobPostingById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("Invalid job posting ID");

                var result = await _jobPostingService.GetJobPostingByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Job posting not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllJobPostings()
        {
            try
            {
                var result = await _jobPostingService.GetAllJobPostingsAsync();
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("No job postings found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("company/{companyId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetJobPostingsByCompanyId(Guid companyId)
        {
            try
            {
                if (companyId == Guid.Empty)
                    return BadRequest("Invalid company ID");

                var result = await _jobPostingService.GetJobPostingsByCompanyIdAsync(companyId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("No job postings found for this company");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateJobPosting(Guid id, [FromBody] JobPostingUpdateRequest request)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("Invalid job posting ID");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _jobPostingService.UpdateJobPostingAsync(id, request);
                if (!result)
                    return BadRequest("Update failed");

                return Ok("Job posting updated successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Job posting not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteJobPosting(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest("Invalid job posting ID");

                var result = await _jobPostingService.DeleteJobPostingAsync(id);
                if (!result)
                    return BadRequest("Delete failed");

                return Ok("Job posting deleted successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Job posting not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
