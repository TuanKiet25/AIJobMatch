using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Requests
{
    public class SubscriptionPlanRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public Role TargetRole { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal Price { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 day")]
        public int DurationInDays { get; set; }
        [Required]
        public string Features { get; set; }
    }
}
