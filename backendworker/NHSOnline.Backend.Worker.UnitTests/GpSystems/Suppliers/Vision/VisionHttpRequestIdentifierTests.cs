using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.Support.Http;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision
{
    [TestClass]
    public class VisionHttpRequestIdentifierTests
    {
        private IFixture _fixture;
        private VisionHttpRequestIdentifier _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<VisionHttpRequestIdentifier>();
        }

        [TestMethod]
        public void HttpRequestIdentifier_ValidRequestWithHeader_ReturnsValidIdentifier()
        {
            var headerString = _fixture.Create<string>();
            var request = _fixture.Create<HttpRequestMessage>();
            request.Headers.Add(Constants.VisionConstants.RequestIdentifierHeader, headerString);

            var expectedResponse = new HttpRequestIdentity()
            {
                Provider = "Vision",
                Identifier = headerString,
                RequestUrl = request.RequestUri,
                Method = request.Method.ToString()
            };
            
            var stringResponse = $"Provider:Vision - RequestMethod:{request.Method} - RequestUrl:{request.RequestUri} - RequestIdentifier:{headerString} ";

            var result = _systemUnderTest.Identify(request);
            
            result.Should().BeEquivalentTo(expectedResponse);
            result.ToString().Should().Be(stringResponse);
        }
        
        [TestMethod]
        public void HttpRequestIdentifier_ValidRequestWithoutHeader_ReturnsValidIdentifier()
        {
            var request = _fixture.Create<HttpRequestMessage>();

            var expectedResponse = new HttpRequestIdentity()
            {
                Provider = "Vision",
                Identifier = null,
                RequestUrl = request.RequestUri,
                Method = request.Method.ToString()
            };
            
            var stringResponse = $"Provider:Vision - RequestMethod:{request.Method} - RequestUrl:{request.RequestUri} - RequestIdentifier: ";

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
                Provider = "Vision",
                Identifier = null,
                RequestUrl = null,
                Method = null
            };
            
            var stringResponse = $"Provider:Vision - RequestMethod: - RequestUrl: - RequestIdentifier: ";

            var result = _systemUnderTest.Identify(request);
            
            result.Should().BeEquivalentTo(expectedResponse);
            result.ToString().Should().Be(stringResponse);
        }
    }
}