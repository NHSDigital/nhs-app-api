using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal interface IAzureNotificationHubClient
    {
        Task CreateOrUpdateInstallation(Installation installation);
        Task DeleteInstallation(string installationId);
        Task DeleteRegistration(string registrationId);
        Task<bool> InstallationExists(string installationId);
        Task<bool> RegistrationExists(string registrationId);
        Task<List<NotificationRegistrationItem>> FindInstallationIdentifiers(string devicePns);
    }
}