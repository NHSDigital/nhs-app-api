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
        Task<(HttpStatusCode, string)> Migrate(MigrationRequest request);
        Task<NotificationSendResponse> SendNotification(NotificationRequest request);
        Task<NotificationOutcomeResponse> GetNotificationOutcomeDetails(string notificationId, string hubPath);
    }
}