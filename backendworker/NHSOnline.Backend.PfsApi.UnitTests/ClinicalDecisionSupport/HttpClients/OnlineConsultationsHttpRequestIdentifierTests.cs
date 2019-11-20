using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.UnitTests.ClinicalDecisionSupport.HttpClients
{
    [TestClass]
    public class OnlineConsultationsHttpRequestIdentifierTests
    {
        private IFixture _fixture;
        private OnlineConsultationsHttpRequestIdentifier _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<OnlineConsultationsHttpRequestIdentifier>();
        }

        [TestMethod]
        public void HttpRequestIdentifier_ValidGetRequest_ReturnsHttpRequestIdentity()
        {
            // Arrange
            var request = _fixture.Create<HttpRequestMessage>();
            
            // Act
            var result = _systemUnderTest.Identify(request);

            // Assert
            result.Should().BeOfType(typeof(HttpRequestIdentity));
        }

        [TestMethod]
        public void HttpRequestIdentifier_RequestHeaderContainsOlcProviderIdentifier_ContainsProvider()
        {
            // Arrange
            var request = _fixture.Create<HttpRequestMessage>();
            var provider = _fixture.Create<string>();
            
            request.Headers.Add(Support.Constants.OnlineConsultationConstants.ProviderIdentifierHeader, provider);
            
            // Act
            var result = _systemUnderTest.Identify(request);

            // Assert
            result.ToString().Should().Contain($"Provider={provider}");
        }

        [TestMethod]
        public void HttpRequestIdentifier_RequestHeaderContainsOlcSessionIdentifier_ContainsOlcSessionId()
        {
            // Arrange
            var request = _fixture.Create<HttpRequestMessage>();
            var sessionId = _fixture.Create<string>();
            
            request.Headers.Add(Support.Constants.OnlineConsultationConstants.SessionIdentifierHeader, sessionId);
            
            // Act
            var result = _systemUnderTest.Identify(request);

            // Assert
            result.ToString().Should().Contain($"OlcSessionId={sessionId}");
        }
    }
}