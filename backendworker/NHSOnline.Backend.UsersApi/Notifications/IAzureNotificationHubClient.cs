using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Notification = NHSOnline.Backend.UsersApi.Notifications.Models.Notification;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface IAzureNotificationHubClient
    {
        Task CreateOrUpdateInstallation(Installation installation);
        Task DeleteInstallation(string installationId);
        Task<bool> InstallationExists(string installationId);
        Task<ICollection<string>> FindInstallationIdentifiers(string devicePns);
        Task<ICollection<string>> FindInstallationIdentifiersByNhsLoginId(string nhsLoginId);
        Task SendNotification(string nhsLoginId, Notification notification);
    }
}