using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class MockInterview : BaseEntity
    {
        public Guid CandidateId { get; set; }
        public string? CustomTargetJobTitle { get; set; } //nếu candidate muốn phỏng vấn vị trí khác với cv thì ghi vào
        public double? OverallScore { get; set; }
        public string? OverallFeedback { get; set; }
        public DateTime StartTime { get; set; } 
        public Candidate? Candidate { get; set; }
        required
        public string CvSnapshot { get; set; } 
        public InterviewDifficulty InterviewDifficulty { get; set; }
        public InterviewStatus InterviewStatus { get; set; } 
        public List<MockInterviewDetail>? Details { get; set; } 

    }
}
