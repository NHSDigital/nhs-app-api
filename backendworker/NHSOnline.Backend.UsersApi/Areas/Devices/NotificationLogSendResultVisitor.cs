using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class NotificationLogSendResultVisitor : INotificationSendResultVisitor<Task>
    {
        private readonly IEventHubLogger _eventHubLogger;
        private readonly ILogger _logger;
        private readonly string _nhsLoginId;
        private readonly IMapper<AddNotificationSenderContext, SenderContextEventLogData> _mapper;
        private readonly NotificationSendRequest _notificationSendRequest;

        public NotificationLogSendResultVisitor(
            IEventHubLogger eventHubLogger,
            ILogger logger,
            IMapper<AddNotificationSenderContext, SenderContextEventLogData> mapper,
            string nhsLoginId,
            NotificationSendRequest notificationSendRequest)
        {
            _eventHubLogger = eventHubLogger;
            _logger = logger;
            _mapper = mapper;
            _nhsLoginId = nhsLoginId;
            _notificationSendRequest = notificationSendRequest;
        }

        public Task Visit(NotificationSendResult.BadGateway result) => Task.CompletedTask;

        public Task Visit(NotificationSendResult.Conflict result) => Task.CompletedTask;

        public Task Visit(NotificationSendResult.InternalServerError result) => Task.CompletedTask;

        public async Task Visit(NotificationSendResult.Success result)
        {
            try
            {
                var senderContextLogData = _mapper.Map(_notificationSendRequest.SenderContext);

                await _eventHubLogger.NotificationEnqueued(
                    new NotificationEnqueuedEventLogData(
                    _nhsLoginId,
                    result.NotificationResponse.NotificationId,
                    result.NotificationResponse.TrackingId,
                    result.NotificationResponse.Scheduled,
                    senderContextLogData
                ));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when logging notification send result");
            }
        }
    }
}