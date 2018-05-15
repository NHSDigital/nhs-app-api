using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments;
using NHSOnline.Backend.Worker.Date;
using NHSOnline.Backend.Worker.Router;

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
            _dateRangeValidator = new DateRangeValidator();
            _timeZoneInfoProvider = new TimeZoneInfoProvider();
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
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
            var toDate = _dateTimeOffsetProvider.CreateDateTimeOffset().AddDays(14);
 
            _dateRangeValidator.IsValid(null, toDate).Should().BeTrue();
        }
        
        [TestMethod]
        public void IsValid_ReturnsTrue_WhenFromdateAndToDateAreInThePast()
        {
            var fromdate = _dateTimeOffsetProvider.CreateDateTimeOffset(new DateTime(2018, 3, 20));
            var toDate = _dateTimeOffsetProvider.CreateDateTimeOffset(new DateTime(2018, 3, 29));
            
            _dateRangeValidator.IsValid(fromdate, toDate).Should().BeTrue();
        }
        
        [TestMethod]
        public void IsValid_ReturnsFalse_WhenFromDateIsAfterToDate()
        {
            var fromDate = _dateTimeOffsetProvider.CreateDateTimeOffset(new DateTime(2018, 3, 2));
            var toDate = fromDate.AddDays(-14);
 
            _dateRangeValidator.IsValid(fromDate, toDate).Should().BeFalse();
        }
    }
}
