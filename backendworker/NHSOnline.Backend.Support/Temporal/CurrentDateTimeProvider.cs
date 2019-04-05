using System;

namespace NHSOnline.Backend.Support.Temporal
{
    public interface ICurrentDateTimeProvider
    {
        DateTime UtcNow { get; }
        DateTime LocalNow { get; }
        DateTime Today { get; }
    }
    
    public class CurrentDateTimeProvider : ICurrentDateTimeProvider
    {
        private readonly TimeZoneInfo _timeZoneInfo;

        public CurrentDateTimeProvider(ITimeZoneInfoProvider timeZoneProvider)
        {
            _timeZoneInfo = timeZoneProvider.TimeZone;
        }

        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime LocalNow => TimeZoneInfo.ConvertTime(DateTime.UtcNow, _timeZoneInfo);
        public DateTime Today => DateTime.Today;
    }
}