using MSS.WLIM.DataServices.Models;

namespace MSS.WLIM.User.API.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAll();
        Task<UserDTO> Get(string id);
        Task<UserDTO> Add(UserDTO user);
        Task<string> UploadFileAsync(UserProfileDTO user);
        Task<UserDTO> Update(UserDTO employee);
        Task<bool> Delete(string id);
    }
}
