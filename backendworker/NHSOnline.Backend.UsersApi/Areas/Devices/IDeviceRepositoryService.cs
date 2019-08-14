using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public interface IDeviceRepositoryService
    {
        Task<DeviceRepositoryResult> Create(
            NotificationRegistrationResult registration,
            RegisterDeviceRequest request);
    }
}