using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Requests
{
    public class MockInterviewRequest
    {
        public Guid MockInterviewId { get; set; }
        public Guid MockInterviewDetailId { get; set; }
        public string AnswerText { get; set; }
    }
}
