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
    public class PersonConfiguration : BaseEntityConfiguration<Person>
    {
        public override void Configure(EntityTypeBuilder<Person> builder)
        {
            base.Configure(builder);

            builder.ToTable("People");

            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.MiddleName)
                .HasMaxLength(100);

            builder.HasMany(p => p.Employees)
                .WithOne(e => e.Person)
                .HasForeignKey(e => e.PersonId);

            builder.HasOne(p => p.TaxIdentifier)
                .WithOne()
                .HasForeignKey<Person>(p => p.TaxIdentifierId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(p => p.FullName);
        }
    }
}
