using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public class NotificationService: INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly IAzureNotificationHubClient _azureHubClient;

        public NotificationService(ILogger<NotificationService> logger, IAzureNotificationHubClient azureHubClient)
        {
            _logger = logger;
            _azureHubClient = azureHubClient;
        }

        public async Task<NotificationSendResult> Send(string nhsLoginId,
            NotificationSendRequest notificationSendRequest)
        {
            try
            {
                _logger.LogEnter();

                var notification = new Notification
                {
                    Title = notificationSendRequest.Title?.Trim(),
                    Subtitle = notificationSendRequest.Subtitle?.Trim(),
                    Body = notificationSendRequest.Body.Trim(),
                    Url = string.IsNullOrWhiteSpace(notificationSendRequest.Url)
                        ? null
                        : new Uri(notificationSendRequest.Url.Trim())
                };

                await _azureHubClient.SendNotification(nhsLoginId, notification);
                return new NotificationSendResult.Success();
            }
            catch (MessagingException ex)
            {
                _logger.LogError(ex, "Failed to send notification, an unexpected exception has been thrown");
                return new NotificationSendResult.BadGateway();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to send notification, an unexpected exception has been thrown");
                return new NotificationSendResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification, an unexpected exception has been thrown");
                return new NotificationSendResult.InternalServerError();
            }
            finally
            {
               _logger.LogExit();
            }
        }
    }
}