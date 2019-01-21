using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Prescriptions
{
    [TestClass]
    public class TppPrescriptionValidationServiceTests
    {
        private PrescriptionValidationService _prescriptionRequestValidationService;

        [TestInitialize]
        public void TestInitialize()
        {
            Mock<ILogger<TppPrescriptionValidationService>> logger = new Mock<ILogger<TppPrescriptionValidationService>>();
            _prescriptionRequestValidationService = new TppPrescriptionValidationService(logger.Object);
        }

        [TestMethod]
        public void IsValidFromDate_ReturnsFalse_WhenDateIsNull()
        {
            // Arrange
            DateTimeOffset? fromDate = null;
            var defaultFromDate = DateTimeOffset.Now.AddMonths(-6);

            // Act
            var result = _prescriptionRequestValidationService.IsGetValid(fromDate, defaultFromDate);

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
            var result = _prescriptionRequestValidationService.IsGetValid(fromDate, defaultFromDate);

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
            var result = _prescriptionRequestValidationService.IsGetValid(fromDate, defaultFromDate);

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
            var result = _prescriptionRequestValidationService.IsGetValid(fromDate, defaultFromDate);

            // Assert
            result.Should().BeTrue();
        }

        // Valid RepeatPrescriptionRequest

        [TestMethod]
        public void IsValidRepeatPrescriptionRequest_ReturnsTrue_WhenModelIsValidType()
        {
            // Arrange
            var modelUnderTest = new RepeatPrescriptionRequest
            {
                CourseIds = new List<string>
                {
                    "310dbe9a-c002-4d6f-aeba-7ce6717da2aa",
                    "28985814-bb40-4b37-8f1d-c5f323b098da",
                }               
            };

            // Act
            var result = _prescriptionRequestValidationService.IsPostValid(modelUnderTest);

            // Assert
            result.Should().BeTrue();
        }
        
        [TestMethod]
        public void IsValidRepeatPrescriptionRequest_ReturnsTrue_WhenModelIsNotAGuid()
        {
            // Arrange
            var modelUnderTest = new RepeatPrescriptionRequest
            {
                CourseIds = new List<string>
                {
                    "anyString",
                    "notAGuidIsAllowed",
                }               
            };

            // Act
            var result = _prescriptionRequestValidationService.IsPostValid(modelUnderTest);

            // Assert
            result.Should().BeTrue();
        }
        
        [TestMethod]
        public void IsValidRepeatPrescriptionRequest_ReturnsFalse_WhenCourseIdsIsEmpty()
        {
            // Arrange
            var modelUnderTest = new RepeatPrescriptionRequest
            {
                CourseIds = new List<string>()  
            };

            // Act
            var result = _prescriptionRequestValidationService.IsPostValid(modelUnderTest);

            // Assert
            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(999, true)]
        [DataRow(1000, true)]
        [DataRow(1001, false)]
        public void IsValidRepeatPrescriptionRequest_ReturnsFalse_WhenSpecialRequestExceedsMaxLength(int specialRequestLength, bool isValid)
        {
            // Arrange
            var modelUnderTest = new RepeatPrescriptionRequest
            {
                CourseIds = new List<string>
                {
                    Guid.NewGuid().ToString(),
                },
                SpecialRequest = new String('x', specialRequestLength),
            };

            // Act
            var result = _prescriptionRequestValidationService.IsPostValid(modelUnderTest);

            // Assert
            result.Should().Be(isValid);
        }
    }
}
