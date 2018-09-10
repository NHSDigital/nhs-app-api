using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
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
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOS()) });
            _timeZoneInfoProvider = new TimeZoneInfoProvider(configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
        }
        
        [TestMethod]
        public void SetsDefaultRange_WhenFromDateAndToDateAreNull()
        {

            var todayDate = _dateTimeOffsetProvider.CreateDateTimeOffset(new DateTime(2018, 5, 12));
            var expectedFromDate = _dateTimeOffsetProvider.CreateDateTimeOffset(new DateTime(2018, 5, 12, 14, 15, 31));
            var expectedToDate = _dateTimeOffsetProvider.CreateDateTimeOffset(new DateTime(2018, 6, 10)).SetTimeToMidnight();

            var mockDateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            mockDateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset()).Returns(expectedFromDate);
            mockDateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset(todayDate.DateTime)).Returns(todayDate);
            
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
            var fromDate = _dateTimeOffsetProvider.CreateDateTimeOffset(new DateTime(2018, 5, 12, 14, 15, 31));
            var mockDateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            
            //Act
            var dateRange = new AppointmentSlotsDateRange(mockDateTimeOffsetProvider.Object, fromDate, null);

            //Assert
            var expectedFromDate = _dateTimeOffsetProvider.CreateDateTimeOffset(new DateTime(2018, 5, 12, 14, 15, 31));
            var expectedToDate = _dateTimeOffsetProvider.CreateDateTimeOffset(new DateTime(2018, 6, 10)).SetTimeToMidnight();
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
            var toDate = _dateTimeOffsetProvider.CreateDateTimeOffset(new DateTime(2018, 5, 27, 18, 45, 22));
            var toDateAtMidnight = _dateTimeOffsetProvider.CreateDateTimeOffset(new DateTime(2018, 5, 27)).SetTimeToMidnight();
            var expectedFromDate = _dateTimeOffsetProvider.CreateDateTimeOffset(new DateTime(2018, 4, 29)).SetTimeToMidnight();
            var expectedFromDateOut = _dateTimeOffsetProvider.CreateDateTimeOffset(new DateTime(2018, 4, 29)).SetTimeToMidnight();
            var expectedToDateOut = _dateTimeOffsetProvider.CreateDateTimeOffset(new DateTime(2018, 5, 27, 18, 45, 22));

            var mockDateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            mockDateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset()).Returns(expectedFromDate);
            mockDateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset(toDateAtMidnight.DateTime)).Returns(toDateAtMidnight);
            
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
