using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

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

        public Task DeleteRegistration(string registrationId) => _hubClientWrapper.DeleteRegistrationAsync(registrationId);

        public Task<bool> InstallationExists(string installationId) => _hubClientWrapper.InstallationExistsAsync(installationId);

        public Task<bool> RegistrationExists(string registrationId) => _hubClientWrapper.RegistrationExistsAsync(registrationId);

        public async Task<List<NotificationRegistrationItem>> FindInstallationIdentifiers(string devicePns)
        {
            var foundRegistrations = await _hubClientWrapper.GetRegistrationsByChannelAsync(devicePns, InstallationRecordMaxResults);
            var existingRegistrations = foundRegistrations as RegistrationDescription[] ?? foundRegistrations.ToArray();

            if (existingRegistrations.Length == 0)
            {
                return new List<NotificationRegistrationItem>();
            }

            var installTags = existingRegistrations.SelectMany(rd => rd.Tags)
                .Where(t => t.StartsWith(InstallationTagName, StringComparison.OrdinalIgnoreCase));
            var installationRegistrations = existingRegistrations.Where(x => x.Tags.Overlaps(installTags));

            var installationItems = installTags
                .Select(t => t.Substring(InstallationTagName.Length + 1, InstallationIdGuidLength))
                .Distinct()
                .Select(x => new NotificationRegistrationItem{ Id = x, Type = NotificationRegistrationItem.RegistrationType.Installation});

            var registrationItems = existingRegistrations.Except(installationRegistrations)
                .Select(x => new NotificationRegistrationItem
                {
                    Id = x.RegistrationId,
                    Type = NotificationRegistrationItem.RegistrationType.Registration
                });

            return installationItems.Union(registrationItems).ToList();
        }
    }
}