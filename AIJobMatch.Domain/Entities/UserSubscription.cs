using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class UserSubscription : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public UserSubscriptionStatus Status { get; set; }
        public SubscriptionPlans? SubscriptionPlans { get; set; }
    }
}
