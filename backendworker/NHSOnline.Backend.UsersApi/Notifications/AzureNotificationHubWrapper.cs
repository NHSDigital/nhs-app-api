using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using NHSOnline.Backend.UsersApi.Notifications.Extensions;
using NHSOnline.Backend.UsersApi.Notifications.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal class AzureNotificationHubWrapper : IAzureNotificationHubWrapper
    {
        private const int InstallationRecordMaxResults = 100;

        private readonly INotificationHubClient _hubClient;

        private readonly IInstallationFactory _installationFactory;
        private readonly INhsLoginIdService _readService;
        private readonly INhsLoginIdService _writeService;

        private readonly string _description;

        public int Generation { get; }
        public string Path { get; }

        public AzureNotificationHubWrapper(
            AzureNotificationHubConfiguration configuration,
            INotificationHubClientFactory notificationHubClientFactory)
        {
            _hubClient = notificationHubClientFactory.CreateClientFromConnectionString(
                $"{configuration.ConnectionString}{configuration.SharedAccessKey}",
                configuration.NotificationHubPath
            );

            _installationFactory = new InstallationFactory(new InstallationTemplateFactory());
            _readService = new NhsLoginIdService(configuration.ReadCharacters);
            _writeService = new NhsLoginIdService(configuration.WriteCharacters);

            _description = $"{configuration.NotificationHubPath} "
                           + $"(Gen {configuration.Generation}: "
                           + $"Reads [{configuration.ReadCharacters.ToUpperInvariant()}], "
                           + $"Writes [{configuration.WriteCharacters.ToUpperInvariant()}])";

            Generation = configuration.Generation;
            Path = configuration.NotificationHubPath;
        }

        public bool CanReadFor(string nhsLoginId) => _readService.HandlesNhsLoginId(nhsLoginId);
        public bool CanWriteFor(string nhsLoginId) => _writeService.HandlesNhsLoginId(nhsLoginId);

        public async Task<string> CreateInstallation(InstallationRequest request)
        {
            var installation = _installationFactory.Create(request);

            await _hubClient.CreateOrUpdateInstallationAsync(installation);

            return installation.InstallationId;
        }

        public Task DeleteInstallation(string installationId) => _hubClient.DeleteInstallationAsync(installationId);

        public Task<bool> InstallationExists(string installationId) => _hubClient.InstallationExistsAsync(installationId);

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
            var notificationOutcome = await _hubClient.ScheduleNotificationAsync(notification, request.ScheduledTime.Value, tag);

            return notificationOutcome.ScheduledNotificationId;
        }

        public override string ToString() => _description;
    }
}