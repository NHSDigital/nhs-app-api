using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Notifications.Models;

namespace NHSOnline.Backend.Users.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly INotificationClient _notificationClient;
        private readonly INotificationsConfiguration _notificationsConfiguration;
        private readonly IMessageService _messageService;

        public NotificationService(
            ILogger<NotificationService> logger,
            INotificationClient notificationClient,
            INotificationsConfiguration notificationsConfiguration,
            IMessageService messageService)
        {
            _logger = logger;
            _notificationClient = notificationClient;
            _notificationsConfiguration = notificationsConfiguration;
            _messageService = messageService;
        }

        public async Task<NotificationSendResult> Send(string nhsLoginId,
            NotificationSendRequest notificationSendRequest)
        {
            try
            {
                _logger.LogEnter();

                var badgeCount = await GetBadgeCount(nhsLoginId);

                var request = new NotificationRequest
                {
                    Title = notificationSendRequest.Title?.Trim(),
                    Subtitle = notificationSendRequest.Subtitle?.Trim(),
                    Body = notificationSendRequest.Body.Trim(),
                    Url = string.IsNullOrWhiteSpace(notificationSendRequest.Url)
                        ? null
                        : new Uri(notificationSendRequest.Url.Trim()),
                    NhsLoginId = nhsLoginId,
                    ScheduledTime = notificationSendRequest.ScheduledTime,
                    BadgeCount = badgeCount,
                };

                var notificationResponse = await _notificationClient.SendNotification(request);
                return new NotificationSendResult.Success(notificationResponse);
            }
            catch (InstallationNotFoundException ex)
            {
                _logger.LogError(ex, "Failed to send notification, installation not found on any hub");
                return new NotificationSendResult.Conflict();
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

        private async Task<long> GetBadgeCount(string nhsLoginId)
        {
            if (!_notificationsConfiguration.IosBadgeCountEnabled)
            {
                return 0;
            }

            var result = await _messageService.GetUnreadMessageCount(nhsLoginId);

            return result.Accept(new NotificationBadgeCountResultVisitor());
        }

        public async Task<NotificationOutcomeResult> GetNotificationOutcomeDetails(string notificationId,
            string hubPath)
        {
            try
            {
                _logger.LogEnter();
                var notificationOutcomeResponse =
                    await _notificationClient.GetNotificationOutcomeDetails(notificationId, hubPath);
                return new NotificationOutcomeResult.Success(notificationOutcomeResponse);
            }
            catch (NotificationHubNotFoundException ex)
            {
                _logger.LogError(ex, "Failed to retrieve notification outcome details for " +
                                     $"NotificationId = {notificationId} and HubPath = {hubPath}," +
                                     ". The requested hub path was not found");
                return new NotificationOutcomeResult.NotFound();
            }
            catch (MessagingEntityNotFoundException ex)
            {
                _logger.LogError(ex, "Failed to retrieve notification outcome details for " +
                                     $"NotificationId = {notificationId} and HubPath = {hubPath}," +
                                     ".The requested notification outcome details are not present in the hub");

                return new NotificationOutcomeResult.NotFound();
            }
            catch (MessagingException ex)
            {
                _logger.LogError(ex, "Failed to retrieve notification outcome details for " +
                                     $"NotificationId = {notificationId} and HubPath = {hubPath}," +
                                     ". Possibly the outcome for the current notification is not yet available.");

                return new NotificationOutcomeResult.BadGateway();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to retrieve notification outcome details for " +
                                     $"NotificationId = {notificationId} and HubPath = {hubPath}," +
                                     " an unexpected exception has been thrown");
                return new NotificationOutcomeResult.BadGateway();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve notification outcome details for " +
                                     $"NotificationId = {notificationId} and HubPath = {hubPath}," +
                                     ". An unexpected exception has been thrown");
                return new NotificationOutcomeResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}