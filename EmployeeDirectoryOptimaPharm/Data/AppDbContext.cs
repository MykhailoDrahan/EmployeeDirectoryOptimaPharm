using EmployeeDirectoryOptimaPharm.Configurations;
using EmployeeDirectoryOptimaPharm.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDirectoryOptimaPharm.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<IDCard> IDCards { get; set; }
        public DbSet<Passport> Passports { get; set; }
        public DbSet<PersonIdentityDocument> PersonIdentityDocuments { get; set; }
        public DbSet<TaxIdentifier> TaxIdentifiers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new PositonConfiguration());
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new IDCardConfiguration());
            modelBuilder.ApplyConfiguration(new PassportConfiguration());
            modelBuilder.ApplyConfiguration(new PersonIdentityDocumentConfiguration());
            modelBuilder.ApplyConfiguration(new TaxIdentifierConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
