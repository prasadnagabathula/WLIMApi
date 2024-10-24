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
    public class DepartmentRepository : IRepository<Departments>
    {
        private readonly DataBaseContext _context;

        public DepartmentRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Departments>> GetAll()
        {
            return await _context.WHTblDepartment.ToListAsync();
        }

        public async Task<Departments> Get(string id)
        {
            return await _context.WHTblDepartment.FindAsync(id);
        }

        public async Task<Departments> Create(Departments _object)
        {
            _context.WHTblDepartment.Add(_object);
            await _context.SaveChangesAsync();
            return _object;
        }

        public async Task<Departments> Update(Departments _object)
        {
            _context.Entry(_object).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return _object;
        }

        public async Task<bool> Delete(string id)
        {
            var data = await _context.WHTblDepartment.FindAsync(id);
            if (data == null)
            {
                return false;
            }

            _context.WHTblDepartment.Remove(data);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
