using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Settings;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.PfsApi.UnitTests.ClinicalDecisionSupport.HttpClients
{
    [TestClass]
    public sealed class OnlineConsultationsProviderHttpClientTests : IDisposable
    {
        private Mock<ILogger<OnlineConsultationsProviderHttpClient>> _mockLogger;

        private MockHttpMessageHandler _mockHttpMessageHandler;
        private HttpClient _httpClient;

        private OnlineConsultationsProviderHttpClient _onlineConsultationsProviderHttpClient;

        private const string ServiceDefinitionId = "testId";
        private const string MockedProviderResponse = "{\"test\": \"value\"}";
        private const string MockedEvaluateRequestBody = "{\"request\": \"body\"}";
        private string _searchServiceDefinitionPath;

        private bool addJsDisabledHeader;
        
        private readonly OnlineConsultationsProviderSettings TestProviderSettings =
            new OnlineConsultationsProviderSettings
            {
                Provider = "eConsult",
                BaseAddress = "http://test.test/test/",
                BearerToken = "testBearerToken"
            };

        [TestInitialize]
        public void TestInitialize()
        {
            _mockHttpMessageHandler = new MockHttpMessageHandler();
            _httpClient = new HttpClient(_mockHttpMessageHandler);
            _mockLogger = new Mock<ILogger<OnlineConsultationsProviderHttpClient>>();

            _onlineConsultationsProviderHttpClient = new OnlineConsultationsProviderHttpClient(
                _httpClient,
                TestProviderSettings,
                _mockLogger.Object);

            _searchServiceDefinitionPath =
                TestProviderSettings.BaseAddress + Constants.CdsApiEndpoints.ServiceDefinitionPath;
        }

        [TestMethod]
        public void OnlineConsultationsProviderHttpClient_WhenCreated_ConfiguresHttpClientBasedOnProviderSettings()
        {
            // Assert
            _httpClient.BaseAddress.Should().Be(TestProviderSettings.BaseAddress);
        }

        [TestMethod]
        public async Task GetServiceDefinitionById_WhenInvoked_ProxiesRequestToProvider()
        {
            // Arrange
            _mockHttpMessageHandler
                .When(HttpMethod.Get, GetFormattedUrl(Constants.CdsApiEndpoints.GetServiceDefinitionByIdPathFormat, ServiceDefinitionId))
                .With(ValidateMockRequestHeaders)
                .Respond(HttpStatusCode.OK, Constants.ContentTypes.ApplicationJsonFhir, MockedProviderResponse);

            // Act
            var response = await _onlineConsultationsProviderHttpClient.GetServiceDefinitionById(ServiceDefinitionId);

            // Assert
            (await response.Content.ReadAsStringAsync()).Should().BeEquivalentTo(MockedProviderResponse);
        }

        [TestMethod]
        public async Task SearchServiceDefinitionsByQuery_WhenInvoked_ProxiesRequestToProvider()
        {
            // Arrange
            _mockHttpMessageHandler
                .When(HttpMethod.Get, _searchServiceDefinitionPath)
                .With(ValidateMockRequestHeaders)
                .Respond(HttpStatusCode.OK, Constants.ContentTypes.ApplicationJsonFhir, MockedProviderResponse);
            
            // Act
            var response = await _onlineConsultationsProviderHttpClient.SearchServiceDefinitionsByQuery();
            
            // Assert
            (await response.Content.ReadAsStringAsync()).Should().BeEquivalentTo(MockedProviderResponse);
        }

        [TestMethod]
        public async Task EvaluateServiceDefinition_WhenInvoked_ProxiesRequestToProvider()
        {
            // Arrange
            _mockHttpMessageHandler
                .When(HttpMethod.Post, GetFormattedUrl(Constants.CdsApiEndpoints.EvaluateServiceDefinitionPathFormat, ServiceDefinitionId))
                .With(ValidateMockRequestHeaders)
                .With(ValidateMockEvaluateRequestBody)
                .Respond(HttpStatusCode.OK, Constants.ContentTypes.ApplicationJsonFhir, MockedProviderResponse);
            
            // Act
            var response = await _onlineConsultationsProviderHttpClient.EvaluateServiceDefinition(ServiceDefinitionId, MockedEvaluateRequestBody, false);
            
            // Assert
            (await response.Content.ReadAsStringAsync()).Should().BeEquivalentTo(MockedProviderResponse);
        }

        [TestMethod]
        public async Task EvaluateServiceDefinition_WhenInvokedWithAddJSDisabledHeaderTrue_AddsHeader()
        {
            // Arrange
            addJsDisabledHeader = true;
            _mockHttpMessageHandler
                .When(HttpMethod.Post, GetFormattedUrl(Constants.CdsApiEndpoints.EvaluateServiceDefinitionPathFormat, ServiceDefinitionId))
                .With(ValidateMockRequestHeaders)
                .With(ValidateMockEvaluateRequestBody)
                .Respond(HttpStatusCode.OK, Constants.ContentTypes.ApplicationJsonFhir, MockedProviderResponse);
            
            // Act
            var response = await _onlineConsultationsProviderHttpClient.EvaluateServiceDefinition(ServiceDefinitionId, MockedEvaluateRequestBody, addJsDisabledHeader);
            
            // Assert
            (await response.Content.ReadAsStringAsync()).Should().BeEquivalentTo(MockedProviderResponse);
        }
        
        [TestMethod]
        public async Task EvaluateServiceDefinition_WhenInvokedWithAddJSDisabledHeaderFalse_DoesNotAddHeader()
        {
            // Arrange
            addJsDisabledHeader = false;
            _mockHttpMessageHandler
                .When(HttpMethod.Post, GetFormattedUrl(Constants.CdsApiEndpoints.EvaluateServiceDefinitionPathFormat, ServiceDefinitionId))
                .With(ValidateMockRequestHeaders)
                .With(ValidateMockEvaluateRequestBody)
                .Respond(HttpStatusCode.OK, Constants.ContentTypes.ApplicationJsonFhir, MockedProviderResponse);
            
            // Act
            var response = await _onlineConsultationsProviderHttpClient.EvaluateServiceDefinition(ServiceDefinitionId, MockedEvaluateRequestBody, addJsDisabledHeader);
            
            // Assert
            (await response.Content.ReadAsStringAsync()).Should().BeEquivalentTo(MockedProviderResponse);
        }

        private string GetFormattedUrl(string endpointFormat, string serviceDefinitionId)
        {
            return TestProviderSettings.BaseAddress + string.Format(
                       CultureInfo.InvariantCulture,
                       endpointFormat,
                       serviceDefinitionId);
        }

        private bool ValidateMockRequestHeaders(HttpRequestMessage request)
        {
            return 
                "Bearer".Equals(request.Headers.Authorization.Scheme, StringComparison.Ordinal) &&
                   (!addJsDisabledHeader || request.Headers.Contains(Constants.HttpRequestHeaders.NHSOJavascriptDisabled)) &&
                   TestProviderSettings.BearerToken.Equals(request.Headers.Authorization.Parameter,
                       StringComparison.Ordinal);
        }

        private static bool ValidateMockEvaluateRequestBody(HttpRequestMessage request)
        {
            return request.Content.ReadAsStringAsync().Result.Equals(MockedEvaluateRequestBody, StringComparison.Ordinal);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _mockHttpMessageHandler.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}