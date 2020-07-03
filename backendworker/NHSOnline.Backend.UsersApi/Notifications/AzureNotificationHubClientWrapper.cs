using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal class AzureNotificationHubClientWrapper : IAzureNotificationHubClientWrapper
    {
        private readonly NotificationHubClient _hubClient;

        public AzureNotificationHubClientWrapper(AzureNotificationConfiguration configuration)
        {
            _hubClient = NotificationHubClient.CreateClientFromConnectionString(
                $"{configuration.ConnectionString}{configuration.SharedAccessKey}",
                configuration.NotificationHubPath);
        }

        public Task CreateOrUpdateInstallationAsync(Installation installation) => _hubClient.CreateOrUpdateInstallationAsync(installation);

        public Task DeleteInstallationAsync(string installationId) => _hubClient.DeleteInstallationAsync(installationId);

        public Task<bool> InstallationExistsAsync(string installationId) => _hubClient.InstallationExistsAsync(installationId);

        public Task<bool> RegistrationExistsAsync(string registrationId) => _hubClient.RegistrationExistsAsync(registrationId);

        public Task DeleteRegistrationAsync(string registrationId) => _hubClient.DeleteRegistrationAsync(registrationId);

        public async Task<IEnumerable<RegistrationDescription>> GetRegistrationsByChannelAsync(string devicePns, int installationRecordMaxResults) =>
            await _hubClient.GetRegistrationsByChannelAsync(devicePns, installationRecordMaxResults);
    }
}