using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Notifications.Extensions;
using NHSOnline.Backend.Users.Notifications.Models;

namespace NHSOnline.Backend.Users.Notifications
{
    public class AzureNotificationHubWrapper : IAzureNotificationHubWrapper
    {
        private const int InstallationRecordMaxResults = 100;

        private readonly string _description;
        private readonly INotificationHubClient _hubClient;
        private readonly IInstallationFactory _installationFactory;
        private readonly INhsLoginIdService _readService;
        private readonly INhsLoginIdService _writeService;

        public AzureNotificationHubWrapper(
            AzureNotificationHubConfiguration hubConfiguration,
            NotificationsConfiguration notificationsConfiguration,
            INotificationHubClientFactory notificationHubClientFactory)
        {
            _hubClient = notificationHubClientFactory.CreateClientFromConnectionString(
                $"{hubConfiguration.ConnectionString}{hubConfiguration.SharedAccessKey}",
                hubConfiguration.NotificationHubPath
            );

            _installationFactory = new InstallationFactory(
                new InstallationTemplateFactory(), notificationsConfiguration.NotificationInstallationExpiryMonths);
            _readService = new NhsLoginIdService(hubConfiguration.ReadCharacters);
            _writeService = new NhsLoginIdService(hubConfiguration.WriteCharacters);

            _description = $"{hubConfiguration.NotificationHubPath} "
                           + $"(Gen {hubConfiguration.Generation}: "
                           + $"Reads [{hubConfiguration.ReadCharacters.ToUpperInvariant()}], "
                           + $"Writes [{hubConfiguration.WriteCharacters.ToUpperInvariant()}])";

            Generation = hubConfiguration.Generation;
            Path = hubConfiguration.NotificationHubPath;
        }

        public int Generation { get; }
        public string Path { get; }

        public bool CanReadFor(string nhsLoginId)
        {
            return _readService.HandlesNhsLoginId(nhsLoginId);
        }

        public bool CanWriteFor(string nhsLoginId)
        {
            return _writeService.HandlesNhsLoginId(nhsLoginId);
        }

        public async Task<string> CreateInstallation(InstallationRequest request)
        {
            var installation = _installationFactory.Create(request);

            await _hubClient.CreateOrUpdateInstallationAsync(installation);

            return installation.InstallationId;
        }

        public Task DeleteInstallation(string installationId)
        {
            return _hubClient.DeleteInstallationAsync(installationId);
        }

        public Task<bool> InstallationExists(string installationId)
        {
            return _hubClient.InstallationExistsAsync(installationId);
        }

        public async Task<string[]> GetInstallationIdsByNhsLoginId(string nhsLoginId)
        {
            var tag = NhsLoginTagGenerator.Generate(nhsLoginId);

            return (await _hubClient.GetRegistrationsByTagAsync(tag, InstallationRecordMaxResults)).InstallationIds();
        }

        public async Task<string> SendNotification(NotificationRequest request)
        {
            var properties = request.ToDictionary();
            var tag = NhsLoginTagGenerator.Generate(request.NhsLoginId);

            var notificationOutcome = await _hubClient.SendTemplateNotificationAsync(properties, tag);

            return notificationOutcome.NotificationId;
        }

        public async Task<string> SendScheduledNotification(NotificationRequest request)
        {
            var properties = request.ToDictionary();
            var tag = NhsLoginTagGenerator.Generate(request.NhsLoginId);

            var notification = new TemplateNotification(properties);

            Debug.Assert(request.ScheduledTime != null, "request.ScheduledTime != null");
            var notificationOutcome =
                await _hubClient.ScheduleNotificationAsync(notification, request.ScheduledTime.Value, tag);

            return notificationOutcome.ScheduledNotificationId;
        }

        public async Task<NotificationOutcomeResponse> GetNotificationOutcomeDetails(string notificationId)
        {
            var notificationOutcome = await _hubClient.GetNotificationOutcomeDetailsAsync(notificationId);

            var iosPushNotificationOutcomeCounts =
                notificationOutcome.ApnsOutcomeCounts?.Select(keyValuePair => new PlatformOutcome
                {
                    Outcome = keyValuePair.Key,
                    Count = keyValuePair.Value,
                    Platform = Constants.SupportedDeviceNames.iOS
                }) ??
                new List<PlatformOutcome>();

            var androidPushNotificationOutcomeCounts =
                notificationOutcome.FcmOutcomeCounts?.Select(keyValuePair => new PlatformOutcome
                {
                    Outcome = keyValuePair.Key,
                    Count = keyValuePair.Value,
                    Platform = Constants.SupportedDeviceNames.Android
                }) ??
                new List<PlatformOutcome>();

            return new NotificationOutcomeResponse
            {
                State = notificationOutcome.State.ToString(),
                EnqueueTime = notificationOutcome.EnqueueTime,
                StartTime = notificationOutcome.StartTime,
                EndTime = notificationOutcome.EndTime,
                PnsErrorDetailsUri = notificationOutcome.PnsErrorDetailsUri,
                PlatformOutcomes = iosPushNotificationOutcomeCounts.Concat(androidPushNotificationOutcomeCounts)
            };
        }

        public override string ToString()
        {
            return _description;
        }
    }
}