using BCrypt.Net;
using EmployeeDirectoryOptimaPharm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDirectoryOptimaPharm.Data
{
    public interface IUserService
    {
        Task<bool> IsAnyAsync();
        Task<bool> UsernameExist(string username);
        Task<bool> RegisterAsync(string username, string password);
        Task<bool> LoginAsync(string username, string password);
    }
    public class UserService : BaseService, IUserService
    {
        public UserService(AppDbContext dbContext) : base(dbContext) { }

        public async Task<bool> IsAnyAsync()
        {
            return await _dbContext.Users.AnyAsync();
        }

        public async Task<bool> UsernameExist(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username.ToUpper()) != null;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            if(await UsernameExist(username))
            {
                return false;
            }
            else
            {
                string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 15);
                var user = new User() { Username = username.ToUpper(), Password = passwordHash };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }
        public async Task<bool> LoginAsync(string username, string password)
        {
            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username.ToUpper());
            if (user != null)
            {
                return BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password);
            }
            else
            {
                return false;
            }
        }
    }
}
