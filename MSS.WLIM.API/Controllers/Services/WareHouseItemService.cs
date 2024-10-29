using Emgu.CV.Ocl;
using Microsoft.EntityFrameworkCore;
using WLFSystem.Models;
namespace WLFSystem.Controllers.Services
 
{
    public class WareHouseItemService : IWareHouseItemService
    {
        private readonly DataBaseContext _context;
        public async Task<WareHouseItem> Add(WareHouseItem item)
        {
            // Add the new item to the DbSet
            await _context.TblWareHouseItem.AddAsync(item);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return item;
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<WareHouseItem> Get(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WareHouseItem>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<WareHouseItem> Update(WareHouseItem employee)
        {
            throw new NotImplementedException();
        }
    }
}
