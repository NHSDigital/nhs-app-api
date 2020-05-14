using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CorrelationId;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NHSOnline.Backend.ServiceJourneyRules.Common.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support.ResponseParsers;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.ServiceJourneyRules.Common.UnitTests
{
    [TestClass]
    public sealed class ServiceJourneyRulesClientTests : IDisposable
    {
        public static readonly Uri BaseUri = new Uri("http://base_url/");
        private const string ServiceJourneyRuleApiPath = "api/servicejourneyrules";

        private IServiceJourneyRulesClient _systemUnderTest;
        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<IServiceJourneyRulesConfig> _configMock;
        private HttpClient _httpClient;
        private ServiceJourneyRulesHttpClient _sjrHttpClient;
        private Mock<ILogger<ServiceJourneyRulesClient>> _mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockHttpHandler = new MockHttpMessageHandler();

            _configMock = new Mock<IServiceJourneyRulesConfig>();
            _configMock.SetupGet(x => x.ServiceJourneyRulesBaseUrl).Returns(BaseUri);

            _httpClient = new HttpClient(_mockHttpHandler);
            _sjrHttpClient = new ServiceJourneyRulesHttpClient(_httpClient, _configMock.Object);

            _mockLogger = new Mock<ILogger<ServiceJourneyRulesClient>>();

            _systemUnderTest = new ServiceJourneyRulesClient(
                _mockLogger.Object,
                _sjrHttpClient,
                new JsonResponseParser(),
                new CorrelationContextAccessor());
        }

        [TestMethod]
        public async Task GetServiceJourneyRules_ReturnsValidResponse()
        {
            // Arrange
            var expectedResponse =
                new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.OK);

            _mockHttpHandler
                .WhenServiceJourneyRules(HttpMethod.Get, ServiceJourneyRuleApiPath)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.GetServiceJourneyRules("Test");

            // Assert
            response.Should().BeOfType<ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>>()
                .Subject.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task GetServiceJourneyRules_ReturnsInValidResponse_ErrorNotFound()
        {
            // Arrange
            var expectedResponse = new ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>(HttpStatusCode.OK);

            _mockHttpHandler
                .WhenServiceJourneyRules(HttpMethod.Post, ServiceJourneyRuleApiPath)
                .Respond(HttpStatusCode.NotFound, System.Net.Mime.MediaTypeNames.Application.Json,
                    JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.GetServiceJourneyRules("Test");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _mockHttpHandler.Dispose();
        }
    }
}