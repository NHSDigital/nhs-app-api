using System.Threading.Tasks;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal interface IAzureNotificationHubWrapper
    {
        bool CanReadFor(string nhsLoginId);
        bool CanWriteFor(string nhsLoginId);
        int Generation { get; }
        string Path { get; }

        Task<string> CreateInstallation(InstallationRequest request);
        Task DeleteInstallation(string installationId);
        Task<bool> InstallationExists(string installationId);
        Task<string[]> GetInstallationIdsByNhsLoginId(string nhsLoginId);
        Task<string> SendNotification(NotificationRequest request);
        Task<string> SendScheduledNotification(NotificationRequest request);
        Task<NotificationOutcomeResponse> GetNotificationOutcomeDetails(string notificationId);
    }
}