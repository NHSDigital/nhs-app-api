using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis
{
    [TestClass]
    public class EmisHttpRequestIdentifierTests
    {
        private IFixture _fixture;
        private EmisHttpRequestIdentifier _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<EmisHttpRequestIdentifier>();
        }

        [TestMethod]
        public void HttpRequestIdentifier_ValidRequest_ReturnsValidIdentifier()
        {
            var request = _fixture.Create<HttpRequestMessage>();

            var expectedResponse = new HttpRequestIdentity()
            {
                Provider = "Emis",
                Identifier = null,
                RequestUrl = request.RequestUri,
                Method = request.Method.ToString()
            };
            
            var stringResponse = $"Provider=Emis UpStreamMethod={request.Method} UpStreamUrl={request.RequestUri} UpStreamIdentifier= ";

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
                Provider = "Emis",
                Identifier = null,
                RequestUrl = null,
                Method = null
            };
            
            var stringResponse = "Provider=Emis UpStreamMethod= UpStreamUrl= UpStreamIdentifier= ";

            var result = _systemUnderTest.Identify(request);
            
            result.Should().BeEquivalentTo(expectedResponse);
            result.ToString().Should().Be(stringResponse);
        }
    }
}