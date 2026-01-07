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
    public class WardConfig : IEntityTypeConfiguration<Ward>
    {
        public void Configure(EntityTypeBuilder<Ward> builder)
        {
            builder.HasKey(w => w.WardCode);
            builder.HasMany(w => w.Addresses)
                   .WithOne(a => a.Ward)
                   .HasForeignKey(a => a.WardCode)
                   .OnDelete(DeleteBehavior.Restrict);
        }


    }
}

