using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class JobApplication : BaseEntity
    {
        public Guid JobPostingId { get; set; }
        public Guid CandidateProfileId { get; set; }
        public Guid CandidateId { get; set; }
        public JobPosting? JobPosting { get; set; }
        public CandidateProfile? CandidateProfile { get; set; }
        public Candidate? Candidate { get; set; }
        public JobApplicationStatus Status { get; set; } = JobApplicationStatus.Pending;
        public string ProfilesSnapshot { get; set; } 
        public string? CVUrl { get; set; }
        public double MatchScore { get; set; } = 0;
        public string? AIAnalysis { get; set; }
        public string? CoverLetter { get; set; }    

    }
}
