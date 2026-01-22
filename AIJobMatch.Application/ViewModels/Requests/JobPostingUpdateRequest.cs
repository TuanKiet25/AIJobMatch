using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Requests
{
    public class JobPostingUpdateRequest
    {
        [Required]
        public string Title { get; set; }
        public string JobType { get; set; }
        public int Quantity { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string Currency { get; set; } = "VND";
        public bool IsNegotiable { get; set; }

        public string Description { get; set; }
        public string Requirement { get; set; }
        public string Benefits { get; set; }

        public int YearsOfExperience { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
