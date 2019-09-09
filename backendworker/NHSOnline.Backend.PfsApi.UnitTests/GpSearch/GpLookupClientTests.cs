using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NHSOnline.Backend.PfsApi.GpSearch;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.Support.ResponseParsers;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.PfsApi.UnitTests.GpSearch
{
    [TestClass]
    public sealed class GpLookupClientTests : IDisposable
    {
        private IFixture _fixture;
        private GpLookupClient _systemUnderTest;

        private Uri _gLookupClientApiBaseUrl;
        private string _gpLookupApiKey;
        private MockHttpMessageHandler _mockHttpHandler;
        private GpLookupHttpClient _httpClient;
        private Mock<IGpLookupConfig> _mockConfig;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<IJsonResponseParser>(() => new JsonResponseParser());
            _gLookupClientApiBaseUrl = _fixture.Create<Uri>();
            _gpLookupApiKey = _fixture.Create<string>();

            _mockHttpHandler = new MockHttpMessageHandler();
            _mockConfig = _fixture.Freeze<Mock<IGpLookupConfig>>();

            _mockConfig.SetupGet(x => x.NhsSearchBaseUrl).Returns(_gLookupClientApiBaseUrl);
            _mockConfig.SetupGet(x => x.GpLookupApiKey).Returns(_gpLookupApiKey);

            _httpClient = new GpLookupHttpClient(new HttpClient(_mockHttpHandler), _mockConfig.Object);
            _fixture.Inject(_httpClient);

            _systemUnderTest = _fixture.Create<GpLookupClient>();
        }

        [TestMethod]
        public async Task PharmaciesSearch_CallsCorrectEndpoint_AndHandlesResponse()
        {
            // Arrange
            var request = new OrganisationSearchData();

            var expectedResponse = _fixture.Create<NhsOrganisationSearchResponse>();

            _mockHttpHandler
                .When(HttpMethod.Post, new Uri(_gLookupClientApiBaseUrl, "service-search/search?api-version=1").ToString())
                .WithHeaders("subscription-key", $"{_gpLookupApiKey}")
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.PharmaciesSearch(request);

            // Assert
            using (new AssertionScope())
            {
                response.Body.Should().BeEquivalentTo(expectedResponse);
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }

            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }
        
        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}
