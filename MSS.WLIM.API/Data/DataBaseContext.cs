using Microsoft.EntityFrameworkCore;
using WLFSystem.Models;

namespace WLFSystem
{
    public class DataBaseContext:DbContext
    {

        public DataBaseContext(DbContextOptions<DataBaseContext>options):base(options)
        {

        }
        public DbSet<WareHouseItem> WarehouseItems { get; set; }
    }
}
