using System;
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
    }
}
