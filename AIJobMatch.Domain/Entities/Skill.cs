using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class Skill : BaseEntity
    {
        public Guid ProfileId { get; set; }
        public CandidateProfile? Profile { get; set; }
        public string? SkillName { get; set; }
        public string? ProficiencyLevel { get; set; }

    }
}
