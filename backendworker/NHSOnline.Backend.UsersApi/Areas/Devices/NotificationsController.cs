using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.AspNet.ApiKey;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;
using NHSOnline.Backend.UsersApi.Notifications;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)]
    public class NotificationsController : Controller
    {
        private readonly ILogger<NotificationsController> _logger;
        private readonly INotificationService _notificationService;
        private readonly IEventHubLogger _eventHubLogger;

        private readonly IMapper<AddNotificationSenderContext, SenderContextEventLogData>
            _notificationSenderContextEventLogDataMapper;

        public NotificationsController(
            INotificationService notificationService,
            ILogger<NotificationsController> logger,
            IEventHubLogger eventHubLogger,
            IMapper<AddNotificationSenderContext, SenderContextEventLogData> notificationSenderContextEventLogDataMapper)
        {
            _notificationService = notificationService;
            _logger = logger;
            _eventHubLogger = eventHubLogger;
            _notificationSenderContextEventLogDataMapper = notificationSenderContextEventLogDataMapper;
        }

        [HttpPost]
        [Route("api/users/{nhsLoginId}/devices/notifications")]
        public async Task<IActionResult> Post
        (
            [FromRoute(Name = "nhsLoginId")] string nhsLoginId,
            [FromBody] NotificationSendRequest notificationSendRequest
        )
        {
            try
            {
                _logger.LogEnter();

                if (!IsNotificationSendRequestValid(notificationSendRequest))
                {
                    return BadRequest();
                }

                var sendResult = await _notificationService.Send(nhsLoginId, notificationSendRequest);
                await sendResult.Accept(new NotificationLogSendResultVisitor(
                    _eventHubLogger,
                    _logger,
                    _notificationSenderContextEventLogDataMapper,
                    nhsLoginId,
                    notificationSendRequest)
                );

                return sendResult.Accept(new NotificationSendResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private bool IsNotificationSendRequestValid(NotificationSendRequest request)
        {
            return new ValidateAndLog(_logger)
                .IsNotNull(request, nameof(request))
                .IsNotNullOrWhitespace(request?.Body, nameof(request.Body))
                .IsUriOrNull(request?.Url, nameof(request.Url))
                .IsValid();
        }
    }
}