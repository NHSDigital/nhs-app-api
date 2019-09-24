using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INotificationService
    {
        Task<RegistrationResult> Register(RegisterDeviceRequest registerDeviceRequest, AccessToken accessToken);
        Task<RegistrationExistsResult> Exists(UserDevice userDevice);
        Task<DeleteRegistrationResult> Delete(string registrationId);
    }
}