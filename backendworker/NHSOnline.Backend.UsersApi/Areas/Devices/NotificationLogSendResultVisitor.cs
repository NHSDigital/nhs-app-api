using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class NotificationLogSendResultVisitor : INotificationSendResultVisitor<Task>
    {
        private readonly IEventHubLogger _eventHubLogger;
        private readonly ILogger _logger;
        private readonly string _nhsLoginId;

        public NotificationLogSendResultVisitor(
            IEventHubLogger eventHubLogger,
            ILogger logger,
            string nhsLoginId)
        {
            _eventHubLogger = eventHubLogger;
            _logger = logger;
            _nhsLoginId = nhsLoginId;
        }

        public Task Visit(NotificationSendResult.BadGateway result) => Task.CompletedTask;

        public Task Visit(NotificationSendResult.Conflict result) => Task.CompletedTask;

        public Task Visit(NotificationSendResult.InternalServerError result) => Task.CompletedTask;

        public async Task Visit(NotificationSendResult.Success result)
        {
            try
            {
                var data = new NotificationEnqueuedEventLogData(
                    _nhsLoginId,
                    result.NotificationResponse.NotificationId,
                    result.NotificationResponse.TrackingId,
                    result.NotificationResponse.Scheduled);

                await _eventHubLogger.NotificationEnqueued(data);
            }
            catch (Exception e)

            {
                _logger.LogError(e, "Error when logging notification send result");
            }
        }
    }
}