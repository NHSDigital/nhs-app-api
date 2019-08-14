using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure.Steps
{
    internal class CreateOrUpdateRegistrationStep : OperationStep
    {
        private readonly IAzureNotificationHubClient _azureHubClient;

        public CreateOrUpdateRegistrationStep(ILogger<CreateOrUpdateRegistrationStep> logger,
            IAzureNotificationHubClient azureHubClient) : base(logger)
        {
            _azureHubClient = azureHubClient;
        }

        public override async Task<RegistrationResult> Execute(RegistrationDescription registration,
            NotificationRegistrationRequest request)
        {
            try
            {
                var registrationResult = await _azureHubClient.CreateOrUpdateRegistrationAsync(registration);

                var isValid = new ValidateAndLog(_logger)
                    .IsNotNullOrWhitespace(registrationResult?.RegistrationId,
                        nameof(registrationResult.RegistrationId))
                    .IsNotNull(registrationResult?.ExpirationTime, nameof(registrationResult.ExpirationTime))
                    .IsValid();

                if (!isValid)
                {
                    _logger.LogError($"Registration creation did not return registration details, failed to register");
                    return new RegistrationResult.InternalServerError();
                }

                return new RegistrationResult.Success(new NotificationRegistrationResult
                {
                    RegistrationId = registrationResult.RegistrationId,
                    RegistrationExpiry = registrationResult.ExpirationTime
                });
            }
            catch (MessagingException ex)
            {
                return ReturnGoneIfHubResponseIsGone(ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    $"Failed request to update registration on Azure notification hubs, HttpRequestException has been thrown.");
                return new RegistrationResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Failed register on Azure notification hubs, an unexpected exception has been thrown");
                return new RegistrationResult.InternalServerError();
            }
        }

        private RegistrationResult ReturnGoneIfHubResponseIsGone(MessagingException ex)
        {
            var webex = ex.InnerException as WebException;
            if (webex?.Status == WebExceptionStatus.ProtocolError)
            {
                var response = (HttpWebResponse) webex.Response;
                if (response.StatusCode == HttpStatusCode.Gone)
                {
                    _logger.LogError(
                        $"Azure registration returned a response of Gone the resource is no longer available");
                    return new RegistrationResult.BadGateway();
                }
            }

            _logger.LogError(ex,
                $"Failed request to register with Azure notification hubs, MessagingException has been thrown.");
            return new RegistrationResult.BadGateway();
        }
    }
}