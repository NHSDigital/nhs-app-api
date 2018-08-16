using System;
using System.Globalization;

namespace NHSOnline.Backend.Worker.Support.Temporal
{
    public interface IDateTimeOffsetProvider
    {
        DateTimeOffset CreateDateTimeOffset(string dateTime);
        DateTimeOffset CreateDateTimeOffset(DateTime dateTime);
        DateTimeOffset CreateDateTimeOffset();
        DateTimeOffset ConvertToLocalTime(DateTimeOffset dateTimeOffset);
    }
    
    public class DateTimeOffsetProvider: IDateTimeOffsetProvider
    {
        private readonly TimeZoneInfo _localTimeZone;

        public DateTimeOffsetProvider(TimeZoneInfoProvider timeZoneInfoProvider)
        {
            _localTimeZone = timeZoneInfoProvider.TimeZone;
        }

        public DateTimeOffset CreateDateTimeOffset(string dateTime)
        {
            var parsedDateTime = DateTime.Parse(dateTime, CultureInfo.InvariantCulture, DateTimeStyles.None);
            var offSet = _localTimeZone.GetUtcOffset(parsedDateTime);
            return new DateTimeOffset(parsedDateTime, offSet);
        }
        public DateTimeOffset CreateDateTimeOffset(DateTime dateTime)
        {
            dateTime = TimeZoneInfo.ConvertTime(dateTime, _localTimeZone);
            var offSet = _localTimeZone.GetUtcOffset(dateTime);
            return new DateTimeOffset(dateTime, offSet);
        }
        public DateTimeOffset CreateDateTimeOffset()
        {
            return CreateDateTimeOffset(DateTime.UtcNow);
        }

        public DateTimeOffset ConvertToLocalTime(DateTimeOffset dateTimeOffset)
        {
            return TimeZoneInfo.ConvertTime(dateTimeOffset, _localTimeZone);
        }
    }
}
