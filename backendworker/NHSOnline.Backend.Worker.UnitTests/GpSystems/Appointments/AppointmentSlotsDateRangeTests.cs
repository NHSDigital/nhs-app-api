using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Appointments
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
        public void SetsDefaultRange_WhenFromDateAndToDateAreNull()
        {
            const string todayDateString = "2018-05-12T00:00:00";

            DateTimeOffset? todayDate = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(todayDateString).SetTimeToMidnight();
            var expectedFromDate = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-12T14:15:31");
            var expectedToDate = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-06-10T00:00:00").SetTimeToMidnight();

            var mockDateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            mockDateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset()).Returns(expectedFromDate);
            mockDateTimeOffsetProvider.Setup(x => x.TryCreateDateTimeOffset(todayDateString, out todayDate)).Returns(true);
            
            var dateRange = new AppointmentSlotsDateRange(mockDateTimeOffsetProvider.Object, null, null);

            dateRange.FromDate.DateTime.Should().BeSameDateAs(expectedFromDate.DateTime);
            dateRange.FromDate.Should().HaveHour(expectedFromDate.Hour);
            dateRange.FromDate.Should().HaveMinute(expectedFromDate.Minute);
            dateRange.FromDate.Should().HaveSecond(expectedFromDate.Second);
            
            dateRange.ToDate.DateTime.Should().BeSameDateAs(expectedToDate.DateTime);
            dateRange.ToDate.Should().HaveHour(expectedToDate.Hour);
            dateRange.ToDate.Should().HaveMinute(expectedToDate.Minute);
            dateRange.ToDate.Should().HaveSecond(expectedToDate.Second);
        }

        [TestMethod]
        public void SetsDefaultToDate_WhenToDateIsNull()
        {
            //Arrange
            var fromDate = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-12T14:15:31");
            var mockDateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            
            //Act
            var dateRange = new AppointmentSlotsDateRange(mockDateTimeOffsetProvider.Object, fromDate, null);

            //Assert
            var expectedFromDate = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-12T14:15:31");
            var expectedToDate = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-06-10T00:00:01").SetTimeToMidnight();
            var expectedFrom = expectedFromDate;
            var expectedTo = expectedToDate;

            dateRange.FromDate.DateTime.Should().BeSameDateAs(expectedFrom.Date);
            dateRange.FromDate.Should().HaveHour(expectedFrom.Hour);
            dateRange.FromDate.Should().HaveMinute(expectedFrom.Minute);
            dateRange.FromDate.Should().HaveSecond(expectedFrom.Second);
            
            dateRange.ToDate.DateTime.Should().BeSameDateAs(expectedTo.Date);
            dateRange.ToDate.Should().HaveHour(expectedTo.Hour);
            dateRange.ToDate.Should().HaveMinute(expectedTo.Minute);
            dateRange.ToDate.Should().HaveSecond(expectedTo.Second);
        }
        
        [TestMethod]
        public void SetsDefaultFromDate_WhenFromDateIsNull()
        {
            var toDate = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-27T18:45:22");
            const string toDateAtMidnightString = "2018-05-27T00:00:00";
            DateTimeOffset? toDateAtMidnight = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(toDateAtMidnightString).SetTimeToMidnight();
            var expectedFromDate = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-04-29T00:00:01").SetTimeToMidnight();
            var expectedFromDateOut = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-04-29T00:00:01").SetTimeToMidnight();
            var expectedToDateOut = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-27T18:45:22");

            var mockDateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            mockDateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset()).Returns(expectedFromDate);
            mockDateTimeOffsetProvider.Setup(x => x.TryCreateDateTimeOffset(toDateAtMidnightString, out toDateAtMidnight)).Returns(true);
            
            var dateRange = new AppointmentSlotsDateRange(mockDateTimeOffsetProvider.Object, null, toDate);

            dateRange.FromDate.DateTime.Should().BeSameDateAs(expectedFromDateOut.Date);
            dateRange.FromDate.Should().HaveHour(expectedFromDateOut.Hour);
            dateRange.FromDate.Should().HaveMinute(expectedFromDateOut.Minute);
            dateRange.FromDate.Should().HaveSecond(expectedFromDateOut.Second);
            
            dateRange.ToDate.DateTime.Should().BeSameDateAs(expectedToDateOut.Date);
            dateRange.ToDate.Should().HaveHour(expectedToDateOut.Hour);
            dateRange.ToDate.Should().HaveMinute(expectedToDateOut.Minute);
            dateRange.ToDate.Should().HaveSecond(expectedToDateOut.Second);
        }
    }
}
