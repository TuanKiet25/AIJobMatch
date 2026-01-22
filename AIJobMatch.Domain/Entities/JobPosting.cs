using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class JobPosting : BaseEntity
    {
        // --- Foreign Keys ---
        public Guid RecruiterId { get; set; } 
        public Recruiter Recruiter { get; set; }

        public Guid CompanyId { get; set; } 
        public Company Company { get; set; }

        //Basic Info
        [Required]
        public string Title { get; set; }
        public string JobType { get; set; } 
        public int Quantity { get; set; } 

        //Salary
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string Currency { get; set; } = "VND"; // VND/USD
        public bool IsNegotiable { get; set; } 

        public string Description { get; set; }
        public string Requirement { get; set; }
        public string Benefits { get; set; }

        public int YearsOfExperience { get; set; }  
        public DateTime ExpiryDate { get; set; } 
        public bool IsActive { get; set; } = true; 
        public int ViewCount { get; set; } 
    }
}

