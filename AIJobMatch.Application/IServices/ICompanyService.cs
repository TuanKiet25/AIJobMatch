using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.IServices
{
    public interface ICompanyService
    {
        public Task<ServiceResult<string>> VerifyCompanyAsync(Guid companyId, VerificationStatus verificationStatus, string mess);
        public Task<ServiceResult<string>> UpdateCompanyAsync(Guid companyId, CompanyUpdateResquest request);
        public Task<ServiceResult<List<CompanyRegisterResponse>>> GetAllCompanyAsync();
        public Task<ServiceResult<CompanyRegisterResponse>> GetCompanyByIdAsync(Guid companyId);
        public Task<ServiceResult<string>> DeleteCompanyAsync(Guid companyId);
        public Task<ServiceResult<List<CompanyRegisterResponse>>> GetAllCompanyPedingVerifyAsync();
        public Task<ServiceResult<string>> GetCompanyInviteCodeAsync(Guid companyId);
        public Task<ServiceResult<CompanyRegisterResponse>> GetCompanyByRecruiterId (Guid id);
        public Task<ServiceResult<List<UserResponse>>> GetAllRecruiterbyCombanyIdAsync(Guid id);
    }
}
