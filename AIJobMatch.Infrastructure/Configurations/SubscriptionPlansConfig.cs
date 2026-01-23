using AIJobMatch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Infrastructure.Configurations
{
    public class SubscriptionPlansConfig : IEntityTypeConfiguration<SubscriptionPlans>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPlans> builder)
        {
            builder.HasKey(sp => sp.Id);

            // Relationship với UserSubscription và Transactions được config ở phía "many"
            // (UserSubscriptionConfig và TransactionsConfig) để tránh conflict
            // Ở đây chỉ cần đảm bảo navigation properties được định nghĩa đúng
        }
    }
}
