using System;
using System.Globalization;

namespace NHSOnline.Backend.Support.Temporal
{
    public interface IDateTimeOffsetProvider
    {
        DateTimeOffset CreateDateTimeOffset();
        DateTimeOffset CreateDateTimeOffset(DateTime dateTime);
        DateTimeOffset ConvertToLocalTime(DateTimeOffset dateTimeOffset);
        bool TryCreateDateTimeOffset(string dateTime, out DateTimeOffset? dateTimeOffset);
    }

    public class DateTimeOffsetProvider : IDateTimeOffsetProvider
    {
        private readonly ICurrentDateTimeProvider _currentTimeProvider;
        private readonly TimeZoneInfo _localTimeZone;

        public DateTimeOffsetProvider(ITimeZoneInfoProvider timeZoneInfoProvider, ICurrentDateTimeProvider currentTimeProvider)
        {
            _currentTimeProvider = currentTimeProvider;
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
            var dateTime = TimeZoneInfo.ConvertTime(_currentTimeProvider.UtcNow, _localTimeZone);
            var offSet = _localTimeZone.GetUtcOffset(dateTime);
            return new DateTimeOffset(dateTime, offSet);
        }
        
        public DateTimeOffset CreateDateTimeOffset(DateTime date)
        {
            var offSet = _localTimeZone.GetUtcOffset(date);
            return new DateTimeOffset(date, offSet);
        }

        public DateTimeOffset ConvertToLocalTime(DateTimeOffset dateTimeOffset)
        {
            return TimeZoneInfo.ConvertTime(dateTimeOffset, _localTimeZone);
        }
    }
}
