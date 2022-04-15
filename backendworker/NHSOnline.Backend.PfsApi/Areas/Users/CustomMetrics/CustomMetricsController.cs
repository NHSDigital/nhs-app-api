using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Registrations;

namespace NHSOnline.Backend.PfsApi.Areas.Users.CustomMetrics
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CustomMetricsController : Controller
    {
        private readonly IMetricLogger<AccessTokenMetricContext> _metricLogger;
        private readonly ILogger<CustomMetricsController> _logger;
        private readonly INotificationsDecisionAuditService _notificationsDecisionAuditService;
        private readonly IAccessTokenProvider _accessTokenProvider;

        public CustomMetricsController(IMetricLogger<AccessTokenMetricContext> metricLogger,
            ILogger<CustomMetricsController> logger,
            INotificationsDecisionAuditService notificationsDecisionAuditService,
            IAccessTokenProvider accessTokenProvider)
        {
            _metricLogger = metricLogger;
            _logger = logger;
            _notificationsDecisionAuditService = notificationsDecisionAuditService;
            _accessTokenProvider = accessTokenProvider;
        }

        [ApiVersionRoute("api/users/me/devices/prompt/metrics")]
        [HttpPost]
        [UserProfile]
        public async Task<IActionResult> PostNotificationsPromptMetrics(
            [FromBody] NotificationsPromptData notificationsPromptData)
        {
            try
            {
                _logger.LogEnter();

                if (!IsNotificationPromptDataValid(notificationsPromptData))
                {
                    return BadRequest();
                }

                await _metricLogger.NotificationsPrompt(notificationsPromptData);
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [ApiVersionRoute("api/users/me/silver-integration-blocked/metrics")]
        [HttpPost]
        [UserProfile]
        public async Task<IActionResult> PostSilverIntegrationJumpOffBlockedMetrics(
            [FromBody] SilverIntegrationJumpOffBlockedData silverIntegrationJumpOffBlockedData)
        {
            try
            {
                _logger.LogEnter();

                if (!IsSilverIntegrationJumpOffBlockedDataValid(silverIntegrationJumpOffBlockedData))
                {
                    return BadRequest();
                }

                await _metricLogger.SilverIntegrationJumpOffBlocked(silverIntegrationJumpOffBlockedData);

                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [ApiVersionRoute("api/users/me/devices/log/audit")]
        [HttpPost]
        [UserProfile]
        public async Task<IActionResult> PostNotificationsLogAudit(
            [FromBody] NotificationsAuditData notificationsAuditData)
        {
            try
            {
                _logger.LogEnter();

                if (!IsNotificationsAuditDataDataValid(notificationsAuditData))
                {
                    return BadRequest();
                }

                var accessToken = _accessTokenProvider.AccessToken;
                await _notificationsDecisionAuditService.LogAudit(
                    notificationsAuditData,
                    accessToken);

                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private bool IsNotificationPromptDataValid(NotificationsPromptData data)
        {
            return new ValidateAndLog(_logger)
                .IsNotNull(data, nameof(data))
                .IsNotNullOrWhitespace(data?.Platform, nameof(data.Platform))
                .IsNotNullOrWhitespace(
                    data?.NotificationsRegistered.ToString(
                        CultureInfo.InvariantCulture),
                    nameof(data.NotificationsRegistered))
                .IsUriOrNull(data?.ScreenShown.ToString(
                        CultureInfo.InvariantCulture),
                    nameof(data.ScreenShown))
                .IsValid();
        }

        private bool IsSilverIntegrationJumpOffBlockedDataValid(SilverIntegrationJumpOffBlockedData data)
        {
            return new ValidateAndLog(_logger)
                .IsNotNull(data, nameof(data))
                .IsNotNullOrWhitespace(data?.ProviderId, nameof(data.ProviderId))
                .IsNotNullOrWhitespace(data?.ProviderName, nameof(data.ProviderName))
                .IsNotNullOrWhitespace(data?.JumpOffId, nameof(data.JumpOffId))
                .IsNotNullOrWhitespace(data?.Reason, nameof(data.Reason))
                .IsValid();
        }

        private bool IsNotificationsAuditDataDataValid(NotificationsAuditData data)
        {
            return new ValidateAndLog(_logger)
                .IsNotNull(data, nameof(data))
                .IsNotNull(data?.NotificationsDecisionSource, nameof(data.NotificationsDecisionSource))
                .IsValid();
        }
    }
}