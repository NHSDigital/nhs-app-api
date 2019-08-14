using System.Threading.Tasks;

namespace NHSOnline.Backend.UsersApi.Repository
{
    public interface IUserDeviceRepository
    {
        Task Create(UserDevice userDevice);
    }
}