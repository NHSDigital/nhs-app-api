using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INotificationClient
    {
        Task<string> CreateInstallation(InstallationRequest request, string targetPath = null);
        Task DeleteInstallation(string installationId, string nhsLoginId);
        Task DeleteInstallationsByDevicePns(string devicePns);
        Task<bool> InstallationExists(string installationId, string nhsLoginId);
        Task<ICollection<string>> FindInstallationIdsByNhsLoginId(string nhsLoginId);
        Task<(HttpStatusCode, string)> Migrate(MigrationRequest request);
        Task<NotificationResponse> SendNotification(NotificationRequest request);
    }
}