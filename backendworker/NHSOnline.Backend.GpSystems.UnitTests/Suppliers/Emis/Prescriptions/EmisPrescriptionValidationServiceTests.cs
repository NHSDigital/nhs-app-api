using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Prescriptions
{
    [TestClass]
    public class EmisPrescriptionValidationServiceTests
    {
        private string SpecialRequestWith1Newlines = "Lorem ipsum dolor sit amet, \nconsectetur adipiscing elit.";

        private string FormattedSpecialRequestWith1Newlines = "Lorem ipsum dolor sit amet, \nconsectetur adipiscing elit.";

        private string SpecialRequest500CharsWith13Newlines = "Lorem ipsum dolor \nsit amet, \nconsectetur adipiscing elit. \nNulla feugiat commodo dolor, \nmaximus elementum turpis interdum quis. \nNunc lacinia, metus in vulputate interdum, \nelit magna placerat massa, \nnon dignissim lacus magna eget erat. \nInteger lobortis volutpat tristique. \nNulla semper tincidunt neque ac imperdiet. \nPraesent ipsum ante, \nimperdiet non purus eu, dapibus vulputate mi. \nNam eget aliquet felis. \nSuspendisse lacus tellus, egestas mattis vulputate eget, tincidunt nec justo. Quisq";

        private string FormattedSpecialRequest500CharsWith13Newlines = "Lorem ipsum dolor  sit amet,  consectetur adipiscing elit.  Nulla feugiat commodo dolor,  maximus elementum turpis interdum quis.  Nunc lacinia, metus in vulputate interdum,  elit magna placerat massa,  non dignissim lacus magna eget erat.  Integer lobortis volutpat tristique.  Nulla semper tincidunt neque ac imperdiet.  Praesent ipsum ante,  imperdiet non purus eu, dapibus vulputate mi.  Nam eget aliquet felis.  Suspendisse lacus tellus, egestas mattis vulputate eget, tincidunt nec justo. Quisq";

        private PrescriptionValidationService _prescriptionRequestValidationService;

        [TestInitialize]
        public void TestInitialize()
        {
            var logger = new Mock<ILogger<EmisPrescriptionValidationService>>();
            _prescriptionRequestValidationService = new EmisPrescriptionValidationService(logger.Object);
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
                },
                SpecialRequest = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.Quisque vestibulum.",
            };
            var userSession = new EmisUserSession { PrescriptionSpecialRequestNecessity = Necessity.Optional };

            // Act
            var result = _prescriptionRequestValidationService.IsPostValid(modelUnderTest, userSession);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsValidRepeatPrescriptionRequest_ReturnsFalse_WhenModelIsInValidType()
        {
            // Arrange
            var modelUnderTest = new RepeatPrescriptionRequest
            {
                CourseIds = new List<string>
                {
                    "310dbe9a-c002-4d6f-aeba-7ce6717da2aa",
                    "notAGuid",
                }
            };
            var userSession = new EmisUserSession { PrescriptionSpecialRequestNecessity = Necessity.Optional };

            // Act
            var result = _prescriptionRequestValidationService.IsPostValid(modelUnderTest, userSession);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsValidRepeatPrescriptionRequest_ReturnsFalse_WhenCourseIdsIsEmpty()
        {
            // Arrange
            var modelUnderTest = new RepeatPrescriptionRequest
            {
                CourseIds = new List<string>()
            };
            var userSession = new EmisUserSession { PrescriptionSpecialRequestNecessity = Necessity.Optional };

            // Act
            var result = _prescriptionRequestValidationService.IsPostValid(modelUnderTest, userSession);

            // Assert
            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(Necessity.Optional, 499, true)]
        [DataRow(Necessity.Optional, 500, true)]
        [DataRow(Necessity.Optional, 501, false)]
        public void IsValidRepeatPrescriptionRequest_ReturnsFalse_WhenSpecialRequestExceedsMaxLength(Necessity necessity, int specialRequestLength, bool isValid)
        {
            // Arrange
            var modelUnderTest = new RepeatPrescriptionRequest
            {
                CourseIds = new List<string>
                {
                    Guid.NewGuid().ToString(),
                },
                SpecialRequest = new string('x', specialRequestLength),
            };
            var userSession = new EmisUserSession { PrescriptionSpecialRequestNecessity = necessity };

            // Act
            var result = _prescriptionRequestValidationService.IsPostValid(modelUnderTest, userSession);

            // Assert
            result.Should().Be(isValid);
        }

        [TestMethod]
        public void IsValidRepeatPrescriptionRequest_ReturnsTrue_WhenNewlineConvertedToSpaces()
        {
            // Arrange
            var modelUnderTest = new RepeatPrescriptionRequest
            {
                SpecialRequest = SpecialRequestWith1Newlines
            };

            // Act
            var result = _prescriptionRequestValidationService.MassageSpecialRequest(modelUnderTest.SpecialRequest);

            // Assert
            result.Should().BeEquivalentTo(FormattedSpecialRequestWith1Newlines);
        }

        [TestMethod]
        public void IsValidRepeatPrescriptionRequest_ReturnsTrue_WhenSpecialRequest500CharsIncl13Newlines()
        {
            // Arrange
            var modelUnderTest = new RepeatPrescriptionRequest
            {
                SpecialRequest = SpecialRequest500CharsWith13Newlines
            };

            // Act
            var result = _prescriptionRequestValidationService.MassageSpecialRequest(modelUnderTest.SpecialRequest);

            // Assert
            result.Should().BeEquivalentTo(FormattedSpecialRequest500CharsWith13Newlines);
        }

        [TestMethod]
        public void IsValidRepeatPrescriptionRequest_ReturnsFalse_WhenSpecialRequestEmpty()
        {
            // Arrange
            var modelUnderTest = new RepeatPrescriptionRequest
            {
                CourseIds = new List<string>
                {
                    "310dbe9a-c002-4d6f-aeba-7ce6717da2aa",
                    "28985814-bb40-4b37-8f1d-c5f323b098da",
                },
                SpecialRequest = "",
            };
            var userSession = new EmisUserSession { PrescriptionSpecialRequestNecessity = Necessity.Mandatory };

            // Act
            var result = _prescriptionRequestValidationService.IsPostValid(modelUnderTest, userSession);

            // Assert
            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(Necessity.NotAllowed, "This is a special request", false)]
        [DataRow(Necessity.NotAllowed, null, true)]
        [DataRow(Necessity.NotAllowed, "", false)]
        [DataRow(Necessity.Optional, "This is a special request", true)]
        [DataRow(Necessity.Optional, "", true)]
        [DataRow(Necessity.Optional, null, true)]
        [DataRow(Necessity.Mandatory, "", false)]
        [DataRow(Necessity.Mandatory, "This is a special request", true)]
        [DataRow(Necessity.Mandatory, null, false)]
        public void IsPostValid_ReturnsTrue_WhenSpecialRequestSatisfiesNecessity(Necessity necessity, string specialRequest, bool isValid)
        {
            // Arrange
            var modelUnderTest = new RepeatPrescriptionRequest
            {
                CourseIds = new List<string>
                {
                    Guid.NewGuid().ToString(),
                },
                SpecialRequest = specialRequest
            };
            var userSession = new EmisUserSession { PrescriptionSpecialRequestNecessity = necessity };

            // Act
            var result = _prescriptionRequestValidationService.IsPostValid(modelUnderTest, userSession);

            // Assert
            result.Should().Be(isValid);
        }
    }
}
