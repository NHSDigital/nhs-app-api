using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public interface IDeviceRepositoryService
    {
        Task<DeviceRegistrationResult> Create(
            NotificationRegistrationResult registration,
            RegisterDeviceRequest request,
            AccessToken accessToken);

        Task<SearchDeviceResult> Find(string devicePns, AccessToken accessToken);
        Task<DeleteDeviceResult> Delete(string deviceId, AccessToken accessToken);
    }
}