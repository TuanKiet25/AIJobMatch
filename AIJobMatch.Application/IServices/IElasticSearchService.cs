using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Domain.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.IServices
{
    public interface IElasticSearchService
    {
        Task<bool> IndexJobAsync(JobPostingDocument job);
        Task<List<JobPostingDocument>> RecommendJobsAsync(JobSearchRequest jobSearchRequest);
    }
}
