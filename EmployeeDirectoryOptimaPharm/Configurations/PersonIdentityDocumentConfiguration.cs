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
    public class PersonIdentityDocumentConfiguration : BaseEntityConfiguration<PersonIdentityDocument>
    {
        public override void Configure(EntityTypeBuilder<PersonIdentityDocument> builder)
        {
            base.Configure(builder);

            builder.ToTable("PersonIdentityDocuments");

            builder.Property(doc => doc.Type)
                .IsRequired();

            builder.HasOne(doc => doc.Person)
                .WithOne(p => p.PersonIdentityDocument)
                .HasForeignKey<PersonIdentityDocument>(doc => doc.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(doc => doc.IDCard)
                .WithOne()
                .HasForeignKey<PersonIdentityDocument>(doc => doc.IDCardId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(doc => doc.Passport)
                .WithOne()
                .HasForeignKey<PersonIdentityDocument>(doc => doc.PassportId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Ignore(doc => doc.DocumentNumber);

            builder.Ignore(doc => doc.DocumentName);
        }
    }
}
