using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure.Steps
{
    internal class CreateRegistrationIdStep : OperationStep
    {
        private readonly IAzureNotificationHubClient _azureHubClient;
        
        public CreateRegistrationIdStep(ILogger<CreateRegistrationIdStep> logger, IAzureNotificationHubClient azureHubClient) : base(logger)
        {
            _azureHubClient = azureHubClient;
        }
        
        public override async Task<RegistrationResult> Execute(RegistrationDescription registration, string devicePns)
        {
            try
            {
                registration.RegistrationId = await _azureHubClient.CreateRegistrationId();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Failed request to create registrationId on Azure notification hubs, HttpRequestException has been thrown.");
                return new RegistrationResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create registrationId on Azure notification hubs, an unexpected exception has been thrown");
                return new RegistrationResult.InternalServerError();
            }
                
            return await Next.Execute(registration, devicePns);
        }
    }
}