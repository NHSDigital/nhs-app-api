using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal interface IAzureNotificationHubClient
    {
        Task CreateOrUpdateInstallation(Installation installation);
        Task DeleteInstallation(string installationId);
        Task<bool> InstallationExists(string installationId);
        Task<List<string>> FindInstallationIdentifiers(string devicePns);
    }
}