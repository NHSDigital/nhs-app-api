using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure
{
    public interface IAzureNotificationHubClient
    {
        Task<string> CreateRegistrationId();
        Task<RegistrationDescription> CreateOrUpdateRegistration(RegistrationDescription registration);
        Task DeleteRegistration(string registrationId);
        Task DeleteAllRegistrations(string devicePns);
        Task<bool> RegistrationExists(string registrationId, string devicePns);
    }
}