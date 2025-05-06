using EmployeeDirectoryOptimaPharm.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDirectoryOptimaPharm.Data
{
    public interface IDBFulfillService
    {
        void FullfilDB();
    }
    public class DBFulfillService : BaseService, IDBFulfillService
    {
        public DBFulfillService(AppDbContext dbContext) : base(dbContext) { }

        public void FullfilDB()
        {
            int n = 100;
            List<PersonIdentityDocument> personIdentityDocuments = CreateDocuments(n);
            List<TaxIdentifier> taxIdentifiers = CreateTaxIdentifiers(n);
            List<Person> people = CreatePeople(personIdentityDocuments, taxIdentifiers);
            List<Position> positions = CreatePositions();
            List<Employee> employees = CreateEmployees(people, positions);
            _dbContext.AddRange(employees);
            _dbContext.SaveChanges();
        }

        private List<PersonIdentityDocument> CreateDocuments(int n)
        {
            Random random = new Random();
            var identityDocumentTypeLength = Enum.GetValues(typeof(IdentityDocumentType)).Length;
            List<PersonIdentityDocument> personIdentityDocuments = new List<PersonIdentityDocument>();

            for (int i = 0; i < n; i++) 
            {
                PersonIdentityDocument personIdentityDocument = new PersonIdentityDocument();
                IdentityDocumentType identityDocumentType = (IdentityDocumentType)random.Next(identityDocumentTypeLength);
                personIdentityDocument.Type = identityDocumentType;
                switch (identityDocumentType)
                {
                    case IdentityDocumentType.IDCard:
                        {
                            IDCard iDCard = new IDCard() { Number = GenerateNumber(9) };
                            personIdentityDocument.IDCard = iDCard;
                        }
                        break;
                    case IdentityDocumentType.Passport:
                        {
                            Passport passport = new Passport() { Series = GenerateSerice(2), Number = GenerateNumber(6) };
                            personIdentityDocument.Passport = passport;
                        }
                        break;
                }
                personIdentityDocuments.Add(personIdentityDocument);
            }
            return personIdentityDocuments;
        }
        private List<TaxIdentifier> CreateTaxIdentifiers(int n)
        {
            Random random = new Random();
            List<TaxIdentifier> taxIdentifiers = new List<TaxIdentifier>();
            for (int i = 0; i < n; i++)
            {
                TaxIdentifier taxIdentifier = new TaxIdentifier() { Number = GenerateNumber(10) };
                taxIdentifiers.Add(taxIdentifier);
            }
            return taxIdentifiers;
        }

        private List<Position> CreatePositions()
        {
            Random random = new Random();
            List<string> positionNames = new List<string> {"Pharmacist", "Regulatory Affairs Specialist", "Quality Assurance Manager", "Clinical Research Associate", "Laboratory Technician", "IT Support Specialist", "Software Developer", "Data Analyst", "System Administrator", "Product Manager"};
            List<Position> positions = positionNames.Select(name => new Position { Name = name }).ToList();
            return positions;
        }
        private List<Person> CreatePeople(List<PersonIdentityDocument> documents, List<TaxIdentifier> taxIdentifiers)
        {
            Random random = new Random();
            List<string> lastNames = new List<string> { "Shevchenko", "Kovalenko", "Melnyk", "Marchenko", "Boyko", "Dubovyk", "Sydorenko", "Tkachenko", "Kravchenko", "Bondarenko" };
            List<string> firstNamesMale = new List<string> { "Oleksandr", "Andrii", "Volodymyr", "Yurii", "Maksym", "Dmytro", "Serhii", "Viktor", "Oleh", "Petro" };
            List<string> middleNamesMale = new List<string> { "Ivanovych", "Mykolaovych", "Petrovych", "Oleksiiovych", "Serhiiovych", "Anatoliiovych", "Vasylovych", "Leonidovych", "Pavlovych", "Yuriiovych" };
            List<string> firstNamesFemale = new List<string> { "Iryna", "Kateryna", "Svitlana", "Natalia", "Oksana", "Olena", "Halyna", "Tetiana", "Liliia", "Mariia" };
            List<string> middleNamesFemale = new List<string> { "Ivanivna", "Mykolaivna", "Petrivna", "Oleksiivna", "Serhiiivna", "Anatoliivna", "Vasylivna", "Leonidivna", "Pavlivna", "Yuriiivna" };
            List<Person> people = documents.Zip(taxIdentifiers, (d, t) =>
            {
                int gender = random.Next(2);
                return new Person
                {
                    PersonIdentityDocument = d,
                    TaxIdentifier = t,
                    LastName = lastNames[random.Next(lastNames.Count)],
                    FirstName = gender == 0 ? firstNamesFemale[random.Next(firstNamesFemale.Count)] : firstNamesMale[random.Next(firstNamesMale.Count)],
                    MiddleName = random.Next(7) == 0 ? "" : gender == 0 ? middleNamesFemale[random.Next(middleNamesFemale.Count)] : middleNamesMale[random.Next(middleNamesMale.Count)]
                };
            }).ToList();
            return people;
        }

        private List<Employee> CreateEmployees(List<Person> people, List<Position> positions)
        {
            Random random = new Random();
            DateTime dateRangeStart = DateTime.Today.AddYears(-1);
            DateTime dateRangeEnd = DateTime.Today.AddMonths(1);
            int dateRange = (dateRangeEnd - dateRangeStart).Days;
            List<Employee> employees = people.SelectMany(person =>
            {
                var severalPositions = random.Next(5) == 0
                    ? positions.OrderBy(_ => random.Next()).Take(random.Next(positions.Count / 2)) 
                    : positions.OrderBy(_ => random.Next()).Take(1);
                return severalPositions.Select(p =>
                {
                    DateTime startDate = dateRangeStart.AddDays(random.Next(dateRange + 1));
                    return new Employee
                    {
                        Person = person,
                        Position = p,
                        Salary = random.Next(1, 1001) * 100,
                        EmploymentRate = random.Next(4) == 0 ? random.Next(1, 21) * 0.25 : 1,
                        StartDate = startDate,
                        EndDate = random.Next(3) == 0 ? startDate.AddMonths(random.Next(5)) : null
                    };
                });
            }).ToList();
            return employees;
        }
        private string GenerateSerice(int n = 2)
        {
            Random random = new Random();
            StringBuilder builder = new StringBuilder(n);
            for (int i = 0; i < n; i++)
            {
                builder.Append((char)random.Next('A', 'Z'+1));
            }
            return builder.ToString();
        }

        private string GenerateNumber(int n)
        {
            Random random = new Random();
            StringBuilder builder = new StringBuilder(n);
            for (int i = 0; i < n; i++)
            {
                builder.Append(random.Next(0, 10));
            }
            return builder.ToString();
        }
    }
}
