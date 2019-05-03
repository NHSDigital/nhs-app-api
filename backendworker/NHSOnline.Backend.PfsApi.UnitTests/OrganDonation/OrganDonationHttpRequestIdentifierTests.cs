using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UnitTests.OrganDonation
{
    [TestClass]
    public class OrganDonationHttpRequestIdentifierTests
    {
        private IFixture _fixture;
        private OrganDonationHttpRequestIdentifier _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<OrganDonationHttpRequestIdentifier>();
        }

        [TestMethod]
        public void HttpRequestIdentifier_ValidRequestWithHeaders_ReturnsValidIdentifierWithCorrelationIdentifier()
        {
            var sessionId = _fixture.Create<string>();
            var sequenceId = _fixture.Create<string>();
            var request = _fixture.Create<HttpRequestMessage>();
            request.Headers.Add(Constants.OrganDonationConstants.SessionIdHeaderKey, sessionId);
            request.Headers.Add(Constants.OrganDonationConstants.SequenceIdHeaderKey, sequenceId);
            
            var stringResponse = $"Provider=OrganDonation UpStreamMethod={request.Method} UpStreamUrl={request.RequestUri} " +
                                 $"CorrelationIdentifier={{\"SessionIdHeaderKey\":\"{sessionId}\",\"SequenceIdHeaderKey\":\"{sequenceId}\"}}";

            var result = _systemUnderTest.Identify(request);
            
            result.ToString().Should().Be(stringResponse);
        }
        
        [TestMethod]
        public void HttpRequestIdentifier_ValidRequestWithNoHeaders_ReturnsValidIdentifierWithCorrelationIdentifier()
        {
            var request = _fixture.Create<HttpRequestMessage>();
            
            var stringResponse = $"Provider=OrganDonation UpStreamMethod={request.Method} UpStreamUrl={request.RequestUri} " +
                                 $"CorrelationIdentifier={{\"SessionIdHeaderKey\":null,\"SequenceIdHeaderKey\":null}}";

            var result = _systemUnderTest.Identify(request);
            
            result.ToString().Should().Be(stringResponse);
        }
        
        [TestMethod]
        public void HttpRequestIdentifier_ValidGetRequest_ReturnsValidIdentifierWithoutCorrelationIdentifier()
        {
            var sessionId = _fixture.Create<string>();
            var sequenceId = _fixture.Create<string>();
            
            var request = _fixture.Create<HttpRequestMessage>();
            request.Headers.Add(Constants.OrganDonationConstants.SessionIdHeaderKey, sessionId);
            request.Headers.Add(Constants.OrganDonationConstants.SequenceIdHeaderKey, sequenceId);
            request.Method = HttpMethod.Get;
            
            var stringResponse = $"Provider=OrganDonation UpStreamMethod={request.Method} UpStreamUrl={request.RequestUri}";

            var result = _systemUnderTest.Identify(request);
            
            result.ToString().Should().Be(stringResponse);
        }
    }
}