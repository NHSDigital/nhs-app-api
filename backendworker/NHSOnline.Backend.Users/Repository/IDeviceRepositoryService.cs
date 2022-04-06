using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Notifications;

namespace NHSOnline.Backend.Users.Repository
{
    public interface IDeviceRepositoryService
    {
        Task<RegisterDeviceResult> Create(
            NotificationRegistrationResult registration,
            RegisterDeviceRequest request,
            AccessToken accessToken);

        Task<SearchDeviceResult> Find(string devicePns, AccessToken accessToken);

        Task<DeleteDeviceResult> Delete(string deviceId, string nhsLoginId);
    }
}