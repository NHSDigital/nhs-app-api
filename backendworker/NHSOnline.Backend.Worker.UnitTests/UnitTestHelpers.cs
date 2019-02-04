using System;
using System.Globalization;
using FluentAssertions;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.UnitTests
{
    public static class UnitTestHelpers
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
            return dateTime.ToString("yyyy-MM-ddTH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
