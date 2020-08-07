using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Repository
{
    public interface IDeviceRepositoryService
    {
        Task<RegisterDeviceResult> Create(
            NotificationRegistrationResult registration,
            RegisterDeviceRequest request,
            AccessToken accessToken);

        Task<SearchDeviceResult> Find(string devicePns, AccessToken accessToken);
        Task<SearchDeviceResult> FindRegistrations(int maxRecords);
        Task<DeleteDeviceResult> Delete(string deviceId, string nhsLoginId);
        Task<UpdateDeviceResult> Update(string deviceId, string nhsLoginId, string registrationId);
    }
}