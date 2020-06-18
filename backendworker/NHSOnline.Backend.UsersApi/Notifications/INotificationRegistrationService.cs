using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INotificationRegistrationService
    {
        Task<RegistrationResult> Register(string devicePns, DeviceType deviceType, AccessToken accessToken);
        Task<RegistrationExistsResult> Exists(UserDevice userDevice);
        Task<DeleteRegistrationResult> Delete(string id);
    }
}