using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class JobPostingResponse
    {
        public Guid Id { get; set; }
        public Guid RecruiterId { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } 
        public string RecruiterName { get; set; }
        public string Title { get; set; }
        public string JobType { get; set; }
        public int Quantity { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string Currency { get; set; }
        public bool IsNegotiable { get; set; }

        public string Description { get; set; }
        public string Requirement { get; set; }
        public string Benefits { get; set; }

        public int YearsOfExperience { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Address Information
        public AddressResponse Address { get; set; }
    }


}
