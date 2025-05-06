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
    public class IDCardConfiguration : BaseEntityConfiguration<IDCard>
    {
        public override void Configure(EntityTypeBuilder<IDCard> builder)
        {
            base.Configure(builder);

            builder.ToTable("IDCards");

            builder.Property(i => i.Number)
                .HasColumnType("char(9)")
                .IsRequired();

            builder.HasIndex(i => i.Number)
                .IsUnique();
        }
    }
}
