using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal interface IAzureNotificationHubClientWrapper
    {
        Task CreateOrUpdateInstallationAsync(Installation installation);
        Task DeleteInstallationAsync(string installationId);
        Task<bool> InstallationExistsAsync(string installationId);
        Task<IEnumerable<RegistrationDescription>> GetRegistrationsByChannelAsync(string devicePns,
            int installationRecordMaxResults);
    }
}