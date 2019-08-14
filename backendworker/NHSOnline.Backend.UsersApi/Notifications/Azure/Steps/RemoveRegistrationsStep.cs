using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.UsersApi.Notifications.Azure.Steps
{
    internal class RemoveRegistrationsStep : OperationStep
    {
        private readonly IAzureNotificationHubClient _azureHubClient;

        public RemoveRegistrationsStep(ILogger<RemoveRegistrationsStep> logger, IAzureNotificationHubClient azureHubClient) : base(logger)
        {
            _azureHubClient = azureHubClient;
        }

        public override async Task<RegistrationResult> Execute(RegistrationDescription registration, NotificationRegistrationRequest request)
        {
            try
            {
                await RemoveExistingRegistrations(request.DevicePns);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Failed request to remove existing registrations on Azure notification hubs, HttpRequestException has been thrown.");
                return new RegistrationResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to remove existing registrations on Azure notification hubs, an unexpected exception has been thrown");
                return new RegistrationResult.InternalServerError();
            }
            
            return await Next.Execute(registration, request);
        }
        
        private async Task RemoveExistingRegistrations(string devicePns)
        {
            if (devicePns != null)
            {
                var registrations = await _azureHubClient.GetRegistrationsByChannelAsync(devicePns);

                foreach (RegistrationDescription registration in registrations)
                {
                    await _azureHubClient.DeleteRegistrationAsync(registration);
                }
            }
        }
    }
}