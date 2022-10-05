using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Users.Notifications
{
    public class NotificationsConfiguration : INotificationsConfiguration
    {
        public bool IosBadgeCountEnabled { get; }

        public NotificationsConfiguration(ILogger<NotificationsConfiguration> logger, IConfiguration configuration)
        {
            IosBadgeCountEnabled = configuration.GetBoolOrFallback("IOS_BADGE_COUNT_ENABLED", false, logger);
        }
    }
}