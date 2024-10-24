using MSS.WLIM.DataServices.Models;
namespace MSS.WLIM.Role.API.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<Roles>> GetAll();
        public Task<Roles> Add(Roles _object);
    }
}
