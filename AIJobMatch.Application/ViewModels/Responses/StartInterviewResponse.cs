using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class StartInterviewResponse
    {
        public Guid MockInterviewId { get; set; } 
        public Guid FirstQuestionId { get; set; } 
        public string FirstQuestionText { get; set; } 
    }
}
