using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class Education : BaseEntity
    {
        public Guid ProfileId { get; set; }
        public CandidateProfile? Profile { get; set; }
        public string? SchoolName { get; set; }
        public string? Degree { get; set; }
        public string? Major { get; set; }
        public string? Grade { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }
}
