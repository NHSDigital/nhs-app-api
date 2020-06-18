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

        public Task<bool> InstallationExists(string installationId) => _hubClientWrapper.InstallationExistsAsync(installationId);

        public async Task<List<string>> FindInstallationIdentifiers(string devicePns)
        {
            var foundRegistrations = await _hubClientWrapper.GetRegistrationsByChannelAsync(devicePns, InstallationRecordMaxResults);

            if (foundRegistrations == null)
            {
                return new List<string>();
            }

            return foundRegistrations.SelectMany(x => x.Tags)
                .Where(t => t.StartsWith(InstallationTagName, StringComparison.OrdinalIgnoreCase))
                .Select(t => t.Substring(InstallationTagName.Length + 1, InstallationIdGuidLength))
                .Distinct()
                .ToList();
        }
    }
}