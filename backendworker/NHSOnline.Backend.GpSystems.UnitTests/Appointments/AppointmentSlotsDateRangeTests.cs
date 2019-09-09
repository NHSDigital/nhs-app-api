using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
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
        private IFixture _fixture;
        private Mock<ICurrentDateTimeProvider> _mockCurrentDateTimeProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _mockCurrentDateTimeProvider = _fixture.Freeze<Mock<ICurrentDateTimeProvider>>();
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            _timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
        }
        
        [TestMethod]
        public void SetsDefaultRangeAllBst()
        {
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(new DateTime(2019,9,20,12,1,0,0, DateTimeKind.Utc));
            
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);
            
            var expectedFromDate = new DateTimeOffset(2019,9,20,13,1,0, new TimeSpan(1,0,0));
            var expectedToDate = new DateTimeOffset(2019,10,19,0,0,0, new TimeSpan(1,0,0));

            var dateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider);

            dateRange.FromDate.DateTime.Should().BeSameDateAs(expectedFromDate.DateTime);
            dateRange.FromDate.Should().HaveHour(expectedFromDate.Hour);
            dateRange.FromDate.Should().HaveMinute(expectedFromDate.Minute);
            dateRange.FromDate.Should().HaveSecond(expectedFromDate.Second);
            dateRange.FromDate.Offset.Should().Be(expectedFromDate.Offset);
            
            dateRange.ToDate.DateTime.Should().BeSameDateAs(expectedToDate.DateTime);
            dateRange.ToDate.Should().HaveHour(expectedToDate.Hour);
            dateRange.ToDate.Should().HaveMinute(expectedToDate.Minute);
            dateRange.ToDate.Should().HaveSecond(expectedToDate.Second);
            dateRange.ToDate.Offset.Should().Be(expectedToDate.Offset);
        }
        
        [TestMethod]
        public void SetsDefaultRangeAllUtc()
        {
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(new DateTime(2019,1,4,12,1,0,0, DateTimeKind.Utc));
            
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);
            
            var expectedFromDate = new DateTimeOffset(2019,1,4,12,1,0, new TimeSpan(0));
            var expectedToDate = new DateTimeOffset(2019,2,2,0,0,0,new TimeSpan(0));

            var dateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider);

            dateRange.FromDate.DateTime.Should().BeSameDateAs(expectedFromDate.DateTime);
            dateRange.FromDate.Should().HaveHour(expectedFromDate.Hour);
            dateRange.FromDate.Should().HaveMinute(expectedFromDate.Minute);
            dateRange.FromDate.Should().HaveSecond(expectedFromDate.Second);
            dateRange.FromDate.Offset.Should().Be(expectedFromDate.Offset);
            
            dateRange.ToDate.DateTime.Should().BeSameDateAs(expectedToDate.DateTime);
            dateRange.ToDate.Should().HaveHour(expectedToDate.Hour);
            dateRange.ToDate.Should().HaveMinute(expectedToDate.Minute);
            dateRange.ToDate.Should().HaveSecond(expectedToDate.Second);
            dateRange.ToDate.Offset.Should().Be(expectedToDate.Offset);
        }
        
        [TestMethod]
        public void SetsDefaultStartDateBstEndDateUtc()
        {
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(new DateTime(2019,10,20,12,1,0,0, DateTimeKind.Utc));
            
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);
            
            var expectedFromDate = new DateTimeOffset(2019,10,20,13,1,0, new TimeSpan(1,0,0));
            var expectedToDate = new DateTimeOffset(2019,11,18,0,0,0, new TimeSpan(0));

            var dateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider);

            dateRange.FromDate.DateTime.Should().BeSameDateAs(expectedFromDate.DateTime);
            dateRange.FromDate.Should().HaveHour(expectedFromDate.Hour);
            dateRange.FromDate.Should().HaveMinute(expectedFromDate.Minute);
            dateRange.FromDate.Should().HaveSecond(expectedFromDate.Second);
            dateRange.FromDate.Offset.Should().Be(expectedFromDate.Offset);
            
            dateRange.ToDate.DateTime.Should().BeSameDateAs(expectedToDate.DateTime);
            dateRange.ToDate.Should().HaveHour(expectedToDate.Hour);
            dateRange.ToDate.Should().HaveMinute(expectedToDate.Minute);
            dateRange.ToDate.Should().HaveSecond(expectedToDate.Second);
            dateRange.ToDate.Offset.Should().Be(expectedToDate.Offset);
        }
        
        [TestMethod]
        public void SetsDefaultRangeStartDateUtcEndDateBst()
        {
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(new DateTime(2019,3,3,0,0,0,0, DateTimeKind.Utc));
            
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);
            
            var expectedFromDate = new DateTimeOffset(2019,3,3,0,0,0, new TimeSpan(0));
            var expectedToDate = new DateTimeOffset(2019,4,1,0,0,0,new TimeSpan(1,0,0));

            var dateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider);

            dateRange.FromDate.DateTime.Should().BeSameDateAs(expectedFromDate.DateTime);
            dateRange.FromDate.Should().HaveHour(expectedFromDate.Hour);
            dateRange.FromDate.Should().HaveMinute(expectedFromDate.Minute);
            dateRange.FromDate.Should().HaveSecond(expectedFromDate.Second);
            dateRange.FromDate.Offset.Should().Be(expectedFromDate.Offset);
            
            dateRange.ToDate.DateTime.Should().BeSameDateAs(expectedToDate.DateTime);
            dateRange.ToDate.Should().HaveHour(expectedToDate.Hour);
            dateRange.ToDate.Should().HaveMinute(expectedToDate.Minute);
            dateRange.ToDate.Should().HaveSecond(expectedToDate.Second);
            dateRange.ToDate.Offset.Should().Be(expectedToDate.Offset);
        }
    }
}
