using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class SubscriptionPlans : BaseEntity
    {
        public string Name { get; set; }
        public Role TargetRole { get; set; }
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public string Features { get; set; }
        public List<UserSubscription>? UserSubscriptions { get; set; }
        public List<Transactions>? Transactions { get; set; }
    }
}
