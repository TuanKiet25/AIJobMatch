using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class MockInterviewDetail : BaseEntity
    {
        public Guid MockInterviewId { get; set; }
        public int QuestionOrder { get; set; }
        public string QuestionText { get; set; } 
        public string? CandidateAnswer { get; set; } 
        public double? QuestionScore { get; set; } 
        public string? AIFeedback { get; set; } 
        public MockInterview? MockInterview { get; set; }
    }
}
