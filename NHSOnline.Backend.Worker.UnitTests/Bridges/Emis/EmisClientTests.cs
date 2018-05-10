using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.UnitTests.Bridges.Emis.Helpers;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis
{
    [TestClass]
    public class EmisClientTests
    {
        public const string DefaultEmisVersion = "2.1.0.0";
        public static readonly string DefaultEmisApplicationId = Guid.NewGuid().ToString();

        public static readonly Uri BaseUri = new Uri("http://185.13.72.81/PFS/");

        private IEmisClient _sut;
        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<IEmisConfig> _configMock;
        private HttpClient _httpClient;
        private Mock<IHttpClientFactory> _httpClientFactory;
        private static IFixture _fixture;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _mockHttpHandler = new MockHttpMessageHandler();

            _configMock = new Mock<IEmisConfig>();
            _configMock.SetupGet(x => x.BaseUrl).Returns(BaseUri);
            _configMock.SetupGet(x => x.Version).Returns(DefaultEmisVersion);
            _configMock.SetupGet(x => x.ApplicationId).Returns(DefaultEmisApplicationId);
            _httpClient = new HttpClient(_mockHttpHandler);

            _httpClientFactory = _fixture.Freeze<Mock<IHttpClientFactory>>();
            _httpClientFactory.Setup(x => x.GetClient(HttpClientName.EmisApiClient)).Returns(_httpClient);

            _fixture.Inject(_configMock);

            _sut = _fixture.Create<EmisClient>();
        }

        [TestMethod]
        public async Task SessionsEndUserSessionPost_ReturnsAnEndUserSessionId_WhenValidlyRequested()
        {
            var expectedEndUserSessionResponse = _fixture.Create<SessionsEndUserSessionPostResponse>();

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "sessions/endusersession")
                .WithEmisHeaders()
                .Respond("application/json", JsonConvert.SerializeObject(expectedEndUserSessionResponse));

            var response = await _sut.SessionsEndUserSessionPost();

            response.Body.Should().BeEquivalentTo(expectedEndUserSessionResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().BeNull();
        }

        [TestMethod]
        public async Task SessionsPost_ReturnsASessionResponse_WhenValidlyRequested()
        {
            var endUserSessionId = _fixture.Create<string>();
            var expectedResponse = _fixture.Create<SessionsPostResponse>();
            var requestBody = _fixture.Create<SessionsPostRequest>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId)
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "sessions")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(requestBody))
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await _sut.SessionsPost(endUserSessionId, requestBody);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task MeApplicationsPost_ReturnsAnApplicationsPostResponse_WhenValidlyRequested()
        {
            var endUserSessionId = _fixture.Create<string>();
            var expectedResponse = _fixture.Create<MeApplicationsPostResponse>();
            var requestBody = _fixture.Create<MeApplicationsPostRequest>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId)
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "me/applications")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(requestBody))
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await _sut.MeApplicationsPost(endUserSessionId, requestBody);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task
            MeApplicationsPost_ReturnsAnApplicationsPostResponseWithErrorDetails_WhenUserAlreadyRegistered()
        {
            var endUserSessionId = _fixture.Create<string>();
            var expectedResponse = _fixture.Create<ErrorResponse>();
            var requestBody = _fixture.Create<MeApplicationsPostRequest>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId)
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "me/applications")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(requestBody))
                .Respond(HttpStatusCode.InternalServerError, "application/json",
                    JsonConvert.SerializeObject(expectedResponse));

            var response = await _sut.MeApplicationsPost(endUserSessionId, requestBody);

            response.ErrorResponse.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(500);
            response.Body.Should().Be(null);
        }

        [TestMethod]
        public async Task DemographicsGet_ReturnsADemographicsResponse_WhenValidlyRequested()
        {
            var userPatientLinkToken = _fixture.Create<string>();
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();

            var expectedResponse = _fixture.Create<DemographicsGetResponse>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(EmisClient.HeaderSessionId, sessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Get, "demographics?userPatientLinkToken=" + userPatientLinkToken)
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await _sut.DemographicsGet(userPatientLinkToken, sessionId, endUserSessionId);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task EndpointCalled_ReturnsErrorResponseCodeWithNullBody_ResponseHasEmptyErrorProperties()
        {
            var endUserSessionId = _fixture.Create<string>();
            var requestBody = _fixture.Create<SessionsPostRequest>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId)
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "sessions")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(requestBody))
                .Respond(HttpStatusCode.Forbidden);

            var response = await _sut.SessionsPost(endUserSessionId, requestBody);

            response.Body.Should().BeNull();
            response.ErrorResponse.Should().BeNull();
            response.ErrorResponseBadRequest.Should().BeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task EndpointCalled_ReturnsErrorResponseCodeWithEmptyBody_ResponseHasEmptyErrorProperties()
        {
            var endUserSessionId = _fixture.Create<string>();
            var requestBody = _fixture.Create<SessionsPostRequest>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId)
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "sessions")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(requestBody))
                .Respond(HttpStatusCode.Forbidden, "application/json", string.Empty);

            var response = await _sut.SessionsPost(endUserSessionId, requestBody);

            response.Body.Should().BeNull();
            response.ErrorResponse.Should().BeNull();
            response.ErrorResponseBadRequest.Should().BeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task EndpointCalled_ReturnsBadRequest_ResponseHasPopulatedErrorResponseBadRequestProperty()
        {
            var endUserSessionId = _fixture.Create<string>();
            var requestBody = _fixture.Create<SessionsPostRequest>();
            var expectedResponse = _fixture.Create<BadRequestErrorResponse>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId)
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "sessions")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(requestBody))
                .Respond(HttpStatusCode.BadRequest, "application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await _sut.SessionsPost(endUserSessionId, requestBody);

            response.Body.Should().BeNull();
            response.ErrorResponse.Should().BeNull();
            response.ErrorResponseBadRequest.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task PrescriptionsGet_ReturnsAPrescriptionsResponse_WhenValidlyRequested()
        {
            var userPatientLinkToken = _fixture.Create<string>();
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();
            var fromDateTime = _fixture.Create<DateTimeOffset>();
            var toDateTime = _fixture.Create<DateTimeOffset>();

            var expectedResponse = _fixture.Create<PrescriptionRequestsGetResponse>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(EmisClient.HeaderSessionId, sessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Get, "prescriptionrequests?userPatientLinkToken=" + userPatientLinkToken + "&filterFromDate=" + HttpUtility.UrlEncode(fromDateTime.ToString("O")) + "&filterToDate=" + HttpUtility.UrlEncode(toDateTime.ToString("O")))
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await _sut.PrescriptionsGet(userPatientLinkToken, sessionId, endUserSessionId, fromDateTime, toDateTime);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task CoursesGet_ReturnsACoursesResponse_WhenValidlyRequested()
        {
            var userPatientLinkToken = _fixture.Create<string>();
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();

            var expectedResponse = _fixture.Create<CoursesGetResponse>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(EmisClient.HeaderSessionId, sessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Get, "courses?userPatientLinkToken=" + userPatientLinkToken)
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await _sut.CoursesGet(userPatientLinkToken, sessionId, endUserSessionId);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ErrorResponse.Should().Be(null);
        }
    }
}