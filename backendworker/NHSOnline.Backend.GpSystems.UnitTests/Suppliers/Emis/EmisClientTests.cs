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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.ResponseParsers;
using NHSOnline.Backend.Support.Http;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis
{
    [TestClass]
    public sealed class EmisClientTests : IDisposable
    {
        public const string DefaultEmisVersion = "2.1.0.0";
        public static readonly string DefaultEmisApplicationId = Guid.NewGuid().ToString();

        public static readonly Uri BaseUri = new Uri("http://emis_base_url/");

        private const string CertificatePath = "CertificatePath";
        private const string CertificatePassphrase = "CerticiatePassphrase";

        private const int EmisExtendedHttpTimeoutSeconds = 6;
        private const int DefaultHttpTimeoutSeconds = 2;
        private const int PrescriptionsMaxCoursesSoftLimit = 100;
        private const int CoursesMaxCoursesLimit = 100;

        private const string Environment = "testEnv";

        private IEmisClient _systemUnderTest;
        private MockHttpMessageHandler _mockHttpHandler;
        private EmisConfigurationSettings _emisConfig;
        private EmisHttpClient _httpClient;
        private IFixture _fixture;
        private Mock<HttpMessageHandler> _mockedHandler;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<IJsonResponseParser>(() => new JsonResponseParser());

            _mockHttpHandler = new MockHttpMessageHandler();

            _emisConfig = new EmisConfigurationSettings(BaseUri, DefaultEmisApplicationId, DefaultEmisVersion, CertificatePath,
                CertificatePassphrase, EmisExtendedHttpTimeoutSeconds, DefaultHttpTimeoutSeconds, CoursesMaxCoursesLimit, PrescriptionsMaxCoursesSoftLimit,
                Environment);

            _httpClient = new EmisHttpClient(new HttpClient(_mockHttpHandler), _emisConfig);

            _fixture.Inject(_emisConfig);
            _fixture.Inject(_httpClient);

            _systemUnderTest = _fixture.Create<EmisClient>();
        }

        [TestMethod]
        public async Task SessionsEndUserSessionPost_ReturnsAnEndUserSessionId_WhenValidlyRequested()
        {
            // Arrange
            var expectedEndUserSessionResponse = _fixture.Create<SessionsEndUserSessionPostResponse>();

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "sessions/endusersession")
                .WithEmisHeaders()
                .Respond("application/json", JsonConvert.SerializeObject(expectedEndUserSessionResponse));

            // Act
            var response = await _systemUnderTest.SessionsEndUserSessionPost();

            // Assert
            response.Body.Should().BeEquivalentTo(expectedEndUserSessionResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ExceptionErrorResponse.Should().BeNull();
        }

        [TestMethod]
        public async Task SessionsPost_ReturnsASessionResponse_WhenValidlyRequested()
        {
            // Arrange
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

            // Act
            var response = await _systemUnderTest.SessionsPost(endUserSessionId, requestBody);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task MeApplicationsPost_ReturnsAnApplicationsPostResponse_WhenValidlyRequested()
        {
            // Arrange
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

            // Act
            var response = await _systemUnderTest.MeApplicationsPost(endUserSessionId, requestBody);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task
            MeApplicationsPost_ReturnsAnApplicationsPostResponseWithErrorDetails_WhenUserAlreadyRegistered()
        {
            // Arrange
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

            // Act
            var response = await _systemUnderTest.MeApplicationsPost(endUserSessionId, requestBody);

            // Assert
            response.ExceptionErrorResponse.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(500);
        }

        [TestMethod]
        public async Task MeSettingsGet_ReturnsUserSettingsGetResponse_WhenValidlyRequested()
        {
            var userPatientLinkToken = _fixture.Create<string>();
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();
            var expectedResponse = _fixture.Create<MeSettingsGetResponse>();

            var emisUserSession = new EmisUserSession
            {
                EndUserSessionId = endUserSessionId,
                SessionId = sessionId,
            };

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(EmisClient.HeaderSessionId, sessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Get, "me/settings")
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await _systemUnderTest.MeSettingsGet(new EmisRequestParameters(emisUserSession));

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task DemographicsGet_ReturnsADemographicsResponse_WhenValidlyRequested()
        {
            // Arrange
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

            // Act
            var response = await _systemUnderTest.DemographicsGet(new EmisRequestParameters
            {
                UserPatientLinkToken = userPatientLinkToken,
                SessionId = sessionId,
                EndUserSessionId = endUserSessionId
            });

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task EndpointCalled_ReturnsErrorResponseCodeWithNullBody_ResponseHasEmptyErrorProperties()
        {
            // Arrange
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

            // Act
            var response = await _systemUnderTest.SessionsPost(endUserSessionId, requestBody);

            // Assert
            response.Body.Should().BeNull();
            response.ExceptionErrorResponse.Should().BeNull();
            response.ErrorResponseBadRequest.Should().BeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task EndpointCalled_ReturnsErrorResponseCodeWithEmptyBody_ResponseHasEmptyErrorProperties()
        {
            // Arrange
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

            // Act
            var response = await _systemUnderTest.SessionsPost(endUserSessionId, requestBody);

            // Assert
            response.Body.Should().BeNull();
            response.ExceptionErrorResponse.Should().BeNull();
            response.ErrorResponseBadRequest.Should().BeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task EndpointCalled_ReturnsBadRequest_ResponseHasPopulatedErrorResponseBadRequestProperty()
        {
            // Arrange
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

            // Act
            var response = await _systemUnderTest.SessionsPost(endUserSessionId, requestBody);

            // Assert
            response.ExceptionErrorResponse.Should().BeNull();
            response.ErrorResponseBadRequest.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task AllergiesGet_ReturnsAnAllergiesResponse_WhenValidlyRequested()
        {
            // Arrange
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

            // Act
            var response = await _systemUnderTest.MedicalRecordGet(
                new EmisRequestParameters
                {
                    UserPatientLinkToken = userPatientLinkToken,
                    SessionId = sessionId,
                    EndUserSessionId = endUserSessionId
                },
                RecordType.Allergies);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task PrescriptionsGet_ReturnsAPrescriptionsResponse_WhenValidlyRequested()
        {
            // Arrange
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

            // Act
            var response = await _systemUnderTest.PrescriptionsGet(
                new EmisRequestParameters
                {
                    SessionId = sessionId,
                    EndUserSessionId = endUserSessionId,
                    UserPatientLinkToken = userPatientLinkToken
                },
                fromDateTime, toDateTime);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task CoursesGet_ReturnsACoursesResponse_WhenValidlyRequested()
        {
            // Arrange
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

            // Act
            var response = await _systemUnderTest.CoursesGet(new EmisRequestParameters
            {
                SessionId = sessionId,
                EndUserSessionId = endUserSessionId,
                UserPatientLinkToken = userPatientLinkToken
            });

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        public async Task PrescriptionsPost_ReturnsValidResponse_WhenValidlyRequested()
        {
            // Arrange
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();
            var prescriptionPostRequest = _fixture.Create<PrescriptionRequestsPost>();

            var expectedResponse = new PrescriptionRequestPostResponse();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(
                    EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(
                    EmisClient.HeaderSessionId, sessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "prescriptionrequests")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(prescriptionPostRequest))
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.PrescriptionsPost(
                endUserSessionId,
                sessionId,
                prescriptionPostRequest);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ExceptionErrorResponse.Should().BeNull();
        }

        [TestMethod]
        public async Task AppointmentsPost_ReturnsValidResponse_WhenValidlyRequested()
        {
            // Arrange
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();
            var userPatientLinkToken = _fixture.Create<string>();
            var appointmentBookRequest = _fixture.Create<AppointmentBookRequest>();
            appointmentBookRequest.SlotId = _fixture.Create<int>().ToString(CultureInfo.CurrentCulture);

            var expectedRequest = new BookAppointmentSlotPostRequest(userPatientLinkToken, appointmentBookRequest);

            var expectedResponse = new BookAppointmentSlotPostResponse
            {
                BookingCreated = true
            };

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(
                    EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(
                    EmisClient.HeaderSessionId, sessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "appointments")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(expectedRequest))
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var emisRequestParameters = new EmisRequestParameters
            {
                EndUserSessionId = endUserSessionId,
                SessionId = sessionId,
                UserPatientLinkToken = userPatientLinkToken,
            };

            // Act
            var response = await _systemUnderTest.AppointmentsPost(
                emisRequestParameters,
                appointmentBookRequest);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ExceptionErrorResponse.Should().BeNull();
        }

        [TestMethod]
        public async Task AppointmentsCancel_ReturnsValidResponse_WhenValidlyRequested()
        {
            // Arrange
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();
            var userPatientLinkToken = _fixture.Create<string>();
            var slotId = _fixture.Create<long>();
            var cancellationReason = _fixture.Create<string>();

            var expectedRequest = new CancelAppointmentDeleteRequest(userPatientLinkToken, cancellationReason, slotId);

            var expectedResponse = new CancelAppointmentDeleteResponse
            {
                IsCancelled = true
            };

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(
                    EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(
                    EmisClient.HeaderSessionId, sessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Delete, "appointments")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(expectedRequest))
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var emisRequestParameters = new EmisRequestParameters
            {
                EndUserSessionId = endUserSessionId,
                SessionId = sessionId,
                UserPatientLinkToken = userPatientLinkToken,
            };

            // Act
            var response = await _systemUnderTest.AppointmentsDelete(
                emisRequestParameters,
                slotId,
                new CancellationReason { DisplayName = cancellationReason });

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ExceptionErrorResponse.Should().BeNull();
        }

        [TestMethod]
        public async Task NhsUsersPost_ReturnsLinkageDetails_WhenValidRequest()
        {
            // Arrange
            var endUserSessionId = _fixture.Create<string>();
            var requestBody = _fixture.Create<AddNhsUserRequest>();
            var expectedResponse = _fixture.Create<AddNhsUserResponse>();

            var emisUserSession = new EmisUserSession
            {
                EndUserSessionId = endUserSessionId,
            };

            var emisRequestParameters = new EmisRequestParameters(emisUserSession);

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
            var response = await _systemUnderTest.NhsUserPost(emisRequestParameters, requestBody);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task VerificationsPost_ReturnsLinkageDetails_WhenValidRequest()
        {
            // Arrange
            var emisUserSession = _fixture.Create<EmisUserSession>();
            var addVerificationRequest = _fixture.Create<AddVerificationRequest>();
            var expectedResponse = _fixture.Create<AddVerificationResponse>();

            var emisRequestParameters = new EmisRequestParameters(emisUserSession);

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, emisUserSession.EndUserSessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "me/verifications")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(addVerificationRequest))
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.VerificationPost(emisRequestParameters, addVerificationRequest);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task PatientMessagesGet_ReturnsAMessagesGetResponse_ForValidRequest()
        {
            // Arrange
            var userPatientLinkToken = _fixture.Create<string>();
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();

            var expectedResponse = _fixture.Create<MessagesGetResponse>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(EmisClient.HeaderSessionId, sessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Get, "messages?userPatientLinkToken=" + userPatientLinkToken)
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.PatientMessagesGet(new EmisRequestParameters
            {
                SessionId = sessionId,
                EndUserSessionId = endUserSessionId,
                UserPatientLinkToken = userPatientLinkToken
            });

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task PatientMessageDetailsGet_ReturnsAMessageGetResponse_ForValidRequest()
        {
            // Arrange
            var userPatientLinkToken = _fixture.Create<string>();
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();

            var expectedResponse = _fixture.Create<MessageGetResponse>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(EmisClient.HeaderSessionId, sessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Get, "messages/1/?userPatientLinkToken=" + userPatientLinkToken)
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.PatientMessageDetailsGet("1", new EmisRequestParameters
            {
                SessionId = sessionId,
                EndUserSessionId = endUserSessionId,
                UserPatientLinkToken = userPatientLinkToken
            });

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task PatientMessageUpdatePut_ReturnsAMessageUpdateResponse_ForValidRequest()
        {
            // Arrange
            var userPatientLinkToken = _fixture.Create<string>();
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();

            var expectedResponse = _fixture.Create<MessageUpdateResponse>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(EmisClient.HeaderSessionId, sessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Put, "messages")
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.PatientMessageUpdatePut(new EmisRequestParameters
            {
                SessionId = sessionId,
                EndUserSessionId = endUserSessionId,
                UserPatientLinkToken = userPatientLinkToken
            }, new UpdateMessageReadStatusRequest
            {
                MessageId = 1,
                MessageReadState = "Read"
            });

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task PatientMessageDelete_ReturnsAMessageDeleteResponse_ForValidRequest()
        {
            // Arrange
            var userPatientLinkToken = _fixture.Create<string>();
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();

            var expectedResponse = _fixture.Create<MessageDeleteResponse>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(EmisClient.HeaderSessionId, sessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Delete, "messages/1")
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.PatientPracticeMessageDelete(new EmisRequestParameters
            {
                SessionId = sessionId,
                EndUserSessionId = endUserSessionId,
                UserPatientLinkToken = userPatientLinkToken
            }, "1");

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task PatientMessageRecipientsGet_ReturnsAMessageRecipientsGetResponse_ForValidRequest()
        {
            // Arrange
            var userPatientLinkToken = _fixture.Create<string>();
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();

            var expectedResponse = _fixture.Create<MessageRecipientsGetResponse>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(EmisClient.HeaderSessionId, sessionId)
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Get, "messagerecipients?userPatientLinkToken=" + userPatientLinkToken)
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.PatientMessageRecipientsGet(new EmisRequestParameters
            {
                SessionId = sessionId,
                EndUserSessionId = endUserSessionId,
                UserPatientLinkToken = userPatientLinkToken
            });

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task PatientMessagePost_ReturnsAMessagePostResponse_ForValidRequest()
        {
            // Arrange
            var userPatientLinkToken = _fixture.Create<string>();
            var sessionId = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();
            var createPatientMessage = _fixture.Create<CreatePatientMessage>();

            var expectedRequestContent = new PostMessageRequest(userPatientLinkToken, createPatientMessage);
            var expectedResponse = _fixture.Create<MessagePostResponse>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(EmisClient.HeaderSessionId, sessionId),
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "messages")
                .WithContent(JsonConvert.SerializeObject(expectedRequestContent))
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await _systemUnderTest.PatientMessagePost(new EmisRequestParameters
            {
                SessionId = sessionId,
                EndUserSessionId = endUserSessionId,
                UserPatientLinkToken = userPatientLinkToken
            }, createPatientMessage);

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
            await _systemUnderTest.SessionsEndUserSessionPost();

            // Assert
            VerifyCustomTimeoutPresentInRequest();
        }

        [TestMethod]
        public async Task NhsUserPost_VerifyCustomTimeoutHeaderPresent()
        {
            // Arrange
            SetupMockedHandlerEmisForEmisCustomTimeout();

            var emisUserSession = _fixture.Create<EmisUserSession>();
            var emisRequestParameters = new EmisRequestParameters(emisUserSession);
            var requestBody = _fixture.Create<AddNhsUserRequest>();

            // Act
            await _systemUnderTest.NhsUserPost(emisRequestParameters, requestBody);

            // Assert
            VerifyCustomTimeoutPresentInRequest();
        }

        [TestMethod]
        public async Task VerificationPost_VerifyCustomTimeoutHeaderPresent()
        {
            // Arrange
            SetupMockedHandlerEmisForEmisCustomTimeout();

            var emisUserSession = _fixture.Create<EmisUserSession>();
            var addVerificationRequest = _fixture.Create<AddVerificationRequest>();
            var emisRequestParameters = new EmisRequestParameters(emisUserSession);

            // Act
            await _systemUnderTest.VerificationPost(emisRequestParameters, addVerificationRequest);

            // Assert
            VerifyCustomTimeoutPresentInRequest();
        }

        [TestMethod]
        public async Task MeApplicationsPost_VerifyCustomTimeoutHeaderPresent()
        {
            // Arrange
            var endUserSessionId = _fixture.Create<string>();
            var requestBody = _fixture.Create<MeApplicationsPostRequest>();

            SetupMockedHandlerEmisForEmisCustomTimeout();

            // Act
            await _systemUnderTest.MeApplicationsPost(endUserSessionId, requestBody);

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
            await _systemUnderTest.CoursesGet(  new EmisRequestParameters
            {
                SessionId = sessionId,
                EndUserSessionId = endUserSessionId,
                UserPatientLinkToken = userPatientLinkToken
            });

            // Assert
            _mockedHandler.Protected().Verify(
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
            var mockedResponse = _fixture.Create<CoursesGetResponse>().SerializeJson();
            _mockedHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _mockedHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(mockedResponse),
                })
                .Verifiable();

            var httpClient = new EmisHttpClient(new HttpClient(_mockedHandler.Object), _emisConfig);

            _fixture.Inject(_emisConfig);
            _fixture.Inject(httpClient);

            _systemUnderTest = _fixture.Create<EmisClient>();
        }

        private void VerifyCustomTimeoutPresentInRequest()
        {
            _mockedHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Properties.Count == 1
                    && (int)req.Properties[HttpRequestConstants.CustomTimeout]
                    == _emisConfig.EmisExtendedHttpTimeoutSeconds
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