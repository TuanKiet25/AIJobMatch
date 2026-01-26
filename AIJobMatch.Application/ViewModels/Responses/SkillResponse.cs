using AIJobMatch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class SkillResponse
    {
        public Guid Id { get; set; }
        public Guid ProfileId { get; set; }
        public string? SkillName { get; set; }
        public string? ProficiencyLevel { get; set; }
    }
}
