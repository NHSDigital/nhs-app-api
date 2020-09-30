using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Notification = NHSOnline.Backend.UsersApi.Notifications.Models.Notification;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal class AzureNotificationHubClient : IAzureNotificationHubClient
    {
        private readonly IAzureNotificationHubClientWrapper _hubClientWrapper;
        private const string InstallationTagName = "$InstallationId:";
        private static readonly int InstallationIdGuidLength = Guid.Empty.ToString().Length;
        private const int InstallationRecordMaxResults = 100;

        public AzureNotificationHubClient(IAzureNotificationHubClientWrapper hubClientWrapper)
        {
            _hubClientWrapper = hubClientWrapper;
        }

        public Task CreateOrUpdateInstallation(Installation installation) => _hubClientWrapper.CreateOrUpdateInstallationAsync(installation);

        public Task DeleteInstallation(string installationId) => _hubClientWrapper.DeleteInstallationAsync(installationId);

        public Task<bool> InstallationExists(string installationId) => _hubClientWrapper.InstallationExistsAsync(installationId);

        public async Task<ICollection<string>> FindInstallationIdentifiers(string devicePns)
        {
            var foundRegistrations = await _hubClientWrapper.GetRegistrationsByChannelAsync(devicePns, InstallationRecordMaxResults);
            var existingRegistrations = foundRegistrations as RegistrationDescription[] ?? foundRegistrations.ToArray();

            if (existingRegistrations.Length == 0)
            {
                return Array.Empty<string>();
            }

            return existingRegistrations.SelectMany(rd => rd.Tags)
                .Where(t => t.StartsWith(InstallationTagName, StringComparison.OrdinalIgnoreCase))
                .Select(t => t.Substring(InstallationTagName.Length + 1, InstallationIdGuidLength))
                .Distinct()
                .ToArray();
        }

        public async Task<ICollection<string>> FindInstallationIdentifiersByNhsLoginId(string nhsLoginId)
        {
            var nhsLoginIdTag = NhsLoginTagGenerator.Generate(nhsLoginId);
            var foundRegistrations = await _hubClientWrapper.GetRegistrationsByTagAsync(nhsLoginIdTag, InstallationRecordMaxResults);
            var existingRegistrations = foundRegistrations as RegistrationDescription[] ?? foundRegistrations.ToArray();

            if (existingRegistrations.Length == 0)
            {
                return Array.Empty<string>();
            }

            return existingRegistrations.SelectMany(rd => rd.Tags)
                .Where(t => t.StartsWith(InstallationTagName, StringComparison.OrdinalIgnoreCase))
                .Select(t => t.Substring(InstallationTagName.Length + 1, InstallationIdGuidLength))
                .Distinct()
                .ToArray();
        }

        public async Task SendNotification(string nhsLoginId, Notification notification)
        {
            var nhsLoginIdTag = NhsLoginTagGenerator.Generate(nhsLoginId);
            await _hubClientWrapper.SendTemplateNotificationAsync(notification.ToDictionary(), nhsLoginIdTag);
        }
    }
}