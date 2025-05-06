using EmployeeDirectoryOptimaPharm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDirectoryOptimaPharm.Configurations
{
    public class PassportConfiguration : BaseEntityConfiguration<Passport>
    {
        public override void Configure(EntityTypeBuilder<Passport> builder)
        {
            base.Configure(builder);

            builder.ToTable("Passports");

            builder.Property(p => p.Series)
                .HasColumnType("char(2)")
                .IsRequired();

            builder.Property(p => p.Number)
                .HasColumnType("char(6)")
                .IsRequired();

            builder.HasIndex(p => new { p.Series, p.Number })
                .IsUnique();
        }
    }
}
