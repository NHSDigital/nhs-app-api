using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INotificationRegistrationService
    {
        //TODO: Switch to access token once NHSO-6147 is merged
        Task<RegistrationResult> Register(RegisterDeviceRequest request, string nhsLoginId);
    }
}