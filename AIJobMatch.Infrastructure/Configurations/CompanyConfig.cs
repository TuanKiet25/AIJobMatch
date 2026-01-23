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
    public class CompanyConfig : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasMany(c => c.addresses)
                   .WithOne(a => a.Company)
                   .HasForeignKey(a => a.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
