using Microsoft.Extensions.Configuration;
using System;

namespace NHSOnline.Backend.Worker.Support.Date
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
