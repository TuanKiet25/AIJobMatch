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
    public class ProfileConfig : IEntityTypeConfiguration<CandidateProfile>
    {
        public void Configure(EntityTypeBuilder<CandidateProfile> builder)
        {
            builder.HasOne(p => p.Candidate)
                   .WithMany(c => c.CandidateProfiles)
                   .HasForeignKey(p => p.CandidateId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(p => p.WorkExperiences)
                   .WithOne(we => we.Profile)
                   .HasForeignKey(we => we.ProfileId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(p => p.Skills)
                   .WithOne(s => s.Profile)
                   .HasForeignKey(s => s.ProfileId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(p => p.Educations)
                     .WithOne(e => e.Profile)
                     .HasForeignKey(e => e.ProfileId)
                     .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
