using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class WorkExperiences : BaseEntity
    {
        public Guid ProfileId { get; set; }
        public CandidateProfile? Profile { get; set; }
        public string? CompanyName { get; set; }
        public string? Position { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }
}
