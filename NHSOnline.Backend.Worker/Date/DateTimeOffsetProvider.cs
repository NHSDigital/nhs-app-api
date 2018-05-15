using System;
using System.Globalization;

namespace NHSOnline.Backend.Worker.Date
{
    public interface IDateTimeOffsetProvider
    {
        DateTimeOffset CreateDateTimeOffset(string dateTime);
        DateTimeOffset CreateDateTimeOffset(DateTime dateTime);
        DateTimeOffset CreateDateTimeOffset();
    }
    
    public class DateTimeOffsetProvider: IDateTimeOffsetProvider
    {
        private readonly TimeZoneInfo _localTimeZone;

        public DateTimeOffsetProvider(TimeZoneInfoProvider timeZoneInfoProvider)
        {
            _localTimeZone = timeZoneInfoProvider.Create();
        }

        public DateTimeOffset CreateDateTimeOffset(string dateTime)
        {
            var parsedDateTime = DateTime.Parse(dateTime, CultureInfo.InvariantCulture, DateTimeStyles.None);
            var offSet = _localTimeZone.GetUtcOffset(parsedDateTime);
            return new DateTimeOffset(parsedDateTime, offSet);
        }
        public DateTimeOffset CreateDateTimeOffset(DateTime dateTime)
        {
            var offSet = _localTimeZone.GetUtcOffset(dateTime);
            return new DateTimeOffset(dateTime, offSet);
        }
        public DateTimeOffset CreateDateTimeOffset()
        {
            return CreateDateTimeOffset(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified));
        }
    }
}
