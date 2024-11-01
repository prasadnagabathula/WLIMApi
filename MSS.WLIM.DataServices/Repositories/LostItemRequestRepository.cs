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
    public class LostItemRequestRepository : IRepository<LostItemRequests>
    {
        private readonly DataBaseContext _context;

        public LostItemRequestRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LostItemRequests>> GetAll()
        {
            return await _context.WHTblLostItemRequest.ToListAsync();
        }

        public async Task<LostItemRequests> Get(string id)
        {
            return await _context.WHTblLostItemRequest.FindAsync(id);
        }

        public async Task<LostItemRequests> Create(LostItemRequests _object)
        {
            _context.WHTblLostItemRequest.Add(_object);
            await _context.SaveChangesAsync();
            return _object;
        }

        public async Task<LostItemRequests> Update(LostItemRequests _object)
        {
            _context.Entry(_object).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return _object;
        }

        public async Task<bool> Delete(string id)
        {
            var data = await _context.WHTblLostItemRequest.FindAsync(id);
            if (data == null)
            {
                return false;
            }

            _context.WHTblLostItemRequest.Remove(data);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
