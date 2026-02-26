using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class AiCvReviewResponse
    {
        public int Score { get; set; }
        public List<string> Strengths { get; set; }
        public List<string> Weaknesses { get; set; }
        public List<AiSuggestion> Suggestions { get; set; }
    }

    public class AiSuggestion
    {
        // Tên trường trong CVRequest (VD: "AboutMe", "Achievements", "WorkExperiences")
        public string Section { get; set; }

        // NẾU Section là một List, đây sẽ là vị trí của phần tử (0, 1, 2...). 
        // Nếu Section là trường đơn lẻ (như AboutMe), để null.
        public int? ItemIndex { get; set; }

        // Tên của thuộc tính con bên trong List (VD: "Description" của WorkExperience)
        // Nếu Section là trường đơn lẻ, để null.
        public string? SubSection { get; set; }

        public string OriginalText { get; set; }
        public string SuggestedText { get; set; }
        public string Reason { get; set; }
    }
}
