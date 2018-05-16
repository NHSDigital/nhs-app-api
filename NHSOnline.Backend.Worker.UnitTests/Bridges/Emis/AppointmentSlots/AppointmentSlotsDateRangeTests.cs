using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Bridges.Emis.AppointmentSlots;
using NHSOnline.Backend.Worker.Date;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis.AppointmentSlots
{
    [TestClass]
    public class AppointmentSlotsDateRangeTests
    {
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _timeZoneInfoProvider = new TimeZoneInfoProvider();
        }
        
        [TestMethod]
        public void SetsDefaultRange_WhenFromDateAndToDateAreNull()
        {
            var todayDate = new DateTime(2018, 5, 12, 0, 0, 0, 0);
            var expectedFromDate = new DateTime(2018, 5, 12, 14, 15, 31);
            var expectedToDate = new DateTimeOffset(2018, 5, 27, 0, 0, 0, new TimeSpan(1, 0, 0));

            var expectedFromUtc = new DateTimeOffset(2018, 5, 12, 14, 15, 31, new TimeSpan(1, 0, 0)).ToUniversalTime();
            var expectedToUtc = expectedToDate.ToUniversalTime();


            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
            var mockDateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            mockDateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset()).Returns(dateTimeOffsetProvider.CreateDateTimeOffset(expectedFromDate));
            mockDateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset(todayDate)).Returns(dateTimeOffsetProvider.CreateDateTimeOffset(todayDate));
            
            var dateRange = new AppointmentSlotsDateRange(mockDateTimeOffsetProvider.Object, null, null);

            dateRange.FromDate.DateTime.Should().BeSameDateAs(expectedFromUtc.Date);
            dateRange.FromDate.Should().HaveHour(expectedFromUtc.Hour);
            dateRange.FromDate.Should().HaveMinute(expectedFromUtc.Minute);
            dateRange.FromDate.Should().HaveSecond(expectedFromUtc.Second);
            
            dateRange.ToDate.DateTime.Should().BeSameDateAs(expectedToUtc.Date);
            dateRange.ToDate.Should().HaveHour(expectedToUtc.Hour);
            dateRange.ToDate.Should().HaveMinute(expectedToUtc.Minute);
            dateRange.ToDate.Should().HaveSecond(expectedToUtc.Second);
        }
        
        [TestMethod]
        public void SetsDefaultToDate_WhenToDateIsNull()
        {
            var expectedFromDate = new DateTimeOffset(2018, 5, 12, 14, 15, 31, new TimeSpan(1,0,0));
            var expectedToDate = new DateTimeOffset(2018, 5, 27, 0, 0, 0, new TimeSpan(1,0,0));
            var expectedFromUtc = expectedFromDate.ToUniversalTime();
            var expectedToUtc = expectedToDate.ToUniversalTime();
            
            var mockDateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            
            var dateRange = new AppointmentSlotsDateRange(mockDateTimeOffsetProvider.Object, expectedFromDate, null);

            dateRange.FromDate.DateTime.Should().BeSameDateAs(expectedFromUtc.Date);
            dateRange.FromDate.Should().HaveHour(expectedFromUtc.Hour);
            dateRange.FromDate.Should().HaveMinute(expectedFromUtc.Minute);
            dateRange.FromDate.Should().HaveSecond(expectedFromUtc.Second);
            
            dateRange.ToDate.DateTime.Should().BeSameDateAs(expectedToUtc.Date);
            dateRange.ToDate.Should().HaveHour(expectedToUtc.Hour);
            dateRange.ToDate.Should().HaveMinute(expectedToUtc.Minute);
            dateRange.ToDate.Should().HaveSecond(expectedToUtc.Second);
        }
        
        [TestMethod]
        public void SetsDefaultFromDate_WhenFromDateIsNull()
        {
            var toDateAtMidnight = new DateTime(2018, 5, 27, 0, 0, 0);
            var expectedFromDate = new DateTime(2018, 5, 13, 0, 0, 0);
            var expectedToDate = new DateTimeOffset(2018, 5, 27, 18, 45, 22, new TimeSpan(1,0,0));

            var expectedFromDateOut = new DateTimeOffset(2018, 5, 13, 0, 0, 0, new TimeSpan(1, 0, 0)).ToUniversalTime();
            var expectedToDateOut = new DateTimeOffset(2018, 5, 27, 18, 45, 22, new TimeSpan(1, 0, 0)).ToUniversalTime();


            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
            var mockDateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            mockDateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset()).Returns(dateTimeOffsetProvider.CreateDateTimeOffset(expectedFromDate));
            mockDateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset(toDateAtMidnight)).Returns(dateTimeOffsetProvider.CreateDateTimeOffset(toDateAtMidnight));
            
            var dateRange = new AppointmentSlotsDateRange(mockDateTimeOffsetProvider.Object, null, expectedToDate);

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
