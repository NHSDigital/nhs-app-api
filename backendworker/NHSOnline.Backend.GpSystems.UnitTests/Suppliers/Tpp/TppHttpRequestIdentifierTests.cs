using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    [TestClass]
    public class TppHttpRequestIdentifierTests
    {
        private IFixture _fixture;
        private TppHttpRequestIdentifier _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<TppHttpRequestIdentifier>();
        }

        [TestMethod]
        public void HttpRequestIdentifier_ValidRequestWithHeader_ReturnsValidIdentifier()
        {
            var headerString = _fixture.Create<string>();
            var request = _fixture.Create<HttpRequestMessage>();
            request.Headers.Add(Constants.TppConstants.RequestIdentifierHeader, headerString);

            var expectedResponse = new HttpRequestIdentity()
            {
                Provider = "Tpp",
                Identifier = headerString,
                RequestUrl = request.RequestUri,
                Method = request.Method.ToString()
            };
            
            var stringResponse = $"Provider=Tpp UpStreamMethod={request.Method} UpStreamUrl={request.RequestUri} UpStreamIdentifier={headerString} ";

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
                Provider = "Tpp",
                Identifier = null,
                RequestUrl = request.RequestUri,
                Method = request.Method.ToString()
            };
            
            var stringResponse = $"Provider=Tpp UpStreamMethod={request.Method} UpStreamUrl={request.RequestUri} UpStreamIdentifier= ";

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
                Provider = "Tpp",
                Identifier = null,
                RequestUrl = null,
                Method = null
            };
            
            var stringResponse = "Provider=Tpp UpStreamMethod= UpStreamUrl= UpStreamIdentifier= ";

            var result = _systemUnderTest.Identify(request);
            
            result.Should().BeEquivalentTo(expectedResponse);
            result.ToString().Should().Be(stringResponse);
        }
    }
}