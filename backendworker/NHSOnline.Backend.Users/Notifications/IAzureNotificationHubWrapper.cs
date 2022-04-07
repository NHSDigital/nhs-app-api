using System.Threading.Tasks;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Notifications.Models;

namespace NHSOnline.Backend.Users.Notifications
{
    public interface IAzureNotificationHubWrapper
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