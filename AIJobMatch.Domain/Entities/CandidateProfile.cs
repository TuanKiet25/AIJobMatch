using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class CandidateProfile : BaseEntity
    {
        public string? Jobtitle { get; set; }
        public string? AboutMe { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? AvatarUrl { get; set; }
        public string? DesiredJobTitle { get; set; } //vi tri mong muon
        public string? WorkLocation { get; set; }
        public string? JobType { get; set; } // Remote/Hybrid/Fulltime
        public string? Achievements { get; set; }
        public string? Contacts { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CandidateId { get; set; }
        public Candidate? Candidate { get; set; }
        public List<Skill>? Skills { get; set; } 
        public List<WorkExperiences>? WorkExperiences { get; set; } 
        public List<Education>? Educations { get; set; }    
    }
}
