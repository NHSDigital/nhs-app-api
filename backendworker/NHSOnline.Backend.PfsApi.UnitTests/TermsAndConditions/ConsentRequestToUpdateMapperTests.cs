using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson.Serialization;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;

namespace NHSOnline.Backend.PfsApi.UnitTests.TermsAndConditions
{
    [TestClass]
    public class ConsentRequestToUpdateMapperTests
    {
        private ConsentRequestToUpdateMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new ConsentRequestToUpdateMapper();
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
            var assertor = new UpdateDefinitionAssertor<TermsAndConditionsRecord>()
                .AddExpectedUpdate("ConsentGiven", "true")
                .AddExpectedUpdate("DateOfConsent", $"\"{expectedConsentTime}\"");

            // Act
            var result = _systemUnderTest.Map(request, consentTime);

            // Assert
            result.Should().NotBeNull();

            var updates = result.Build();

            assertor.Assert(updates);
        }
    }
}
