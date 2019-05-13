using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.UnitTests.ServiceJourneyRules
{
    [TestClass]
    public class ServiceJourneyRulesHttpRequestIdentifierTests
    {
        private IFixture _fixture;
        private ServiceJourneyRulesHttpRequestIdentifier _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<ServiceJourneyRulesHttpRequestIdentifier>();
        }
        
        [TestMethod]
        public void HttpRequestIdentifier_ValidGetRequest_ReturnsHttpRequestIdentity()
        {
            var request = _fixture.Create<HttpRequestMessage>();
            var result = _systemUnderTest.Identify(request);
            
            result.Should().BeOfType(typeof(HttpRequestIdentity));
        }
    }
}