using System;

namespace NHSOnline.Backend.AspNet.HealthChecks
{
    public class DateTimeHelperService : IDateTimeHelperService
    {
        public long GetUtcNowTimestampAsUnixTimeSeconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}