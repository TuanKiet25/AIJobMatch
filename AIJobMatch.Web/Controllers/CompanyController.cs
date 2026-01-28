using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIJobMatch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : MyBaseController
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var result = await _companyService.DeleteCompanyAsync(id);
            return HandleResult(result);
        }
        [HttpGet("Get_All_Companies_Pending_Verify")]
        public async Task<IActionResult> GetAllCompaniesPendingVerify()
        {
            var result = await _companyService.GetAllCompanyPedingVerifyAsync();
            return HandleResult(result);
        }
        [HttpGet("Get_All_Companies")]
        public async Task<IActionResult> GetAllCompanies()
        {
            var result = await _companyService.GetAllCompanyAsync();
            return HandleResult(result);
        }
        [HttpGet("Get_Company_By_Id/{id}")]
        public async Task<IActionResult> GetCompanyById(Guid id)
        {
            var result = await _companyService.GetCompanyByIdAsync(id);
            return HandleResult(result);
        }
        [HttpPost("Verify_Company/{id}")]
        public async Task<IActionResult> VerifyCompany(Guid id, VerificationStatus verificationStatus, string mess)
        {
            var result = await _companyService.VerifyCompanyAsync(id, verificationStatus, mess);
            return HandleResult(result);
        }
        [HttpPost("update-company/{id}")]   
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyUpdateResquest request)
        {
            var result = await _companyService.UpdateCompanyAsync(id, request);
            return HandleResult(result);
        }
        [HttpGet("get-company-inivteCode/{id}")]
        public async Task<IActionResult> GetCompanyInviteCode(Guid id)
        {
            var result = await _companyService.GetCompanyInviteCodeAsync(id);
            return HandleResult(result);
        }
        [HttpGet("get-company-by-recruiterId/{id}")]
        public async Task<IActionResult> GetCompanyByRecruiterId(Guid id)
        {
            var result = await _companyService.GetCompanyByRecruiterId(id);
            return HandleResult(result);
        }
        [HttpGet("get-all-recruiter-by-companyId/{id}")]
        public async Task<IActionResult> GetAllRecruiterbyCompanyId(Guid id)
        {
            var result = await _companyService.GetAllRecruiterbyCombanyIdAsync(id);
            return HandleResult(result);
        }
    }
}
