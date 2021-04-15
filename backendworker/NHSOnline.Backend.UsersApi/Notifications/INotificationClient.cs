using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Notifications.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INotificationClient
    {
        Task<string> CreateInstallation(InstallationRequest request);
        Task DeleteInstallation(string installationId, string nhsLoginId);
        Task DeleteInstallationsByDevicePns(string devicePns);
        Task<bool> InstallationExists(string installationId, string nhsLoginId);
        Task<ICollection<string>> FindInstallationIdsByNhsLoginId(string nhsLoginId);
        Task SendNotification(NotificationRequest request);
    }
}