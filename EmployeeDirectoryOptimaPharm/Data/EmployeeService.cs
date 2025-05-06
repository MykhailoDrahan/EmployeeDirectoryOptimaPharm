using EmployeeDirectoryOptimaPharm.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDirectoryOptimaPharm.Data
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetEmployeesAsync();
        Task<List<Position>> GetPositionsAsync();
        Task<List<Person>> GetPeopleAsync();
        Task AddEmplyoeeAsync(Employee employee);
        Task AddEmployeesAsync(List<Employee> employees);
        Task TerminateEmployeeAsync(Employee employee);
        Task TerminatePersonAsync(Person person, DateTime endDate);
    }
    public class EmployeeService : BaseService, IEmployeeService
    {
        public EmployeeService(AppDbContext dbContext) : base(dbContext) { }
        
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _dbContext.Employees
                .Include(e => e.Position)
                .Include(e => e.Person)
                    .ThenInclude(p => p.PersonIdentityDocument)
                        .ThenInclude(doc => doc.IDCard)
                .Include(e => e.Person)
                    .ThenInclude(p => p.PersonIdentityDocument)
                        .ThenInclude(doc => doc.Passport)
                .Include(e => e.Person)
                    .ThenInclude(p => p.TaxIdentifier)
                .OrderBy(e => e.Person.LastName)
                    .ThenBy(e => e.Person.FirstName)
                        .ThenBy(e => e.Person.MiddleName)
                .ToListAsync();
        }
        
        public async Task<List<Position>> GetPositionsAsync()
        {
            return await _dbContext.Positions.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<List<Person>> GetPeopleAsync()
        {
            return await _dbContext.People
                .Include(p => p.TaxIdentifier)
                .Include(p=> p.PersonIdentityDocument)
                    .ThenInclude(doc => doc.Passport)
                .Include(p => p.PersonIdentityDocument)
                    .ThenInclude(doc => doc.IDCard)
                .ToListAsync();
        }

        public async Task AddEmplyoeeAsync(Employee employee)
        {
            await _dbContext.AddAsync(employee);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddEmployeesAsync(List<Employee> employees)
        {
            await _dbContext.AddRangeAsync(employees);
            await _dbContext.SaveChangesAsync();
        }

        public async Task TerminateEmployeeAsync(Employee employee)
        {
            _dbContext.Employees.Update(employee);
            await _dbContext.SaveChangesAsync();
        }

        public async Task TerminatePersonAsync(Person person, DateTime endDate)
        {
            var employeesWithSamePerson = await _dbContext.Employees
                .Where(employee => employee.PersonId == person.Id)
                .ToListAsync();

            foreach (var employee in employeesWithSamePerson)
            {
                employee.EndDate = endDate;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
