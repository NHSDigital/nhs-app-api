using System;
using Microsoft.Extensions.Configuration;

namespace NHSOnline.Backend.Worker.Support.Temporal
{
    public class TimeZoneInfoProvider
    {
        public TimeZoneInfo TimeZone { get; }

        public TimeZoneInfoProvider(IConfiguration configuration)
        {
            TimeZone = Create(configuration);
        }

        private static TimeZoneInfo Create(IConfiguration configuration)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(configuration["TIMEZONE"]);
            }
            catch (Exception)
            {
                return TimeZoneInfo.Utc;
            }
        }
    }
}
