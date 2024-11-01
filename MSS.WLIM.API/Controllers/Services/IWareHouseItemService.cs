using WLFSystem.Models;

namespace WLFSystem.Controllers.Services
{
    public interface IWareHouseItemService
    {
        Task<IEnumerable<WareHouseItem>> GetAll();
        Task<WareHouseItem> Get(string id);
        Task<WareHouseItem> Add(WareHouseItem item);
        Task<WareHouseItem> Update(WareHouseItem item);
        Task<bool> Delete(string id);
    }
}
