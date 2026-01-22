using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.IServices
{
    public interface IJobPostingService
    {
        Task<JobPostingResponse> CreateJobPostingAsync(JobPostingRequest request);
        Task<JobPostingResponse> GetJobPostingByIdAsync(Guid id);
        Task<List<JobPostingResponse>> GetAllJobPostingsAsync();
        Task<List<JobPostingResponse>> GetJobPostingsByCompanyIdAsync(Guid companyId);
        Task<bool> UpdateJobPostingAsync(Guid id, JobPostingUpdateRequest request);
        Task<bool> DeleteJobPostingAsync(Guid id);
    }
}
