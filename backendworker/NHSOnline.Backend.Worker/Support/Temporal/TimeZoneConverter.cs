using System;

namespace NHSOnline.Backend.Worker.Support.Temporal
{
    public class TimeZoneConverter
    {
        private readonly TimeZoneInfo _localTimeZone;

        public TimeZoneConverter(TimeZoneInfoProvider localTimeZoneProvider)
        {
            _localTimeZone = localTimeZoneProvider.TimeZone;
        }

        public DateTime ToLocalTime(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, _localTimeZone);
        }

        public DateTimeOffset ToLocalTime(DateTimeOffset dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, _localTimeZone);
        }
    }
}