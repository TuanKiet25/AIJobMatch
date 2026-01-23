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
    public class UserSubscriptionConfig : IEntityTypeConfiguration<UserSubscription>
    {
        public void Configure(EntityTypeBuilder<UserSubscription> builder)
        {
            builder.HasKey(us => us.Id);

            // UserSubscription (N) -> (1) SubscriptionPlans (qua PlanId)
            // Relationship với Account được config trong AccountConfig
            builder.HasOne(us => us.SubscriptionPlans)
                   .WithMany(sp => sp.UserSubscriptions)
                   .HasForeignKey(us => us.PlanId)
                   .OnDelete(DeleteBehavior.Restrict); // Không xóa subscription khi xóa plan

            // Index cho UserId để query nhanh
            builder.HasIndex(us => us.UserId);

            // Index cho PlanId để query nhanh
            builder.HasIndex(us => us.PlanId);

            // Composite index cho UserId và Status để query subscriptions của user theo status
            builder.HasIndex(us => new { us.UserId, us.Status });
        }
    }
}
