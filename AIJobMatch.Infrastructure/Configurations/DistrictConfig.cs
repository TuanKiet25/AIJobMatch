using AIJobMatch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Infrastructure.Configurations
{
    public class DistrictConfig : IEntityTypeConfiguration<District>
    {
        public void Configure(EntityTypeBuilder<District> builder)
        {
            builder.HasKey(d => d.DistrictCode);
            builder.HasMany(d => d.Addresses)
                   .WithOne(a => a.District)
                   .HasForeignKey(a => a.DistrictCode)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(d => d.Wards)
                   .WithOne(w => w.District)
                   .HasForeignKey(w => w.DistrictCode)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
