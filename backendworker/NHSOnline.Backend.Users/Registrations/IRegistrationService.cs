using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Notifications;
using NHSOnline.Backend.Users.Repository;

namespace NHSOnline.Backend.Users.Registrations
{
    public interface IRegistrationService
    {
        Task<RegisterDeviceResult> CreateRegistration(RegisterDeviceRequest request, AccessToken accessToken);
        Task<RegistrationExistsResult> GetRegistration(string devicePns, AccessToken accessToken);
        Task<FindRegistrationsResult> Find(string nhsLoginId);
        Task<DeleteRegistrationResult> DeleteRegistration(string devicePns, AccessToken accessToken);
    }
}