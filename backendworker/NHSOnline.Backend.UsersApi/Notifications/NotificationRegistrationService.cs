using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Notifications.Models;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal class NotificationRegistrationService : INotificationRegistrationService
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

                await _notificationClient.DeleteInstallationsByDevicePns(request.DevicePns);
                var installationId = await _notificationClient.CreateInstallation(request);

                _logger.LogInformation("New registration created");
                return new RegistrationResult.Success(new NotificationRegistrationResult
                {
                    Id = installationId
                });
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Any(x => x is HttpRequestException || x is MessagingException))
            {
                _logger.LogError(ex, "Failed to register installation with Azure, an unexpected exception has been thrown");
                return new RegistrationResult.BadGateway();
            }
            catch (AggregateException ex)
            {
                _logger.LogError(ex, "Failed to register installation with Azure, an unexpected exception has been thrown");
                return new RegistrationResult.InternalServerError();
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

        public async Task<RegistrationExistsResult> Exists(UserDevice userDevice)
        {
            try
            {
                _logger.LogEnter();

                if(await _notificationClient.InstallationExists(userDevice.RegistrationId, userDevice.NhsLoginId))
                {
                    return new RegistrationExistsResult.Found();
                }
                return new RegistrationExistsResult.NotFound();
            }
            catch (MessagingException ex)
            {
                _logger.LogError(ex, "Failed to check if installation exists on Azure notification hubs, an unexpected exception has been thrown");
                return new RegistrationExistsResult.BadGateway();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to check if installation exists on Azure notification hubs, an unexpected exception has been thrown");
                return new RegistrationExistsResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check if installation exists on Azure notification hubs, an unexpected exception has been thrown");
                return new RegistrationExistsResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<FindRegistrationsResult> Find(string nhsLoginId)
        {
            try
            {
                _logger.LogEnter();

                var registrations = await _notificationClient.FindInstallationIdsByNhsLoginId(nhsLoginId);

                if(registrations?.Count > 0)
                {
                    return new FindRegistrationsResult.Found(registrations);
                }
                return new FindRegistrationsResult.NotFound();
            }
            catch (MessagingException ex)
            {
                _logger.LogError(ex, "Failed to find registrations on Azure notification hubs, an unexpected exception has been thrown");
                return new FindRegistrationsResult.BadGateway();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to find registrations on Azure notification hubs, an unexpected exception has been thrown");
                return new FindRegistrationsResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to find registrations on Azure notification hubs, an unexpected exception has been thrown");
                return new FindRegistrationsResult.InternalServerError();
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