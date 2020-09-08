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
        Task DeleteRegistration(string registrationId);
        Task<bool> InstallationExists(string installationId);
        Task<bool> RegistrationExists(string registrationId);
        Task<ICollection<NotificationRegistrationItem>> FindInstallationIdentifiers(string devicePns);
        Task<ICollection<string>> FindInstallationIdentifiersByNhsLoginId(string nhsLoginId);
        Task SendNotification(string nhsLoginId, Notification notification);
    }
}