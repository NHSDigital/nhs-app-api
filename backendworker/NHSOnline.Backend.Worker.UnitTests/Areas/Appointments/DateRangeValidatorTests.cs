using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Appointments
{
    [TestClass]
    public class DateRangeValidatorTests
    {
        private IDateRangeValidator _dateRangeValidator;
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        
        [TestInitialize]
        public void TestInitialize()
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            _timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
            _dateRangeValidator = new DateRangeValidator(_dateTimeOffsetProvider);
        }

        [TestMethod]
        public void IsValid_ReturnsTrue_WhenFromDateAndToDateAreNull()
        {
            _dateRangeValidator.IsValid(null, null).Should().BeTrue();
        }
        
        [TestMethod]
        public void IsValid_ReturnsTrue_WhenFromDateIsBeforeToDate()
        {
            var fromDate = _dateTimeOffsetProvider.CreateDateTimeOffset();
            var toDate = fromDate.AddDays(12);
            
            _dateRangeValidator.IsValid(fromDate, toDate).Should().BeTrue();
        }
        
        [TestMethod]
        public void IsValid_ReturnsTrue_WhenFromDateIsSpecifiedOnly()
        {
            var fromDate = _dateTimeOffsetProvider.CreateDateTimeOffset();
 
            _dateRangeValidator.IsValid(fromDate, null).Should().BeTrue();
        }
        
        [TestMethod]
        public void IsValid_ReturnsTrue_WhenToDateIsSpecifiedOnly()
        {
            var toDate = _dateTimeOffsetProvider.CreateDateTimeOffset().AddDays(28);
 
            _dateRangeValidator.IsValid(null, toDate).Should().BeTrue();
        }
        
        [TestMethod]
        public void IsValid_ReturnsFalse_WhenFromdateAndToDateAreInThePast()
        {
            
            var fromdate = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-03-20T10:00:00");
            var toDate = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-03-29T10:00:00");
            
            _dateRangeValidator.IsValid(fromdate, toDate).Should().BeFalse();
        }
        
        [TestMethod]
        public void IsValid_ReturnsFalse_WhenFromDateIsAfterToDate()
        {
            var nowDate = _dateTimeOffsetProvider.CreateDateTimeOffset();
            var fromDate = nowDate.AddDays(14);
            var toDate = nowDate.AddDays(7);
 
            _dateRangeValidator.IsValid(fromDate, toDate).Should().BeFalse();
        }
    }
}
