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
    public class JobApplicationConfig : IEntityTypeConfiguration<JobApplication>
    {
        public void Configure(EntityTypeBuilder<JobApplication> builder)
        {
            builder.HasOne(ja => ja.JobPosting)
                   .WithMany(jp => jp.JobApplications)
                   .HasForeignKey(ja => ja.JobPostingId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(ja => ja.CandidateProfile)
                   .WithMany(cp => cp.JobApplications)
                   .HasForeignKey(ja => ja.CandidateProfileId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(ja => ja.Candidate)
                     .WithMany(c => c.JobApplications)
                     .HasForeignKey(ja => ja.CandidateId)
                     .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
