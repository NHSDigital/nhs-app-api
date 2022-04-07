using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Users.Notifications.Models;

namespace NHSOnline.Backend.Users.Notifications
{
    public class NotificationRegistrationService : INotificationRegistrationService
    {
        private readonly ILogger<NotificationRegistrationService> _logger;
        private readonly INotificationClient _notificationClient;

        public NotificationRegistrationService(
            INotificationClient notificationClient,
            ILogger<NotificationRegistrationService> logger)
        {
            _notificationClient = notificationClient;
            _logger = logger;
        }

        public async Task<RegistrationResult> Register(InstallationRequest request)
        {
            try
            {
                _logger.LogEnter();

                var installationId = await _notificationClient.CreateInstallation(request);

                _logger.LogInformation("New registration created");
                return new RegistrationResult.Success(new NotificationRegistrationResult
                {
                    Id = installationId
                });
            }
            catch (MessagingException ex)
            {
                _logger.LogError(ex, "Failed to register installation with Azure, an unexpected exception has been thrown");
                return new RegistrationResult.BadGateway();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to register installation with Azure, an unexpected exception has been thrown");
                return new RegistrationResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register installation with Azure, an unexpected exception has been thrown");
                return new RegistrationResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<DeleteRegistrationResult> Delete(string id, string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();
                await _notificationClient.DeleteInstallation(id, nhsLoginId);

                return new DeleteRegistrationResult.Success();
            }
            catch (MessagingException ex)
            {
                _logger.LogError(ex, "Failed to delete installation on Azure notification hubs, an unexpected exception has been thrown");
                return new DeleteRegistrationResult.BadGateway();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to delete installation on Azure notification hubs, an unexpected exception has been thrown");
                return new DeleteRegistrationResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete installation on Azure notification hubs, an unexpected exception has been thrown");
                return new DeleteRegistrationResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}