using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal interface IAzureNotificationHubClientWrapper
    {
        Task CreateOrUpdateInstallationAsync(Installation installation);
        Task DeleteInstallationAsync(string installationId);
        Task DeleteRegistrationAsync(string registrationId);
        Task<bool> InstallationExistsAsync(string installationId);
        Task<bool> RegistrationExistsAsync(string registrationId);
        Task<IEnumerable<RegistrationDescription>> GetRegistrationsByChannelAsync(string devicePns,
            int installationRecordMaxResults);
        Task<IEnumerable<RegistrationDescription>> GetRegistrationsByTagAsync(string tag, int maxRecords);
        Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties,
            string tagExpression);
    }
}