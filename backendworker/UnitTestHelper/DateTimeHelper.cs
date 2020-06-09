using NHSOnline.Backend.Support.Temporal;
using System;
using System.Globalization;
using FluentAssertions;
using Moq;

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

        public static string DateTimeToJson(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }
        
        public static DateTimeOffset? MockDateTimeOffset(this Mock<IDateTimeOffsetProvider> dateTimeOffsetProviderMock, DateTime dateTime)
        {
            DateTimeOffset? slotTime= new DateTimeOffset(dateTime);
            dateTimeOffsetProviderMock.Setup(x => x.TryCreateDateTimeOffset(DateTimeToJson(dateTime), out slotTime))
                .Returns(true);
            return slotTime;
        }
        
        public static DateTimeOffset? MockDateTimeOffset(this Mock<IDateTimeOffsetProvider> dateTimeOffsetProviderMock, string dateTime)
        {
            var formattedDateTime =
                DateTime.Parse(dateTime, CultureInfo.InvariantCulture);
            DateTimeOffset? slotTime= new DateTimeOffset(formattedDateTime);
            dateTimeOffsetProviderMock.Setup(x => x.TryCreateDateTimeOffset(DateTimeToJson(formattedDateTime), out slotTime))
                .Returns(true);
            return slotTime;
        }
    }
}
