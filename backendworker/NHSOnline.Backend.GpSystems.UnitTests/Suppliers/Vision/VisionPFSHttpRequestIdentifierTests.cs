using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision
{
    [TestClass]
    public class VisionPFSHttpRequestIdentifierTests
    {
        private IFixture _fixture;
        private VisionPFSHttpRequestIdentifier _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<VisionPFSHttpRequestIdentifier>();
        }

        [TestMethod]
        public void HttpRequestIdentifier_ValidRequestWithHeader_ReturnsValidIdentifier()
        {
            var headerString = _fixture.Create<string>();
            var request = _fixture.Create<HttpRequestMessage>();
            request.Headers.Add(Constants.VisionConstants.RequestIdentifierHeader, headerString);
            
            var stringResponse = $"Provider=Vision UpStreamMethod={request.Method} UpStreamUrl={request.RequestUri} UpStreamIdentifier={headerString}";

            var result = _systemUnderTest.Identify(request);
            
            result.ToString().Should().Be(stringResponse);
        }
        
        [TestMethod]
        public void HttpRequestIdentifier_ValidRequestWithoutHeader_ReturnsValidIdentifier()
        {
            var request = _fixture.Create<HttpRequestMessage>();
            
            var stringResponse = $"Provider=Vision UpStreamMethod={request.Method} UpStreamUrl={request.RequestUri} UpStreamIdentifier=";

            var result = _systemUnderTest.Identify(request);
            
            result.ToString().Should().Be(stringResponse);
        }
    }
}