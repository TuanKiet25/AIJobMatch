using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class SubscriptionPlanResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Role TargetRole { get; set; }
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public string Features { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool isDeleted { get; set; }
    }
}
