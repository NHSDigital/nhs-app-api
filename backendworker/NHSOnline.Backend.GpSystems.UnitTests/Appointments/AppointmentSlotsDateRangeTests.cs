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

        private const int EightWeeksInDays = 8 * 7;
        private const int SixteenWeeksInDays = 16 * 7;

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
        public void Constructor_TakesDateTimeOffsetProviderAndSixteenWeeksIsEnabled_SetsSixteenWeekRangeAllBst()
        {
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(new DateTime(2019,7,20,12,1,0,0, DateTimeKind.Utc));

            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);

            var expectedFromDate = new DateTimeOffset(2019,7,20,13,1,0, new TimeSpan(1,0,0));
            var expectedToDate = new DateTimeOffset(2019,11,10,0,0,0, new TimeSpan(0));

            var dateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider, true);

            ValidateDateRange(dateRange, expectedFromDate, expectedToDate, SixteenWeeksInDays);
        }

        [TestMethod]
        public void Constructor_TakesDateTimeOffsetProviderSixteenWeeksIsDisabled_SetsEightWeekRangeAllBst()
        {
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(new DateTime(2019,7,20,12,1,0,0, DateTimeKind.Utc));

            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);

            var expectedFromDate = new DateTimeOffset(2019,7,20,13,1,0, new TimeSpan(1,0,0));
            var expectedToDate = new DateTimeOffset(2019,9,15,0,0,0, new TimeSpan(1,0,0));

            var dateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider, false);

            ValidateDateRange(dateRange, expectedFromDate, expectedToDate, EightWeeksInDays);
        }

        [TestMethod]
        public void Constructor_TakesDateTimeOffsetProviderSixteenWeeksIsEnabled_SetsSixteenWeekRangeAllUtc()
        {
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(new DateTime(2019,1,4,12,1,0,0, DateTimeKind.Utc));

            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);

            var expectedFromDate = new DateTimeOffset(2019,1,4,12,1,0, new TimeSpan(0));
            var expectedToDate = new DateTimeOffset(2019,4,27,0,0,0,new TimeSpan(1,0,0));

            var dateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider, true);

            ValidateDateRange(dateRange, expectedFromDate, expectedToDate, SixteenWeeksInDays);
        }

        [TestMethod]
        public void Constructor_TakesDateTimeOffsetProviderSixteenWeeksIsDisabled_SetsEightWeekRangeAllUtc()
        {
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(new DateTime(2019,1,4,12,1,0,0, DateTimeKind.Utc));

            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);

            var expectedFromDate = new DateTimeOffset(2019,1,4,12,1,0, new TimeSpan(0));
            var expectedToDate = new DateTimeOffset(2019,3,2,0,0,0,new TimeSpan(0));

            var dateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider, false);

            ValidateDateRange(dateRange, expectedFromDate, expectedToDate, EightWeeksInDays);
        }

        [TestMethod]
        public void Constructor_TakesDateTimeOffsetProviderSixteenWeeksIsEnabled_SetsSixteenWeekRangeStartDateBstEndDateUtc()
        {
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(new DateTime(2019,10,20,12,1,0,0, DateTimeKind.Utc));

            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);

            var expectedFromDate = new DateTimeOffset(2019,10,20,13,1,0, new TimeSpan(1,0,0));
            var expectedToDate = new DateTimeOffset(2020,2,10,0,0,0, new TimeSpan(0));

            var dateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider, true);

            ValidateDateRange(dateRange, expectedFromDate, expectedToDate, SixteenWeeksInDays);
        }

        [TestMethod]
        public void Constructor_TakesDateTimeOffsetProviderSixteenWeeksIsDisabled_SetsEightWeekRangeStartDateBstEndDateUtc()
        {
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(new DateTime(2019,10,20,12,1,0,0, DateTimeKind.Utc));

            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);

            var expectedFromDate = new DateTimeOffset(2019,10,20,13,1,0, new TimeSpan(1,0,0));
            var expectedToDate = new DateTimeOffset(2019,12,16,0,0,0, new TimeSpan(0));

            var dateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider, false);

            ValidateDateRange(dateRange, expectedFromDate, expectedToDate, EightWeeksInDays);
        }

        [TestMethod]
        public void Constructor_TakesDateTimeOffsetProviderSixteenWeeksIsEnabled_SetsSixteenWeekRangeStartDateUtcEndDateBst()
        {
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(new DateTime(2019,3,3,0,0,0,0, DateTimeKind.Utc));

            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);

            var expectedFromDate = new DateTimeOffset(2019,3,3,0,0,0, new TimeSpan(0));
            var expectedToDate = new DateTimeOffset(2019,6,24,0,0,0,new TimeSpan(1,0,0));

            var dateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider, true);

            ValidateDateRange(dateRange, expectedFromDate, expectedToDate, SixteenWeeksInDays);
        }

        [TestMethod]
        public void Constructor_TakesDateTimeOffsetProviderSixteenWeeksIsDisabled_SetsEightWeekRangeStartDateUtcEndDateBst()
        {
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(new DateTime(2019,3,3,0,0,0,0, DateTimeKind.Utc));

            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);

            var expectedFromDate = new DateTimeOffset(2019,3,3,0,0,0, new TimeSpan(0));
            var expectedToDate = new DateTimeOffset(2019,4,29,0,0,0,new TimeSpan(1,0,0));

            var dateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider, false);

            ValidateDateRange(dateRange, expectedFromDate, expectedToDate, EightWeeksInDays);
        }

        [TestMethod]
        public void Constructor_TestConstructor_SetsPropertiesEquivalentToTheStandardConstructor()
        {
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(new DateTime(2019,3,3,0,0,0,0, DateTimeKind.Utc));

            var dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);

            var regularConstructedDateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider, false);

            var testConstructedDateRange = new AppointmentSlotsDateRange(regularConstructedDateRange.FromDate, regularConstructedDateRange.ToDate);

            testConstructedDateRange.FromDate.Should().Be(regularConstructedDateRange.FromDate);
            testConstructedDateRange.ToDate.Should().Be(regularConstructedDateRange.ToDate);
            testConstructedDateRange.DayRange.Should().Be(regularConstructedDateRange.DayRange);
        }

        [TestMethod]
        public void Constructor_TestConstructor_SetsPropertiesAsExpected()
        {
            var fromDate = new DateTime(2020,2,6);
            var toDate = new DateTime(2020,2,10);

            var dateRange = new AppointmentSlotsDateRange(fromDate, toDate);

            dateRange.FromDate.Should().Be(fromDate);
            dateRange.ToDate.Should().Be(toDate);
            dateRange.DayRange.Should().Be(3);
        }

        private void ValidateDateRange(AppointmentSlotsDateRange dateRange, DateTimeOffset expectedFromDate, DateTimeOffset expectedToDate, int expectedDayRange)
        {
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

            dateRange.DayRange.Should().Be(expectedDayRange);
        }
    }
}
