using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Auth.CitizenId;

namespace NHSOnline.Backend.Auth.UnitTests.CitizenId
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

            var stringResponse = $"Provider=CitizenId UpStreamMethod={request.Method} UpStreamUrl={request.RequestUri}";

            var result = _systemUnderTest.Identify(request);

            result.ToString().Should().Be(stringResponse);
        }
    }
}