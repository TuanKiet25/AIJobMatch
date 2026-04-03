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
    public class AccountConfig : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.Id);
            builder.HasMany(a => a.Addresses)
                   .WithOne(add => add.Account)
                   .HasForeignKey(add => add.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(a => a.Candidate)
                   .WithOne(c => c.Account)
                   .HasForeignKey<Candidate>(c => c.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(a => a.Recruiter)
                     .WithOne(r => r.Account)
                     .HasForeignKey<Recruiter>(r => r.AccountId)
                     .OnDelete(DeleteBehavior.Cascade);
            
            // Configure relationship with UserSubscriptions
            builder.HasMany(a => a.UserSubscriptions)
                   .WithOne(c => c.Account)
                   .HasForeignKey(us => us.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(a => a.Transactions)
                   .WithOne()
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
