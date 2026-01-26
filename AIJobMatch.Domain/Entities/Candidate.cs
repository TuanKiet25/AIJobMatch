using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class Candidate
    {
        [Key]
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }   
        public string? Skill { get; set; }
        public string? Education { get; set; }
        public List<CandidateProfile>? CandidateProfiles { get; set; }
    }
}
