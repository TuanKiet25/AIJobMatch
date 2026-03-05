using AIJobMatch.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Requests
{
    public class JobApplicationRequest
    {
        public JobPostingResponse postingResponse { get; set; }
        public Guid CandidateProfileId { get; set; }
        public Guid CandidateId { get; set; }
        public string? CVUrl { get; set; }
        public string? CoverLetter { get; set; }
    }
    public class JobApplicationUpdateRequest
    {
        public Guid CandidateProfileId { get; set; }
        public string? CVUrl { get; set; }
        public string? CoverLetter { get; set; }
    }
}
