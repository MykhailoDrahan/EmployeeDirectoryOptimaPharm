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
    public class EmployeeConfiguration : BaseEntityConfiguration<Employee>
    {
        public override void Configure(EntityTypeBuilder<Employee> builder)
        {
            base.Configure(builder);

            builder.ToTable("Employees");

            builder.HasOne(e => e.Person)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Position)
                .WithMany()
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(e => e.Salary)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

            builder.Property(e => e.EmploymentRate)
                .HasPrecision(4, 2)
                .IsRequired();

            builder.Property(e => e.StartDate)
                .HasColumnType("date")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            builder.Property(e => e.EndDate)
                .HasColumnType("date")
                .IsRequired(false);

            builder.Ignore(e => e.IsFinished);
        }
    }
}
