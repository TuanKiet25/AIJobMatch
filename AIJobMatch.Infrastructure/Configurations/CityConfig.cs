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
    public class CityConfig : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(c => c.CityCode);
            builder.HasMany(c => c.Districts)
                   .WithOne(d => d.City)
                   .HasForeignKey(d => d.CityCode)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(c => c.Addresses)
                   .WithOne(a => a.City)
                   .HasForeignKey(a => a.CityCode)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
