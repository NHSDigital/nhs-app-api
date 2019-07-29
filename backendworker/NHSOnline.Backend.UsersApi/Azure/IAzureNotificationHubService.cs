using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Azure
{
    public interface IAzureNotificationHubService
    {
        Task<RegistrationResult> Register(RegisterDeviceRequest request);
    }
}