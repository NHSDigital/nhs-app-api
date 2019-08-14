using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure
{
    public interface IAzureNotificationHubClient
    {
        Task<string> CreateRegistrationIdAsync();
        Task<RegistrationDescription> CreateOrUpdateRegistrationAsync(RegistrationDescription registration);
        Task<IEnumerable<RegistrationDescription>> GetRegistrationsByChannelAsync(string devicePns);
        Task DeleteRegistrationAsync(RegistrationDescription registration);
    }
}