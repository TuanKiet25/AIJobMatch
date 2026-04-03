using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class JobSearchResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Requirement { get; set; }
        public int YearsOfExperience { get; set; }
        public bool IsActive { get; set; }
    }
}
