using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class MockInterviewResponse
    {
        public InterviewStatus InterviewStatus { get; set; }
        public Guid? NextQuestionId { get; set; }
        public string? NextQuestionText { get; set; }
        public int? NextQuestionOrder { get; set; }
    }
}
