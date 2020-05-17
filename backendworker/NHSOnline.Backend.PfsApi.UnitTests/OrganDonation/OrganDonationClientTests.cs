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
using NHSOnline.Backend.Support.ResponseParsers;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support.Session;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.PfsApi.UnitTests.OrganDonation
{
    [TestClass]
    public sealed class OrganDonationClientTests : IDisposable
    {
        private const string DefaultClientIdHeader = "DefaultClientIdHeader";
        private const string DefaultSubscriptionHeader = "DefaultSubscriptionHeader";
        private const string SearchPath = "Registration/_search";
        private const string RegistrationPath = "Registration";
        private const string ReferenceDataPath = "ReferenceData";
        public static readonly Uri BaseUri = new Uri("http://base_url/");

        private IOrganDonationClient _systemUnderTest;
        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<IOrganDonationConfig> _configMock;
        private OrganDonationHttpClient _httpClient;
        private IFixture _fixture;
        private P9UserSession _userSession;

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

            _userSession = _fixture.Create<P9UserSession>();

            _systemUnderTest = _fixture.Create<OrganDonationClient>();
        }

        [TestMethod]
        public async Task PostLookup_ReturnsValidResponse()
        {
            var expectedResponse = _fixture.Create<RegistrationLookupResponse>();

            _mockHttpHandler
                .WhenOrganDonation(HttpMethod.Post, SearchPath)
                .Respond(System.Net.Mime.MediaTypeNames.Application.Json, JsonConvert.SerializeObject(expectedResponse));

            var response = await _systemUnderTest.PostLookup(new RegistrationLookupRequest(), _userSession);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.NotFound)]
        [DataTestMethod]
        public async Task PostLookup_ReturnsError(HttpStatusCode errorCode)
        {
            var expectedResponse = _fixture.Create<OrganDonationErrorResponse>();

            _mockHttpHandler
                .WhenOrganDonation(HttpMethod.Post, SearchPath)
                .Respond(errorCode, System.Net.Mime.MediaTypeNames.Application.Json,
                    JsonConvert.SerializeObject(expectedResponse));

            var response = await _systemUnderTest.PostLookup(new RegistrationLookupRequest(), _userSession);

            response.ErrorResponse.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(errorCode);
            response.Body.Should().BeNull();
        }

        [TestMethod]
        public async Task GetReferenceData_ReturnsValidResponse()
        {
            var expectedResponse = _fixture.Create<ReferenceDataResponse>();

            _mockHttpHandler
                .WhenOrganDonation(HttpMethod.Get, ReferenceDataPath)
                .Respond(System.Net.Mime.MediaTypeNames.Application.Json,
                    JsonConvert.SerializeObject(expectedResponse));

            var response = await _systemUnderTest.GetAllReferenceData();

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.NotFound)]
        [DataTestMethod]
        public async Task GetReferenceData_ReturnsError(HttpStatusCode errorCode)
        {
            var expectedResponse = _fixture.Create<OrganDonationErrorResponse>();

            _mockHttpHandler
                .WhenOrganDonation(HttpMethod.Get, ReferenceDataPath)
                .Respond(errorCode, System.Net.Mime.MediaTypeNames.Application.Json,
                    JsonConvert.SerializeObject(expectedResponse));

            var response = await _systemUnderTest.GetAllReferenceData();

            response.ErrorResponse.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(errorCode);
            response.Body.Should().BeNull();
        }

        [TestMethod]
        public async Task PostRegistration_ReturnsValidResponse()
        {
            var expectedResponse = _fixture.Create<OrganDonationBasicResponse>();

            _mockHttpHandler
                .WhenOrganDonation(HttpMethod.Post, RegistrationPath)
                .Respond(System.Net.Mime.MediaTypeNames.Application.Json,
                    JsonConvert.SerializeObject(expectedResponse));

            var response = await _systemUnderTest.PostRegistration(new RegistrationRequest(), _userSession);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.NotFound)]
        public async Task PostRegistration_ReturnsError(HttpStatusCode errorCode)
        {
            var expectedResponse = _fixture.Create<OrganDonationErrorResponse>();

            _mockHttpHandler
                .WhenOrganDonation(HttpMethod.Post, RegistrationPath)
                .Respond(errorCode, System.Net.Mime.MediaTypeNames.Application.Json,
                    JsonConvert.SerializeObject(expectedResponse));

            var response = await _systemUnderTest.PostRegistration(new RegistrationRequest(), _userSession);

            response.ErrorResponse.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(errorCode);
            response.Body.Should().BeNull();
        }

        [TestMethod]
        public async Task PutUpdate_ReturnsValidResponse()
        {
            var expectedResponse = _fixture.Create<OrganDonationBasicResponse>();

            _mockHttpHandler
                .WhenOrganDonation(HttpMethod.Put, $"{RegistrationPath}/id")
                .Respond(System.Net.Mime.MediaTypeNames.Application.Json,
                    JsonConvert.SerializeObject(expectedResponse));

            var updateRequest = new RegistrationRequest()
            {
                Id = "id"
            };

            var response = await _systemUnderTest.PutUpdate(updateRequest, _userSession);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.NotFound)]
        public async Task PutUpdate_ReturnsError(HttpStatusCode errorCode)
        {
            var expectedResponse = _fixture.Create<OrganDonationErrorResponse>();

            _mockHttpHandler
                .WhenOrganDonation(HttpMethod.Put, $"{RegistrationPath}/id")
                .Respond(errorCode, System.Net.Mime.MediaTypeNames.Application.Json,
                    JsonConvert.SerializeObject(expectedResponse));

            var updateRequest = new RegistrationRequest
            {
                Id = "id"
            };

            var response = await _systemUnderTest.PutUpdate(updateRequest, _userSession);

            response.ErrorResponse.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(errorCode);
            response.Body.Should().BeNull();
        }

        [TestMethod]
        public async Task Delete_ReturnsValidResponse()
        {
            var expectedResponse = _fixture.Create<OrganDonationBasicResponse>();

            _mockHttpHandler
                .WhenOrganDonation(HttpMethod.Delete, $"{RegistrationPath}/id")
                .Respond(System.Net.Mime.MediaTypeNames.Application.Json,
                    JsonConvert.SerializeObject(expectedResponse));

            var request = new WithdrawRequest
            {
                Id = "id"
            };

            var response = await _systemUnderTest.Delete(request, _userSession);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.NotFound)]
        public async Task Delete_ReturnsError(HttpStatusCode errorCode)
        {
            var expectedResponse = _fixture.Create<OrganDonationErrorResponse>();

            _mockHttpHandler
                .WhenOrganDonation(HttpMethod.Delete, $"{RegistrationPath}/id")
                .Respond(errorCode, System.Net.Mime.MediaTypeNames.Application.Json,
                    JsonConvert.SerializeObject(expectedResponse));

            var request = new WithdrawRequest
            {
                Id = "id"
            };

            var response = await _systemUnderTest.Delete(request, _userSession);

            response.ErrorResponse.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(errorCode);
            response.Body.Should().BeNull();
        }

        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}
