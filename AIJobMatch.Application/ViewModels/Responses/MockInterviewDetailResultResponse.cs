using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class MockInterviewDetailResultResponse
    {
        public Guid Id { get; set; }
        public int QuestionOrder { get; set; }
        public string QuestionText { get; set; }
        public string? CandidateAnswer { get; set; }
        public double? QuestionScore { get; set; }
        public string? AIFeedback { get; set; }
    }
}
