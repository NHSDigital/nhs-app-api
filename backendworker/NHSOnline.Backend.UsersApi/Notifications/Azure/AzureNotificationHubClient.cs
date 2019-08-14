using System.Collections;
using System.Collections.Generic;
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
        
        public Task<string> CreateRegistrationIdAsync() => _hubClient.CreateRegistrationIdAsync();

        public Task<RegistrationDescription> CreateOrUpdateRegistrationAsync(RegistrationDescription registration) => _hubClient.CreateOrUpdateRegistrationAsync(registration);

        public async Task<IEnumerable<RegistrationDescription>> GetRegistrationsByChannelAsync(string devicePns) => await _hubClient.GetRegistrationsByChannelAsync(devicePns, 100);

        public Task DeleteRegistrationAsync(RegistrationDescription registration) => _hubClient.DeleteRegistrationAsync(registration);
    }
}