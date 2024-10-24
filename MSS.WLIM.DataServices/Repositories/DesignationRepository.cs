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
    public class DesignationRepository : IRepository<Designations>
    {
        private readonly DataBaseContext _context;

        public DesignationRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Designations>> GetAll()
        {
            return await _context.WHTblDesignation.ToListAsync();
        }

        public async Task<Designations> Get(string id)
        {
            return await _context.WHTblDesignation.FindAsync(id);
        }

        public async Task<Designations> Create(Designations _object)
        {
            _context.WHTblDesignation.Add(_object);
            await _context.SaveChangesAsync();
            return _object;
        }

        public async Task<Designations> Update(Designations _object)
        {
            _context.Entry(_object).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return _object;
        }

        public async Task<bool> Delete(string id)
        {
            var data = await _context.WHTblDesignation.FindAsync(id);
            if (data == null)
            {
                return false;
            }

            _context.WHTblDesignation.Remove(data);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
