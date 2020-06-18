using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Registrations
{
    public interface IRegistrationService
    {
        Task<RegisterDeviceResult> CreateRegistration(RegisterDeviceRequest request, AccessToken accessToken);
        Task<RegistrationExistsResult> GetRegistration(string devicePns, AccessToken accessToken);
        Task<DeleteRegistrationResult> DeleteRegistration(string devicePns, AccessToken accessToken);
    }
}