using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Temporal
{
    public interface ITimeZoneInfoProvider
    {
        TimeZoneInfo TimeZone { get; }
    }
    
    public class TimeZoneInfoProvider : ITimeZoneInfoProvider
    {
        private readonly ILogger<TimeZoneInfoProvider> _logger;

        public TimeZoneInfo TimeZone { get; }

        public TimeZoneInfoProvider(ILogger<TimeZoneInfoProvider> logger, IConfiguration configuration)
        {
            _logger = logger;
            TimeZone = Create(configuration);
        }

        private TimeZoneInfo Create(IConfiguration configuration)
        {
            var timeZoneId = configuration["TIMEZONE"];
            if (string.IsNullOrEmpty(timeZoneId))
            {
                return TimeZoneInfo.Utc;
            }

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(configuration["TIMEZONE"]);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to determine TimeZone for ID '{0}'; assuming UTC.", timeZoneId);
                return TimeZoneInfo.Utc;
            }
        }
    }
}
