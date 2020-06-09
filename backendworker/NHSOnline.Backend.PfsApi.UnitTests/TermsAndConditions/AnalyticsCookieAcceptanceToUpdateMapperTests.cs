using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Repository.UnitTests;

namespace NHSOnline.Backend.PfsApi.UnitTests.TermsAndConditions
{
    [TestClass]
    public class AnalyticsCookieAcceptanceToUpdateMapperTests
    {
        private AnalyticsCookieAcceptanceToUpdateMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new AnalyticsCookieAcceptanceToUpdateMapper();
        }

        [TestMethod]
        public void Map()
        {
            // Arrange
            var expectedConsentTime = "2020-06-08T15:26:28";
            var consentTime = DateTimeOffset.Parse(expectedConsentTime, CultureInfo.InvariantCulture);
            var request = new AnalyticsCookieAcceptance
            {
                AnalyticsCookieAccepted = false,
            };

            var assertor = new UpdateDefinitionAssertor<TermsAndConditionsRecord>()
                .AddExpectedUpdate("AnalyticsCookieAccepted", "false")
                .AddExpectedUpdate("DateOfAnalyticsCookieToggle", $"\"{expectedConsentTime}\"");

            // Act
            var result = _systemUnderTest.Map(request, consentTime);

            // Assert
            result.Should().NotBeNull();

            var updates = result.Build();

           assertor.Assert(updates);
        }
    }
}
