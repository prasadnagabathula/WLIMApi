using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSS.WLIM.DataServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MSS.WLIM.DataServices.Repositories
{
    public class UserLoginRepository : IUserLoginRepository
    {
        private readonly DataBaseContext _dbContext;
        private readonly ILogger _logger;

        public UserLoginRepository(DataBaseContext dbContext, ILogger<UserLoginRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<string> GetUserRole(string emailId)
        {
            // Assuming you have a relationship between Employee and Role tables
            var user = await _dbContext.WHTblUser.Include(e => e.Roles)
                                                     .FirstOrDefaultAsync(e => e.EmailId == emailId);
            return user?.Roles?.RoleName; // Return the role name
        }
        public async Task<string> GetUserName(string emailId)
        {
            // Assuming TblEmployee has a property "Name" to store employee's name
            var user = await _dbContext.WHTblUser
                .FirstOrDefaultAsync(e => e.EmailId == emailId);

            return user?.Name; // Return the employee's name
        }

        public async Task<bool> Validate(string emailId, string password)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailId) && !string.IsNullOrEmpty(password))
                {
                    // Get the user by email ID
                    var user = await _dbContext.WHTblUser
                        .FirstOrDefaultAsync(u => u.EmailId == emailId);

                    if (user == null)
                    {
                        return false; // No such user
                    }

                    // Hash the input password using the same method as when storing it
                    var hashedPassword = HashPassword(password);

                    // Compare the hashed password with the stored hashed password
                    if (user.Password == hashedPassword)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
