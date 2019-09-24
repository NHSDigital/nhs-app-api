using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications.Azure.Steps;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure
{
    internal class AzureNotificationHubService : INotificationService
    {
        private readonly IRegistrationDescriptionFactory _registrationFactory;
        private readonly ILogger<AzureNotificationHubService> _logger;
        private readonly IOperationStepBuilder _stepBuilder;
        private readonly IAzureNotificationHubClient _azureHubClient;

        public AzureNotificationHubService
        (
            IRegistrationDescriptionFactory registrationFactory,
            ILogger<AzureNotificationHubService> logger,
            IOperationStepBuilder stepBuilder,
            IAzureNotificationHubClient azureHubClient
        )
        {
            _registrationFactory = registrationFactory;
            _logger = logger;
            _stepBuilder = stepBuilder;
            _azureHubClient = azureHubClient;
        }

        public async Task<RegistrationResult> Register
        (
            RegisterDeviceRequest registerDeviceRequest,
            AccessToken accessToken
        )
        {
            try
            {
                _logger.LogEnter();
                var registration = _registrationFactory.Create(registerDeviceRequest, accessToken.Subject);

                return await _stepBuilder.Add<RemoveRegistrationsStep>()
                    .Add<CreateRegistrationIdStep>()
                    .Add<CreateOrUpdateRegistrationStep>()
                    .Execute(registration, registerDeviceRequest.DevicePns);
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
                var exists =
                    await _azureHubClient.RegistrationExists(userDevice.RegistrationId, userDevice.PnsToken);

                if (exists)
                {
                    return new RegistrationExistsResult.Found();
                }

                return new RegistrationExistsResult.NotFound();
            }
            catch (MessagingException ex)
            {
                _logger.LogError("Failed to check if registration exists on Azure notification hubs, an unexpected exception has been thrown", ex);
                return new RegistrationExistsResult.BadGateway();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Failed to check if registration exists on Azure notification hubs, an unexpected exception has been thrown", ex);
                return new RegistrationExistsResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to check if registration exists on Azure notification hubs, an unexpected exception has been thrown", ex);
                return new RegistrationExistsResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<DeleteRegistrationResult> Delete(string registrationId)
        {
            try
            {
                _logger.LogEnter();
                await _azureHubClient.DeleteRegistration(registrationId);

                return new DeleteRegistrationResult.Success();
            }
            catch (MessagingException ex)
            {
                _logger.LogError("Failed to delete registration on Azure notification hubs, an unexpected exception has been thrown", ex);
                return new DeleteRegistrationResult.BadGateway();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Failed to delete registration on Azure notification hubs, an unexpected exception has been thrown", ex);
                return new DeleteRegistrationResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete registration on Azure notification hubs, an unexpected exception has been thrown", ex);
                return new DeleteRegistrationResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}