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
    public interface IJobApplicationService
    {
        public Task<ServiceResult<JobApplicationResponse>> CreateJobApplicationAsync(JobApplicationRequest request);
        public Task<ServiceResult<JobApplicationResponse>> GetJobApplicationByIdAsync(Guid id);
        public Task<ServiceResult<List<JobApplicationResponse>>> GetJobApplicationsByCandidateIdAsync(Guid candidateId);
        public Task<ServiceResult<List<JobApplicationResponse>>> GetJobApplicationsByJobPostingIdAsync(Guid jobPostingId);
        public Task<ServiceResult<JobApplicationResponse>> UpdateJobApplicationAsync(Guid id, JobApplicationUpdateRequest request);
        public Task<ServiceResult<string>> DeleteJobApplicationAsync(Guid id);
        public Task<ServiceResult<string>> UpdateJobApplicationStatusAsync(Guid id, JobApplicationStatus status);
        public Task<ServiceResult<bool>> AnalyzeAndSaveAiResultAsync(Guid jobApplicationId);

    }
}
