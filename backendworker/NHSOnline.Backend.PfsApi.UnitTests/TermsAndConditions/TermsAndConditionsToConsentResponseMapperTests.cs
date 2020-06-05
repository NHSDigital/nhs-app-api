using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.TermsAndConditions;

namespace NHSOnline.Backend.PfsApi.UnitTests.TermsAndConditions
{
    [TestClass]
    public class TermsAndConditionsToConsentResponseMapperTests
    {
        private string _nhsLoginId;

        private TermsAndConditionsToConsentResponseMapper _systemUnderTest;
        private Mock<ITermsAndConditionsConfiguration> _mockTermsAndConditionsConfiguration;

        [TestInitialize]
        public void TestInitialize()
        {
            _nhsLoginId = "NhsLoginId";
            _mockTermsAndConditionsConfiguration = new Mock<ITermsAndConditionsConfiguration>();
            var logger = new Mock<ILogger<TermsAndConditionsService>>().Object;

            _systemUnderTest = new TermsAndConditionsToConsentResponseMapper(
                logger,
                _mockTermsAndConditionsConfiguration.Object);
        }

        [TestMethod]
        public void Map_UpdateNotNeeded()
        {
            // Arrange
            var consentTime = DateTimeOffset.Now.ToString("s", CultureInfo.InvariantCulture);

            var record = new TermsAndConditionsRecord()
            {
                AnalyticsCookieAccepted = false,
                ConsentGiven = true,
                DateOfAnalyticsCookieToggle = consentTime,
                DateOfConsent = consentTime,
                NhsLoginId = _nhsLoginId
            };

            // Act
            var result = _systemUnderTest.Map(record);

            // Assert
            result.Should().NotBeNull();
            result.ConsentGiven.Should().BeTrue();
            result.AnalyticsCookieAccepted.Should().BeFalse();
            result.UpdatedConsentRequired.Should().BeFalse();
        }

        [TestMethod]
        public void Map_UpdateNeeded()
        {
            // Arrange
            var record = new TermsAndConditionsRecord
            {
                NhsLoginId = _nhsLoginId,
                ConsentGiven = true,
                DateOfConsent = DateTimeOffset.Now.AddHours(-5).ToString("s", CultureInfo.InvariantCulture),
                AnalyticsCookieAccepted = true
            };

            _mockTermsAndConditionsConfiguration.SetupGet(x => x.EffectiveDate)
                .Returns(DateTimeOffset.Now.AddHours(-5));

            // Act
            var result = _systemUnderTest.Map(record);

            // Assert
            result.Should().NotBeNull();
            result.ConsentGiven.Should().BeTrue();
            result.AnalyticsCookieAccepted.Should().BeTrue();
            result.UpdatedConsentRequired.Should().BeTrue();
        }
    }
}
