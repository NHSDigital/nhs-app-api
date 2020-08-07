using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal class MigrationService : IMigrationService
    {
        private readonly ILogger<MigrationService> _logger;
        private readonly INotificationRegistrationService _notificationRegistrationService;
        private readonly IAzureNotificationHubClient _azureHubClient;
        private readonly IInstallationFactory _installationFactory;

        public MigrationService
        (
            INotificationRegistrationService notificationRegistrationService,
            IAzureNotificationHubClient azureHubClient,
            IInstallationFactory installationFactory,
            ILogger<MigrationService> logger
        )
        {
            _notificationRegistrationService = notificationRegistrationService;
            _azureHubClient = azureHubClient;
            _installationFactory = installationFactory;
            _logger = logger;
        }

        public async Task<RegistrationResult> Register(string devicePns, DeviceType deviceType, string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                var installation = _installationFactory.Create(devicePns, deviceType, nhsLoginId);
                await _azureHubClient.CreateOrUpdateInstallation(installation);

                _logger.LogInformation("New registration created");
                return new RegistrationResult.Success(new NotificationRegistrationResult
                {
                    Id = installation.InstallationId
                });
            }
            catch (MessagingException ex)
            {
                _logger.LogError("Failed to register installation with Azure, an unexpected exception has been thrown",
                    ex);
                return new RegistrationResult.BadGateway();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Failed to register installation with Azure, an unexpected exception has been thrown",
                    ex);
                return new RegistrationResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to register installation with Azure, an unexpected exception has been thrown",
                    ex);
                return new RegistrationResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public Task<DeleteRegistrationResult> Delete(string id) => _notificationRegistrationService.Delete(id);
    }
}
