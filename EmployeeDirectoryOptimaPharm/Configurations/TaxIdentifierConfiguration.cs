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
    public class TaxIdentifierConfiguration : BaseEntityConfiguration<TaxIdentifier>
    {
        public override void Configure(EntityTypeBuilder<TaxIdentifier> builder)
        {
            base.Configure(builder);

            builder.ToTable("TaxIdentifier");

            builder.Property(t => t.Number)
                .HasColumnType("char(10)")
                .IsRequired();

            builder.HasIndex(t => t.Number)
                .IsUnique();
        }
    }
}
