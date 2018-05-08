using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Router.Validators;

namespace NHSOnline.Backend.Worker.UnitTests.Router.Validators
{
    [TestClass]
    public class PrescriptionRequestValidationServiceTests
    {
        private IPrescriptionRequestValidationService _prescriptionRequestValidationService;

        [TestInitialize]
        public void TestInitialize()
        {
            _prescriptionRequestValidationService = new PrescriptionRequestValidationService();
        }

        [TestMethod]
        public void IsValidFromDate_ReturnsFalse_WhenDateIsNull()
        {
            // Arrange
            DateTimeOffset? fromDate = null;
            var defaultFromDate = DateTimeOffset.Now.AddMonths(-6);

            // Act
            var result = _prescriptionRequestValidationService.IsValidFromDate(fromDate, defaultFromDate);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsValidFromDate_ReturnsFalse_WhenFromDateIsEarlierThanDefault()
        {
            // Arrange
            var defaultFromDate = new DateTime(2000, 1, 3);
            var fromDate = defaultFromDate.AddSeconds(-1);
            
            // Act
            var result = _prescriptionRequestValidationService.IsValidFromDate(fromDate, defaultFromDate);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsValidFromDate_ReturnsFalse_WhenFromDateIsLaterThanToday()
        {
            // Arrange
            var defaultFromDate = DateTimeOffset.Now.AddMonths(-6);
            var fromDate = DateTimeOffset.Now.AddDays(1); // to be sure to be in the future

            // Act
            var result = _prescriptionRequestValidationService.IsValidFromDate(fromDate, defaultFromDate);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsValidFromDate_ReturnsTrue_WhenFromDateIsValid()
        {
            // Arrange
            var defaultFromDate = DateTimeOffset.Now.AddDays(-2);
            var fromDate = DateTimeOffset.Now.AddDays(-1); // greater than default, but less than today

            // Act
            var result = _prescriptionRequestValidationService.IsValidFromDate(fromDate, defaultFromDate);

            // Assert
            result.Should().BeTrue();
        }
    }
}
