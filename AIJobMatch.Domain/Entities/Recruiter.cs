using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class Recruiter
    {
        [Key]
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }
        public Guid? CompanyId { get; set; }
        public Company? Company { get; set; }
        public List<JobPosting>? JobPostings { get; set; }
    }
}
