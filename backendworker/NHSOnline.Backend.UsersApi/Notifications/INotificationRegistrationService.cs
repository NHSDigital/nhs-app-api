using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INotificationRegistrationService
    {
        Task<RegistrationResult> Register(string devicePns, DeviceType deviceType, string nhsLoginId);
        Task<RegistrationExistsResult> Exists(UserDevice userDevice);
        Task<FindRegistrationsResult> Find(string nhsLoginId);
        Task<DeleteRegistrationResult> Delete(string id);
    }
}