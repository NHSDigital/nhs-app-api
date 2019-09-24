using System.Threading.Tasks;

namespace NHSOnline.Backend.UsersApi.Repository
{
    public interface IUserDeviceRepository
    {
        Task Create(UserDevice userDevice);
        Task<UserDevice> Find(string nhsLoginId, string deviceId);
        Task Delete(string nhsLoginId, string deviceId);
    }
}