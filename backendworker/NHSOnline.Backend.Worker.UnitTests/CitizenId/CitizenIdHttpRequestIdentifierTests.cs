using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.Support.Http;

namespace NHSOnline.Backend.Worker.UnitTests.CitizenId
{
    [TestClass]
    public class CitizenIdHttpRequestIdentifierTests
    {
        private IFixture _fixture;
        private CitizenIdHttpRequestIdentifier _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<CitizenIdHttpRequestIdentifier>();
        }

        [TestMethod]
        public void HttpRequestIdentifier_ValidRequest_ReturnsValidIdentifier()
        {
            var request = _fixture.Create<HttpRequestMessage>();

            var expectedResponse = new HttpRequestIdentity()
            {
                Provider = "CitizenId",
                Identifier = null,
                RequestUrl = request.RequestUri,
                Method = request.Method.ToString()
            };
            
            var stringResponse = $"Provider=CitizenId UpStreamMethod={request.Method} UpStreamUrl={request.RequestUri} UpStreamIdentifier= ";

            var result = _systemUnderTest.Identify(request);
            
            result.Should().BeEquivalentTo(expectedResponse);
            result.ToString().Should().Be(stringResponse);
        }
        
        [TestMethod]
        public void HttpRequestIdentifier_NullRequest_ReturnsValidIdentifier()
        {
            HttpRequestMessage request = null;

            var expectedResponse = new HttpRequestIdentity()
            {
                Provider = "CitizenId",
                Identifier = null,
                RequestUrl = null,
                Method = null
            };
            
            var stringResponse = "Provider=CitizenId UpStreamMethod= UpStreamUrl= UpStreamIdentifier= ";

            var result = _systemUnderTest.Identify(request);
            
            result.Should().BeEquivalentTo(expectedResponse);
            result.ToString().Should().Be(stringResponse);
        }
    }
}