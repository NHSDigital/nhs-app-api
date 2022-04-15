using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.AspNet.ApiKey;
using NHSOnline.Backend.Metrics.EventHub;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Users.Areas.Devices;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Notifications;

namespace NHSOnline.Backend.PfsApi.Areas.Users.Devices
{
    [Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)]
    public class NotificationsController : Controller
    {
        private readonly IEventHubLogger _eventHubLogger;
        private readonly ILogger<NotificationsController> _logger;

        private readonly IMapper<AddNotificationSenderContext, SenderContextEventLogData>
            _notificationSenderContextEventLogDataMapper;

        private readonly INotificationService _notificationService;

        public NotificationsController(
            INotificationService notificationService,
            ILogger<NotificationsController> logger,
            IEventHubLogger eventHubLogger,
            IMapper<AddNotificationSenderContext, SenderContextEventLogData>
                notificationSenderContextEventLogDataMapper)
        {
            _notificationService = notificationService;
            _logger = logger;
            _eventHubLogger = eventHubLogger;
            _notificationSenderContextEventLogDataMapper = notificationSenderContextEventLogDataMapper;
        }

        [HttpPost]
        [ApiVersionRoute("api/users/{nhsLoginId}/devices/notifications")]
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

        [HttpGet]
        [ApiVersionRoute("api/users/devices/notifications/{notificationId}")]
        public async Task<IActionResult> GetNotificationOutcomeDetails
        (
            [FromRoute(Name = "notificationId")] string notificationId,
            [FromQuery(Name = "hubPath")] string hubPath
        )
        {
            try
            {
                _logger.LogEnter();

                if (string.IsNullOrWhiteSpace(notificationId))
                {
                    return BadRequest();
                }

                if (string.IsNullOrWhiteSpace(hubPath))
                {
                    return BadRequest();
                }

                var outcomeResult = await _notificationService.GetNotificationOutcomeDetails(notificationId, hubPath);
                return outcomeResult.Accept(new NotificationOutcomeResultVisitor());
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