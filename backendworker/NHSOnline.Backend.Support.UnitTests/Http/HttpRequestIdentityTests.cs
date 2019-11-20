using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.Support.UnitTests.Http
{
    [TestClass]
    public class HttpRequestIdentityTests
    {
        private IFixture _fixture;
        private HttpRequestIdentity _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        [DataRow("TPP", SourceApi.Tpp)]
        [DataRow("Emis", SourceApi.Emis)]
        [DataRow("Vision", SourceApi.Vision)]
        [DataRow("Microtest", SourceApi.Microtest)]
        [DataRow("OrganDonation", SourceApi.OrganDonation)]
        [DataRow("ServiceJourneyRules", SourceApi.ServiceJourneyRules)]
        [DataRow("CitizenId", SourceApi.NhsLogin)]
        public void ToString_DefaultInstance_HappyPath(string provider, SourceApi sourceApi)
        {
            var requestMessage = _fixture.Create<HttpRequestMessage>();

            _systemUnderTest = new HttpRequestIdentity(provider, requestMessage, sourceApi);

            var result = _systemUnderTest.ToString();

            result.Should().ContainAll(
                $"Provider={provider}",
                $"UpStreamMethod={requestMessage.Method}",
                $"UpStreamUrl={requestMessage.RequestUri}");

            result.Should().NotContainAny("UpStreamIdentifier", "CorrelationIdentifier", "OlcSessionId");
        }

        [TestMethod]
        [DataRow("TPP", SourceApi.Tpp)]
        [DataRow("Emis", SourceApi.Emis)]
        [DataRow("Vision", SourceApi.Vision)]
        [DataRow("Microtest", SourceApi.Microtest)]
        [DataRow("OrganDonation", SourceApi.OrganDonation)]
        [DataRow("ServiceJourneyRules", SourceApi.ServiceJourneyRules)]
        [DataRow("CitizenId", SourceApi.NhsLogin)]
        public void ToString_WithIdentifier_HappyPath(string provider, SourceApi sourceApi)
        {
            var requestMessage = _fixture.Create<HttpRequestMessage>();
            var identifier = _fixture.Create<string>();

            _systemUnderTest = new HttpRequestIdentity(provider, requestMessage, sourceApi)
                .SetUpStreamIdentifier(identifier);

            var result = _systemUnderTest.ToString();

            result.Should().ContainAll(
                $"Provider={provider}",
                $"UpStreamMethod={requestMessage.Method}",
                $"UpStreamUrl={requestMessage.RequestUri}",
                $"UpStreamIdentifier={identifier}");

            result.Should().NotContainAny("CorrelationIdentifier", "OlcSessionId");
        }

        [TestMethod]
        [DataRow("TPP", SourceApi.Tpp)]
        [DataRow("Emis", SourceApi.Emis)]
        [DataRow("Vision", SourceApi.Vision)]
        [DataRow("Microtest", SourceApi.Microtest)]
        [DataRow("OrganDonation", SourceApi.OrganDonation)]
        [DataRow("ServiceJourneyRules", SourceApi.ServiceJourneyRules)]
        [DataRow("CitizenId", SourceApi.NhsLogin)]
        public void ToString_WithCorrelationIdentifier_HappyPath(string provider, SourceApi sourceApi)
        {
            var requestMessage = _fixture.Create<HttpRequestMessage>();
            var correlationIdentifier = _fixture.Create<string>();

            _systemUnderTest = new HttpRequestIdentity(provider, requestMessage, sourceApi)
                .SetCorrelationIdentifier(correlationIdentifier);

            var result = _systemUnderTest.ToString();

            result.Should().ContainAll(
                $"Provider={provider}",
                $"UpStreamMethod={requestMessage.Method}",
                $"UpStreamUrl={requestMessage.RequestUri}",
                $"CorrelationIdentifier={correlationIdentifier}");

            result.Should().NotContainAny("UpStreamIdentifier", "OlcSessionId");
        }

        [TestMethod]
        [DataRow("TPP", SourceApi.Tpp)]
        [DataRow("Emis", SourceApi.Emis)]
        [DataRow("Vision", SourceApi.Vision)]
        [DataRow("Microtest", SourceApi.Microtest)]
        [DataRow("OrganDonation", SourceApi.OrganDonation)]
        [DataRow("ServiceJourneyRules", SourceApi.ServiceJourneyRules)]
        [DataRow("CitizenId", SourceApi.NhsLogin)]
        public void ToString_WithCorrelationIdentifierAndIdentifier_HappyPath(string provider, SourceApi sourceApi)
        {
            var requestMessage = _fixture.Create<HttpRequestMessage>();
            var correlationIdentifier = _fixture.Create<string>();
            var identifier = _fixture.Create<string>();

            _systemUnderTest = new HttpRequestIdentity(provider, requestMessage, sourceApi)
                .SetCorrelationIdentifier(correlationIdentifier)
                .SetUpStreamIdentifier(identifier);

            var result = _systemUnderTest.ToString();

            result.Should().ContainAll(
                $"Provider={provider}",
                $"UpStreamMethod={requestMessage.Method}",
                $"UpStreamUrl={requestMessage.RequestUri}",
                $"CorrelationIdentifier={correlationIdentifier}",
                $"UpStreamIdentifier={identifier}");

            result.Should().NotContainAny("OlcSessionId");
        }

        [TestMethod]
        public void ToString_WithOlcSessionId_HappyPath()
        {
            var requestMessage = _fixture.Create<HttpRequestMessage>();
            var provider = _fixture.Create<string>();
            var olcSessionId = _fixture.Create<string>();

            _systemUnderTest = new HttpRequestIdentity(provider, olcSessionId, requestMessage, SourceApi.OnlineConsultations);

            var result = _systemUnderTest.ToString();

            result.Should().ContainAll(
                $"Provider={provider}",
                $"UpStreamMethod={requestMessage.Method}",
                $"UpStreamUrl={requestMessage.RequestUri}",
                $"OlcSessionId={olcSessionId}");

            result.Should().NotContainAny( "CorrelationIdentifier", "UpStreamIdentifier");
        }

        [TestMethod]
        public void ToString_WithOlcSessionIdAndCorrelationIdAndIdentifier_HappyPath()
        {
            var requestMessage = _fixture.Create<HttpRequestMessage>();
            var provider = _fixture.Create<string>();
            var olcSessionId = _fixture.Create<string>();
            var correlationIdentifier = _fixture.Create<string>();
            var identifier = _fixture.Create<string>();

            _systemUnderTest = new HttpRequestIdentity(provider, olcSessionId, requestMessage, SourceApi.OnlineConsultations)
                .SetCorrelationIdentifier(correlationIdentifier)
                .SetUpStreamIdentifier(identifier);

            var result = _systemUnderTest.ToString();

            result.Should().ContainAll(
                $"Provider={provider}",
                $"UpStreamMethod={requestMessage.Method}",
                $"UpStreamUrl={requestMessage.RequestUri}",
                $"OlcSessionId={olcSessionId}",
                $"CorrelationIdentifier={correlationIdentifier}",
                $"UpStreamIdentifier={identifier}");
        }
    }
}