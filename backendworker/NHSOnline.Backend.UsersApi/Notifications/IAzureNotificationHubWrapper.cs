using System.Threading.Tasks;
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
        Task<NotificationResponse> SendNotification(NotificationRequest request);
        Task<NotificationResponse> SendScheduledNotification(NotificationRequest request);
    }
}