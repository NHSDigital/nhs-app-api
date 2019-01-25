using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Verifications;
using NHSOnline.Backend.Worker.ResponseParsers;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support.Http;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis
{
    [TestClass]
    public sealed class EmisClientTests : IDisposable
    {
        public const string DefaultEmisVersion = "2.1.0.0";
        public static readonly string DefaultEmisApplicationId = Guid.NewGuid().ToString();

        public static readonly Uri BaseUri = new Uri("http://emis_base_url/");

        private IEmisClient _sut;
        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<IEmisConfig> _configMock;
        private EmisHttpClient _httpClient;
        private IFixture _fixture;
        private Mock<IOptions<ConfigurationSettings>> _settings;
        private ConfigurationSettings _configurationSettings;
        private Mock<HttpMessageHandler> mockedHandler;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<IJsonResponseParser>(() => new JsonResponseParser());

            _mockHttpHandler = new MockHttpMessageHandler();

            _configMock = new Mock<IEmisConfig>();
            _configMock.SetupGet(x => x.BaseUrl).Returns(BaseUri);
            _configMock.SetupGet(x => x.Version).Returns(DefaultEmisVersion);
            _configMock.SetupGet(x => x.ApplicationId).Returns(DefaultEmisApplicationId);

            _settings = _fixture.Freeze<Mock<IOptions<ConfigurationSettings>>>();

            _configurationSettings = _fixture.Create<ConfigurationSettings>();
            _configurationSettings.DefaultHttpTimeoutSeconds = 2;
            _configurationSettings.EmisExtendedHttpTimeoutSeconds = 6;

            _settings
                .Setup(x => x.Value)
                .Returns(_configurationSettings);

            _httpClient = new EmisHttpClient(new HttpClient(_mockHttpHandler), _configMock.Object);

            _fixture.Inject(_configMock);
            _fixture.Inject(_httpClient);

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
            response.ExceptionErrorResponse.Should().BeNull();
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
            response.ExceptionErrorResponse.Should().Be(null);
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
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task
            MeApplicationsPost_ReturnsAnApplicationsPostResponseWithErrorDetails_WhenUserAlreadyRegistered()
        {
            var endUserSessionId = _fixture.Create<string>();
            var expectedResponse = _fixture.Create<ExceptionErrorResponse>();
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

            response.ExceptionErrorResponse.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(500);
            response.Body.Should().BeEquivalentTo(new MeApplicationsPostResponse());
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
            response.ExceptionErrorResponse.Should().Be(null);
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
            response.ExceptionErrorResponse.Should().BeNull();
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
            response.ExceptionErrorResponse.Should().BeNull();
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

            response.Body.Should().BeEquivalentTo(new SessionsPostResponse());
            response.ExceptionErrorResponse.Should().BeNull();
            response.ErrorResponseBadRequest.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task AllergiesGet_ReturnsAnAllergiesResponse_WhenValidlyRequested()
        {
            var userPatientLinkToken = _fixture.Create<string>();
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();

            var expectedResponse = _fixture.Create<MedicationRootObject>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(EmisClient.HeaderSessionId, sessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Get, "record?userPatientLinkToken=" + userPatientLinkToken + "&itemType=Allergies")
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await _sut.MedicalRecordGet(userPatientLinkToken, sessionId, endUserSessionId,
                RecordType.Allergies);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
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
                .WhenEmis(HttpMethod.Get,
                    "prescriptionrequests?userPatientLinkToken=" + userPatientLinkToken + "&filterFromDate="
                    + HttpUtility.UrlEncode(fromDateTime.ToString("O", CultureInfo.InvariantCulture)) + "&filterToDate="
                    + HttpUtility.UrlEncode(toDateTime.ToString("O", CultureInfo.InvariantCulture)))
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await _sut.PrescriptionsGet(userPatientLinkToken, sessionId, endUserSessionId, fromDateTime,
                toDateTime);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
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
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task ApplicationsPost_ReturnsTrue_WhenValidlyRequested()
        {
            var expectedResponse = new BookAppointmentSlotPostResponse
            {
                BookingCreated = true
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "appointments")
                .WithEmisHeaders()
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var userSession = new EmisUserSession();

            var response = await _sut.AppointmentsPost(new EmisHeaderParameters(userSession),
                new BookAppointmentSlotPostRequest(userSession.UserPatientLinkToken, new AppointmentBookRequest()));

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ExceptionErrorResponse.Should().BeNull();
        }

        [TestMethod]
        public async Task NhsUsersPost_ReturnsLinkageDetails_WhenValidRequest()
        {
            // Arrange
            const string endUserSessionId = "2ijfd";

            var requestBody = _fixture.Create<AddNhsUserRequest>();
            var expectedResponse = _fixture.Create<AddNhsUserResponse>();

            var emisUserSession = new EmisUserSession
            {
                EndUserSessionId = endUserSessionId,
            };

            var emisHeaderParameters = new EmisHeaderParameters(emisUserSession);

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "users/nhs")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(requestBody))
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _sut.NhsUserPost(emisHeaderParameters, requestBody);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task VerificationsPost_ReturnsLinkageDetails_WhenValidRequest()
        {
            // Arrange
            const string endUserSessionId = "2ijfd";
            const string nhsNumber = "nhsNumber123";
            const string odsCode = "odsCode";
            const string token = "token1";

            var emisUserSession = new EmisUserSession
            {
                EndUserSessionId = endUserSessionId,
            };

            var addVerificationRequest = new AddVerificationRequest
            {
                NhsNumber = nhsNumber,
                NationalPracticeCode = odsCode,
                Token = token,
            };

            var expectedResponse = _fixture.Create<AddVerificationResponse>();

            var emisHeaderParameters = new EmisHeaderParameters(emisUserSession);

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "me/verifications")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(addVerificationRequest))
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _sut.VerificationPost(emisHeaderParameters, addVerificationRequest);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task SessionsEndUserSessionPost_VerifyCustomTimeoutHeaderPresent()
        {
            // Arrange
            SetupMockedHandlerEmisForEmisCustomTimeout();
            
            // Act
            await _sut.SessionsEndUserSessionPost();

            // Assert
            VerifyCustomTimeoutPresentInRequest();
        }
        
        [TestMethod]
        public async Task NhsUserPost_VerifyCustomTimeoutHeaderPresent()
        {
            // Arrange
            SetupMockedHandlerEmisForEmisCustomTimeout();
            
            const string endUserSessionId = "2ijfd";

            var emisUserSession = new EmisUserSession
            {
                EndUserSessionId = endUserSessionId,
            };

            var emisHeaderParameters = new EmisHeaderParameters(emisUserSession);
            var requestBody = _fixture.Create<AddNhsUserRequest>();
            
            // Act
            await _sut.NhsUserPost(emisHeaderParameters, requestBody);

            // Assert
            VerifyCustomTimeoutPresentInRequest();
        }
        
        [TestMethod]
        public async Task VerificationPost_VerifyCustomTimeoutHeaderPresent()
        {
            // Arrange
            const string endUserSessionId = "2ijfd";
            const string nhsNumber = "nhsNumber123";
            const string odsCode = "odsCode";
            const string token = "token1";

            SetupMockedHandlerEmisForEmisCustomTimeout();
            
            var emisUserSession = new EmisUserSession
            {
                EndUserSessionId = endUserSessionId,
            };

            var addVerificationRequest = new AddVerificationRequest
            {
                NhsNumber = nhsNumber,
                NationalPracticeCode = odsCode,
                Token = token,
            };

            var emisHeaderParameters = new EmisHeaderParameters(emisUserSession);
            
            // Act
            await _sut.VerificationPost(emisHeaderParameters, addVerificationRequest);

            // Assert
            VerifyCustomTimeoutPresentInRequest();
        }
        
        [TestMethod]
        public async Task CoursesGet_VerifyDefaultTimeIsUsed()
        {
            // Arrange
            var userPatientLinkToken = _fixture.Create<string>();
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();

            SetupMockedHandlerEmisForEmisCustomTimeout();
            
            // Act
            await _sut.CoursesGet(userPatientLinkToken, sessionId, endUserSessionId);

            // Assert
            mockedHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Properties.Count == 0
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        private void SetupMockedHandlerEmisForEmisCustomTimeout()
        {
            mockedHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockedHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )                
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Anything"),
                })
                .Verifiable();

            var httpClient = new EmisHttpClient(new HttpClient(mockedHandler.Object), _configMock.Object);

            _fixture.Inject(_configMock);
            _fixture.Inject(httpClient);

            _sut = _fixture.Create<EmisClient>();
        }

        private void VerifyCustomTimeoutPresentInRequest()
        {
            mockedHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Properties.Count == 1
                    && (int)req.Properties[HttpRequestConstants.CustomTimeout]
                    == _configurationSettings.EmisExtendedHttpTimeoutSeconds
                ),
                ItExpr.IsAny<CancellationToken>()
            );
        }
          
        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}