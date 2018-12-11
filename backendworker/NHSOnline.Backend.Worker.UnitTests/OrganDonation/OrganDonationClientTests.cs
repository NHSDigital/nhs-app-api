using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.ResponseParsers;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.Worker.UnitTests.OrganDonation
{
    [TestClass]
    public sealed class OrganDonationClientTests : IDisposable
    {
        public const string DefaultClientIdHeader = "DefaultClientIdHeader";
        public const string DefaultSubscriptionHeader = "DefaultSubscriptionHeader";
        public const string SearchPath = "Registration/_search";
        public static readonly Uri BaseUri = new Uri("http://base_url/");

        private IOrganDonationClient _systemUnderTest;
        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<IOrganDonationConfig> _configMock;
        private OrganDonationHttpClient _httpClient;
        private IFixture _fixture;
        private UserSession _userSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<IJsonResponseParser>(() => new JsonResponseParser());
            
            _mockHttpHandler = new MockHttpMessageHandler();
            
            _configMock = new Mock<IOrganDonationConfig>();
            _configMock.SetupGet(x => x.BaseUrl).Returns(BaseUri);
            _configMock.SetupGet(x => x.ClientIdHeader).Returns(DefaultClientIdHeader);
            _configMock.SetupGet(x => x.SubscriptionHeader).Returns(DefaultSubscriptionHeader);

            _httpClient = new OrganDonationHttpClient(new HttpClient(_mockHttpHandler), _configMock.Object);

            _fixture.Inject(_configMock);
            _fixture.Inject(_httpClient);

            _userSession = _fixture.Create<UserSession>();

            _systemUnderTest = _fixture.Create<OrganDonationClient>();
        }

        [TestMethod]
        public async Task PostLookup_ReturnsValidResponse()
        {
            var expectedResponse = _fixture.Create<OrganDonationSuccessResponse<RegistrationLookupResponse>>();
            
            _mockHttpHandler
                .WhenOrganDonation(HttpMethod.Post, SearchPath)
                .Respond(System.Net.Mime.MediaTypeNames.Application.Json, JsonConvert.SerializeObject(expectedResponse));

            var response = await _systemUnderTest.PostLookup(new LookupRegistrationRequest(), _userSession);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task PostLookup_ReturnsError()
        {
            var expectedResponse = _fixture.Create<OrganDonationErrorResponse>();

            _mockHttpHandler
                .WhenOrganDonation(HttpMethod.Post, SearchPath)
                .Respond(HttpStatusCode.InternalServerError, System.Net.Mime.MediaTypeNames.Application.Json,
                    JsonConvert.SerializeObject(expectedResponse));

            var response = await _systemUnderTest.PostLookup(new LookupRegistrationRequest(), _userSession);

            response.ErrorResponse.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.Body.Should().BeNull();
        }

        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }

    }
}
