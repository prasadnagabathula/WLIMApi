using MSS.WLIM.DataServices.Models;

namespace MSS.WLIM.LostItemRequest.API.Services
{
    public interface ILostItemRequestService
    {
        public Task<IEnumerable<LostItemRequests>> GetAll();
        public Task<LostItemRequests> Get(string id);
        public Task<LostItemRequests> Add(LostItemRequests _object);
        public Task<LostItemRequests> Update(LostItemRequests _object);
        public Task<bool> Delete(string id);
    }
}
