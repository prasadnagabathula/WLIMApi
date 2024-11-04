using Emgu.CV.Ocl;
using Microsoft.EntityFrameworkCore;
using WLFSystem.Models;
namespace WLFSystem.Controllers.Services
 
{
    public class WareHouseItemService : IWareHouseItemService
    {
        private readonly DataBaseContext _context;


        public WareHouseItemService(DataBaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<WareHouseItem> Add(WareHouseItem item)
        {
            // Add the new item to the DbSet
            await _context.WarehouseItems.AddAsync(item);


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


        public async Task<IEnumerable<WareHouseItem>> GetAll()
        {
            var identifiedItem = await _context.WarehouseItems.ToListAsync();

            var warehouseItemDto = new List<WareHouseItem>();

            foreach (var d in identifiedItem)
            {
                warehouseItemDto.Add(new WareHouseItem
                {
                    Id = d.Id,
                    Category = d.Category,
                    Tags = d.Tags,
                    ItemDescription = d.ItemDescription,
                    WarehouseLocation = d.WarehouseLocation,
                    Comments = d.Comments,
                    CreatedBy = d.CreatedBy,
                    CreatedDate = d.CreatedDate,
                    UpdatedBy = d.UpdatedBy,

                    UpdatedDate = d.UpdatedDate,
                    FilePath= d.FilePath

                });
            }

            return warehouseItemDto;

        }

        public Task<WareHouseItem> Update(WareHouseItem employee)
        {
            throw new NotImplementedException();
        }
    }
}
