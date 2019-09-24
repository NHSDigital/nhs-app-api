using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure
{
    public class AzureNotificationHubClient : IAzureNotificationHubClient
    {
        private readonly NotificationHubClient _hubClient;

        public AzureNotificationHubClient(AzureNotificationConfiguration configuration)
        {
            _hubClient = NotificationHubClient.CreateClientFromConnectionString(
                $"{configuration.ConnectionString}{configuration.SharedAccessKey}",
                configuration.NotificationHubPath);
        }

        public Task<string> CreateRegistrationId() => _hubClient.CreateRegistrationIdAsync();

        public Task<RegistrationDescription> CreateOrUpdateRegistration(RegistrationDescription registration) =>
            _hubClient.CreateOrUpdateRegistrationAsync(registration);

        public Task DeleteRegistration(string registrationId) => _hubClient.DeleteRegistrationAsync(registrationId);

        public async Task DeleteAllRegistrations(string devicePns)
        {
            var registrations = await GetRegistrationsByChannel(devicePns);

            foreach (RegistrationDescription registration in registrations)
            {
                await DeleteRegistration(registration);
            }
        }

        public async Task<bool> RegistrationExists(string registrationId, string devicePns)
        {
            var registrations = await GetRegistrationsByChannel(devicePns);
            return registrations.Any(r => string.CompareOrdinal(r.RegistrationId, registrationId) == 0);
        }

        private async Task DeleteRegistration(RegistrationDescription registration) =>
            await _hubClient.DeleteRegistrationAsync(registration);

        private async Task<IEnumerable<RegistrationDescription>> GetRegistrationsByChannel(string devicePns) =>
            await _hubClient.GetRegistrationsByChannelAsync(devicePns, 100);
    }
}