using System.Net.Http;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.ServiceJourneyRules.Common.UnitTests
{
    [TestClass]
    public class ServiceJourneyRulesHttpRequestIdentifierTests
    {
        private ServiceJourneyRulesHttpRequestIdentifier _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new ServiceJourneyRulesHttpRequestIdentifier();
        }

        [TestMethod]
        public void HttpRequestIdentifier_ValidGetRequest_ReturnsHttpRequestIdentity()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Get, "https://www.foo.com");

            // Act
            var result = _systemUnderTest.Identify(request);

            // Assert
            result.Should().BeOfType(typeof(HttpRequestIdentity));
        }
    }
}