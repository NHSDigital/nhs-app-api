using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Notifications.Models;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INotificationRegistrationService
    {
        Task<RegistrationResult> Register(InstallationRequest request);
        Task<DeleteRegistrationResult> Delete(string id, string nhsLoginId);
    }
}