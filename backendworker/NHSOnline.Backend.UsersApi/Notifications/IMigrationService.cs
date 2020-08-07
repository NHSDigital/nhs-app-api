using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface IMigrationService
    {
        Task<RegistrationResult> Register(string devicePns, DeviceType deviceType, string nhsLoginId);
        Task<DeleteRegistrationResult> Delete(string id);
    }
}
