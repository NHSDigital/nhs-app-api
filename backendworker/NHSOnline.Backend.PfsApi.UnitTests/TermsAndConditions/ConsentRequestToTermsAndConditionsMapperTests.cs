using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;

namespace NHSOnline.Backend.PfsApi.UnitTests.TermsAndConditions
{
    [TestClass]
    public class ConsentRequestToTermsAndConditionsMapperTests
    {
        private ConsentRequestToTermsAndConditionsMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new ConsentRequestToTermsAndConditionsMapper();
        }

        [TestMethod]
        public void Map()
        {
            // Arrange
            var expectedConsentTime = "2020-06-08T15:26:28";
            var consentTime = DateTimeOffset.Parse(expectedConsentTime, CultureInfo.InvariantCulture);

            var request = new ConsentRequest
            {
                AnalyticsCookieAccepted = false,
                ConsentGiven = true,
                UpdatingConsent = false

            };

            var expectedRecord = new TermsAndConditionsRecord
            {
                AnalyticsCookieAccepted = false,
                ConsentGiven = true,
                DateOfAnalyticsCookieToggle = expectedConsentTime,
                DateOfConsent = expectedConsentTime,
                NhsLoginId = "NhsLoginId"
            };

            // Act
            var result = _systemUnderTest.Map(request, consentTime, "NhsLoginId");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedRecord);
        }
    }
}
