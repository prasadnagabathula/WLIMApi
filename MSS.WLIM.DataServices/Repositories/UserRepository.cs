using Microsoft.EntityFrameworkCore;
using MSS.WLIM.DataServices.Data;
using MSS.WLIM.DataServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS.WLIM.DataServices.Repositories
{
    public class UserRepository : IRepository<Users>
    {
        private readonly DataBaseContext _context;

        public UserRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Users>> GetAll()
        {
            return await _context.WHTblUser.ToListAsync();
        }

        public async Task<Users> Get(string id)
        {
            return await _context.WHTblUser.FindAsync(id);
        }

        public async Task<Users> Create(Users users)
        {
            _context.WHTblUser.Add(users);
            await _context.SaveChangesAsync();
            return users;
        }

        public async Task<Users> Update(Users users)
        {
            _context.Entry(users).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return users;
        }

        public async Task<bool> Delete(string id)
        {
            var users = await _context.WHTblUser.FindAsync(id);
            if (users == null)
            {
                return false;
            }

            _context.WHTblUser.Remove(users);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
