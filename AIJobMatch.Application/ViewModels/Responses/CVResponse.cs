using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class CVResponse
    {
        public Guid Id { get; set; }
        public string? Template { get; set; }
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
        public Guid CandidateId { get; set; }
        public List<SkillResponse>? Skills { get; set; }
        public List<WorkExResponse>? WorkExperiences { get; set; }
        public List<EducationResponse>? Educations { get; set; }
    }
}
