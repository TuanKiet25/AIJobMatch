using AIJobMatch.Application.ViewModels.Responses;
using AIJobMatch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Requests
{
    public class CVRequest
    {
        public string? FullName { get; set; }
        public string? Jobtitle { get; set; }
        public string? AboutMe { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? AvatarUrl { get; set; }
        public string? DesiredJobTitle { get; set; } 
        public string? WorkLocation { get; set; }
        public string? JobType { get; set; } 
        public string? Achievements { get; set; }
        public string? Contacts { get; set; }
        public bool IsActive { get; set; } 
        public Guid CandidtateId { get; set; }
        public List<SkillRequest>? Skills { get; set; }
        public List<WorkExRequest>? WorkExperiences { get; set; }
        public List<EducationRequest>? Educations { get; set; }
    }
}
