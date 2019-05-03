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
            
            var stringResponse = $"Provider=Emis UpStreamMethod={request.Method} UpStreamUrl={request.RequestUri}";

            var result = _systemUnderTest.Identify(request);
            
            result.ToString().Should().Be(stringResponse);
        }
    }
}