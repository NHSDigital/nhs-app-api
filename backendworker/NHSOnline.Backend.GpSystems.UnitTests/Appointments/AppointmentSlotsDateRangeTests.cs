using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Appointments
{
    [TestClass]
    public class AppointmentSlotsDateRangeTests
    {
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        private DateTimeOffsetProvider _dateTimeOffsetProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            _timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
        }
        
        [TestMethod]
        public void SetsDefaultRange()
        {
            const string todayDateString = "2018-05-12T00:00:00";

            DateTimeOffset? todayDate = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(todayDateString).SetTimeToMidnight();
            var expectedFromDate = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-12T14:15:31");
            var expectedToDate = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-06-10T00:00:00").SetTimeToMidnight();

            var mockDateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            mockDateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset()).Returns(expectedFromDate);
            mockDateTimeOffsetProvider.Setup(x => x.TryCreateDateTimeOffset(todayDateString, out todayDate)).Returns(true);
            
            var dateRange = new AppointmentSlotsDateRange(mockDateTimeOffsetProvider.Object);

            dateRange.FromDate.DateTime.Should().BeSameDateAs(expectedFromDate.DateTime);
            dateRange.FromDate.Should().HaveHour(expectedFromDate.Hour);
            dateRange.FromDate.Should().HaveMinute(expectedFromDate.Minute);
            dateRange.FromDate.Should().HaveSecond(expectedFromDate.Second);
            
            dateRange.ToDate.DateTime.Should().BeSameDateAs(expectedToDate.DateTime);
            dateRange.ToDate.Should().HaveHour(expectedToDate.Hour);
            dateRange.ToDate.Should().HaveMinute(expectedToDate.Minute);
            dateRange.ToDate.Should().HaveSecond(expectedToDate.Second);
        }
    }
}
