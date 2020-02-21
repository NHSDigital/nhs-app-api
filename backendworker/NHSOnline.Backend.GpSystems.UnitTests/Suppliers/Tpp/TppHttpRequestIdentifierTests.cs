using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
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
            // Arrange
            var headerString = _fixture.Create<string>();
            var request = _fixture.Create<HttpRequestMessage>();
            request.Headers.Add(Constants.TppConstants.RequestIdentifierHeader, headerString);
            
            var stringResponse = $"Provider=Tpp UpStreamMethod={request.Method} UpStreamUrl={request.RequestUri} UpStreamIdentifier={headerString}";

            // Act
            var result = _systemUnderTest.Identify(request);
            
            // Assert
            result.ToString().Should().Be(stringResponse);
        }
        
        [TestMethod]
        public void HttpRequestIdentifier_ValidRequestWithoutHeader_ReturnsValidIdentifier()
        {
            // Arrange
            var request = _fixture.Create<HttpRequestMessage>();
            
            var stringResponse = $"Provider=Tpp UpStreamMethod={request.Method} UpStreamUrl={request.RequestUri} UpStreamIdentifier=";

            // Act
            var result = _systemUnderTest.Identify(request);
            
            // Assert
            result.ToString().Should().Be(stringResponse);
        }
    }
}