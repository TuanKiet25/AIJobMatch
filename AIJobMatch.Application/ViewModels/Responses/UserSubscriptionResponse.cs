using AIJobMatch.Domain.Entities;
using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class UserSubscriptionResponse
    {
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public string? AccountFullName { get; set; }
        public UserSubscriptionStatus Status { get; set; }
        public string? SubscriptionPlansName { get; set; }
        public Role SubscriptionPlansTargetRole { get; set; }
        public decimal SubscriptionPlansPrice { get; set; }
        public int SubscriptionPlansDurationInDays { get; set; }
        public string? SubscriptionPlansFeatures { get; set; }
    }
}
