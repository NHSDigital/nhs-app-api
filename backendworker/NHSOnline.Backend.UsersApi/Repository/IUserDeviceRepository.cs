using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Repository
{
    public interface IUserDeviceRepository
    {
        Task Create(UserDevice userDevice, RegisterDeviceRequest request);
    }
}