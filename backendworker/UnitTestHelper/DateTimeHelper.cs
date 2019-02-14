using NHSOnline.Backend.Support.Temporal;
using System;
using System.Globalization;
using FluentAssertions;

namespace UnitTestHelper
{
    public static class DateTimeHelper
    {
        public static DateTimeOffset GetDateTimeOffsetForTest(this IDateTimeOffsetProvider dateTimeOffsetProvider, string dateTime)
        {
            var success = dateTimeOffsetProvider.TryCreateDateTimeOffset(dateTime, out var date);
            success.Should().BeTrue("Test setup incorrect, only correct strings should be passed into this method");
            return date.Value;
        }

        public static DateTimeOffset GetDateTimeOffsetForTest(this IDateTimeOffsetProvider dateTimeOffsetProvider, DateTime dateTime)
        {
            return GetDateTimeOffsetForTest(dateTimeOffsetProvider, DateTimeToJson(dateTime));
        }

        public static string DateTimeToJson(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
