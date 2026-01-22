using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TransactionStatus = AIJobMatch.Domain.Enums.TransactionStatus;

namespace AIJobMatch.Domain.Entities
{
    public class Transactions : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public Decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string TransactionCode { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public SubscriptionPlans? SubscriptionPlans { get; set; }
    }
}
