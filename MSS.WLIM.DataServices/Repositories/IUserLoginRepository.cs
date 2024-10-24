using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS.WLIM.DataServices.Repositories
{
    public interface IUserLoginRepository
    {
        public Task<bool> Validate(string username, string password);
        Task<string> GetUserRole(string emailId); // New method to get the user's role
        Task<string> GetUserName(string emailId);
    }
}
