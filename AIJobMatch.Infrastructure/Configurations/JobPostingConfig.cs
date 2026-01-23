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
    public class JobPostingConfig : IEntityTypeConfiguration<JobPosting>
    {
        public void Configure(EntityTypeBuilder<JobPosting> builder)
        {
            
            builder.HasOne(j => j.Recruiter)
                   .WithMany(r => r.JobPostings)
                   .HasForeignKey(j => j.RecruiterId)
                   .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(j => j.Company)
                   .WithMany(c => c.JobPostings)
                   .HasForeignKey(j => j.CompanyId)
                   .OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}
