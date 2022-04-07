using System.Threading.Tasks;
using NHSOnline.Backend.Users.Notifications.Models;
using NHSOnline.Backend.Users.Repository;

namespace NHSOnline.Backend.Users.Notifications
{
    public interface INotificationRegistrationService
    {
        Task<RegistrationResult> Register(InstallationRequest request);
        Task<DeleteRegistrationResult> Delete(string id, string nhsLoginId);
    }
}