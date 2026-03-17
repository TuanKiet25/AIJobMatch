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
    public class MockInterviewConfig : IEntityTypeConfiguration<MockInterview>
    {
        public void Configure(EntityTypeBuilder<MockInterview> builder)
        {
            builder.HasMany(mi => mi.Details)
                   .WithOne(mid => mid.MockInterview)
                   .HasForeignKey(mid => mid.MockInterviewId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(mi => mi.Candidate)
                   .WithMany(c => c.MockInterviews)
                   .HasForeignKey(mi => mi.CandidateId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
