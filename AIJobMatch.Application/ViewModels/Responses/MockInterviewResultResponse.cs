using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class MockInterviewResultResponse
    {
        public Guid Id { get; set; }
        public string? CustomTargetJobTitle { get; set; }
        public double? OverallScore { get; set; }
        public string? OverallFeedback { get; set; }
        public DateTime StartTime { get; set; }
        public InterviewStatus InterviewStatus { get; set; }
        public List<MockInterviewDetailResultResponse> Details { get; set; } = new();
    }
}
