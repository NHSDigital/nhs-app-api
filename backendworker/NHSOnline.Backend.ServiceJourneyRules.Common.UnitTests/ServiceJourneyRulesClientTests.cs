using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using CorrelationId;
using FluentAssertions;
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
        private ServiceJourneyRulesHttpClient _httpClient;
        private IFixture _fixture;
        private ICorrelationContextAccessor _correlationContext;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<IJsonResponseParser>(() => new JsonResponseParser());
            
            _mockHttpHandler = new MockHttpMessageHandler();

            _configMock = new Mock<IServiceJourneyRulesConfig>();
            _configMock.SetupGet(x => x.ServiceJourneyRulesBaseUrl).Returns(BaseUri);

            _httpClient = new ServiceJourneyRulesHttpClient(new HttpClient(_mockHttpHandler), _configMock.Object);
            
            _correlationContext = new CorrelationContextAccessor();

            _fixture.Inject(_configMock);
            _fixture.Inject(_httpClient);
            _fixture.Inject(_correlationContext);

            _systemUnderTest = _fixture.Create<ServiceJourneyRulesClient>();
        }
        
        [TestMethod]
        public async Task GetServiceJourneyRules_ReturnsValidResponse()
        {
            // Arrange
            var expectedResponse = _fixture.Create<ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>>();

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
            var expectedResponse = _fixture.Create<ServiceJourneyRulesApiObjectResponse<ServiceJourneyRulesResponse>>();
            
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
            _mockHttpHandler.Dispose();
        }
    }
}