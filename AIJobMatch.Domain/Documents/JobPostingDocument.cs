using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Documents
{
    public class JobPostingDocument
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Requirement { get; set; }
        public int YearsOfExperience { get; set; }
        public bool IsActive { get; set; }
    }
}
