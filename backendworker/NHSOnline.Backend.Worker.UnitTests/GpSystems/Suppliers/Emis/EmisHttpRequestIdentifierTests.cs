using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.Support.Http;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis
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
            
            var stringResponse = $"Provider:Emis - RequestMethod:{request.Method} - RequestUrl:{request.RequestUri} - RequestIdentifier: ";

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
            
            var stringResponse = $"Provider:Emis - RequestMethod: - RequestUrl: - RequestIdentifier: ";

            var result = _systemUnderTest.Identify(request);
            
            result.Should().BeEquivalentTo(expectedResponse);
            result.ToString().Should().Be(stringResponse);
        }
    }
}