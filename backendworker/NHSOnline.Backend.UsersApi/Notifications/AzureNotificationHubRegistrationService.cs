using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    internal class AzureNotificationHubRegistrationService : INotificationRegistrationService
    {
        private readonly ILogger<AzureNotificationHubRegistrationService> _logger;
        private readonly IAzureNotificationHubClient _azureHubClient;
        private readonly IInstallationFactory _installationFactory;

        public AzureNotificationHubRegistrationService(
            IAzureNotificationHubClient azureHubClient,
            IInstallationFactory installationFactory,
            ILogger<AzureNotificationHubRegistrationService> logger)
        {
            _azureHubClient = azureHubClient;
            _installationFactory = installationFactory;
            _logger = logger;
        }

        public async Task<RegistrationResult> Register(string devicePns, DeviceType deviceType, AccessToken accessToken)
        {
            try
            {
                _logger.LogEnter();
                var existingInstallations = await _azureHubClient.FindInstallationIdentifiers(devicePns);

                _logger.LogInformation($"{existingInstallations.Count} existing registrations found for this pns token");
                if (existingInstallations.Count > 0)
                {
                    var deleteTasks =
                        existingInstallations.Select(GetDeleteTask)
                            .ToList();

                    await Task.WhenAll(deleteTasks);
                    _logger.LogInformation("Existing registrations deleted");
                }

                var installation = _installationFactory.Create(devicePns, deviceType, accessToken.Subject);
                await _azureHubClient.CreateOrUpdateInstallation(installation);

                _logger.LogInformation("New registration created");
                return new RegistrationResult.Success(new NotificationRegistrationResult
                {
                    Id = installation.InstallationId
                });
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Any(x => x is HttpRequestException || x is MessagingException))
            {
                _logger.LogError("Failed to register installation with Azure, an unexpected exception has been thrown", ex);
                return new RegistrationResult.BadGateway();
            }
            catch (AggregateException ex)
            {
                _logger.LogError("Failed to register installation with Azure, an unexpected exception has been thrown", ex);
                return new RegistrationResult.InternalServerError();
            }
            catch (MessagingException ex)
            {
                _logger.LogError("Failed to register installation with Azure, an unexpected exception has been thrown", ex);
                return new RegistrationResult.BadGateway();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Failed to register installation with Azure, an unexpected exception has been thrown", ex);
                return new RegistrationResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to register installation with Azure, an unexpected exception has been thrown", ex);
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

                if(await GetExistsTask(userDevice.RegistrationId))
                {
                    return new RegistrationExistsResult.Found();
                }
                return new RegistrationExistsResult.NotFound();
            }
            catch (MessagingException ex)
            {
                _logger.LogError("Failed to check if installation exists on Azure notification hubs, an unexpected exception has been thrown", ex);
                return new RegistrationExistsResult.BadGateway();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Failed to check if installation exists on Azure notification hubs, an unexpected exception has been thrown", ex);
                return new RegistrationExistsResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to check if installation exists on Azure notification hubs, an unexpected exception has been thrown", ex);
                return new RegistrationExistsResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<DeleteRegistrationResult> Delete(string id)
        {
            try
            {
                _logger.LogEnter();
                await GetDeleteTask(id);

                return new DeleteRegistrationResult.Success();
            }
            catch (MessagingException ex )
            {
                _logger.LogError("Failed to delete installation on Azure notification hubs, an unexpected exception has been thrown", ex);
                return new DeleteRegistrationResult.BadGateway();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Failed to delete installation on Azure notification hubs, an unexpected exception has been thrown", ex);
                return new DeleteRegistrationResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete installation on Azure notification hubs, an unexpected exception has been thrown", ex);
                return new DeleteRegistrationResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private Task<bool> GetExistsTask(string id)
        {
            return Guid.TryParse(id, out _) switch
            {
                true => _azureHubClient.InstallationExists(id),
                false => _azureHubClient.RegistrationExists(id)
            };
        }

        private Task GetDeleteTask(string id)
        {
            return Guid.TryParse(id, out _) switch
            {
                true => _azureHubClient.DeleteInstallation(id),
                false => _azureHubClient.DeleteRegistration(id)
            };
        }

        private Task GetDeleteTask(NotificationRegistrationItem registrationItem)
        {
            return registrationItem.Type switch
            {
                NotificationRegistrationItem.RegistrationType.Installation => _azureHubClient.DeleteInstallation(
                    registrationItem.Id),
                NotificationRegistrationItem.RegistrationType.Registration => _azureHubClient.DeleteRegistration(
                    registrationItem.Id),
                _ => throw new ArgumentOutOfRangeException(nameof(registrationItem))
            };
        }
    }
}