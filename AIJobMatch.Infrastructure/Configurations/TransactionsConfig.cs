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
    public class TransactionsConfig : IEntityTypeConfiguration<Transactions>
    {
        public void Configure(EntityTypeBuilder<Transactions> builder)
        {
            builder.HasKey(t => t.Id);

            // Transactions (N) -> (1) SubscriptionPlans (qua PlanId)
            // Relationship với Account được config trong AccountConfig
            builder.HasOne(t => t.SubscriptionPlans)
                   .WithMany(sp => sp.Transactions)
                   .HasForeignKey(t => t.PlanId)
                   .OnDelete(DeleteBehavior.Restrict); // Không xóa transaction khi xóa plan

            // Index cho TransactionCode để tìm kiếm nhanh
            builder.HasIndex(t => t.TransactionCode)
                   .IsUnique();

            // Index cho UserId để query nhanh
            builder.HasIndex(t => t.UserId);
        }
    }
}
