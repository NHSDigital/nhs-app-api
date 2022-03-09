using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.UsersApi.Areas.CustomMetrics
{
    public class CustomMetricsController : Controller
    {
        private readonly IMetricLogger _metricLogger;
        private readonly ILogger<CustomMetricsController> _logger;

        public CustomMetricsController(IMetricLogger metricLogger, ILogger<CustomMetricsController> logger)
        {
            _metricLogger = metricLogger;
            _logger = logger;
        }

        [Route("api/users/me/devices/prompt/metrics")]
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

        [Route("api/users/me/silver-integration-blocked/metrics")]
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
    }
}