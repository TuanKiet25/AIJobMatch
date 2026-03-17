using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Requests
{
    public class StartInterviewRequest
    {
        public Guid CandidateId { get; set; }
        public Guid CandidateProfileId { get; set; }
        public InterviewDifficulty InterviewDifficulty { get; set; }
        public string? CustomTargetJobTitle { get; set; }
    }
}
