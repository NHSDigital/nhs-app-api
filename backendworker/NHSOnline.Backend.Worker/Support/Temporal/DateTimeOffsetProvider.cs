using System;
using System.Globalization;

namespace NHSOnline.Backend.Worker.Support.Temporal
{
    public interface IDateTimeOffsetProvider
    {
        DateTimeOffset CreateDateTimeOffset();
        DateTimeOffset ConvertToLocalTime(DateTimeOffset dateTimeOffset);
        bool TryCreateDateTimeOffset(string dateTime, out DateTimeOffset? dateTimeOffset);
    }

    public class DateTimeOffsetProvider : IDateTimeOffsetProvider
    {
        private readonly TimeZoneInfo _localTimeZone;

        public DateTimeOffsetProvider(TimeZoneInfoProvider timeZoneInfoProvider)
        {
            _localTimeZone = timeZoneInfoProvider.TimeZone;
        }

        public bool TryCreateDateTimeOffset(string dateTime, out DateTimeOffset? dateTimeOffset)
        {
            var success = DateTime.TryParse(dateTime,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDateTime);

            dateTimeOffset = success
                ? new DateTimeOffset(parsedDateTime, _localTimeZone.GetUtcOffset(parsedDateTime))
                : (DateTimeOffset?) null;

            return success;
        }

        public DateTimeOffset CreateDateTimeOffset()
        {
            var dateTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, _localTimeZone);
            var offSet = _localTimeZone.GetUtcOffset(dateTime);
            return new DateTimeOffset(dateTime, offSet);
        }

        public DateTimeOffset ConvertToLocalTime(DateTimeOffset dateTimeOffset)
        {
            return TimeZoneInfo.ConvertTime(dateTimeOffset, _localTimeZone);
        }
    }
}
