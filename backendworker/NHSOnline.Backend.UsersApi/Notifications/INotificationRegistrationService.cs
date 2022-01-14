using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Notifications.Models;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INotificationRegistrationService
    {
        Task<RegistrationResult> Register(InstallationRequest request);
        Task<RegistrationExistsResult> Exists(UserDevice userDevice);
        Task<FindRegistrationsResult> Find(string nhsLoginId);
        Task<DeleteRegistrationResult> Delete(string id, string nhsLoginId);
    }
}