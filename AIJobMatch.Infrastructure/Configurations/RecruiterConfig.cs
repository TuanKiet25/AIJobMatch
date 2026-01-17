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
    public class RecruiterConfig : IEntityTypeConfiguration<Recruiter>
    {
        public void Configure(EntityTypeBuilder<Recruiter> builder)
        {
            builder.HasOne(r => r.Company)
                   .WithMany(a => a.Recruiters)
                   .HasForeignKey(r => r.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
