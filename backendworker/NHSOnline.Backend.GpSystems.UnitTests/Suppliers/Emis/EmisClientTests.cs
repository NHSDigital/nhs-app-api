using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;
using NHSOnline.Backend.Support.Http;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis
{
    [TestClass]
    public sealed class EmisClientTests
    {
        [TestMethod]
        public async Task SessionsEndUserSessionPost_ReturnsAnEndUserSessionId_WhenValidlyRequested()
        {
            // Arrange
            using var context = new EmisClientTestsContext();
            var expectedEndUserSessionResponse = new SessionsEndUserSessionPostResponse();

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "sessions/endusersession")
                .WithEmisHeaders()
                .Respond("application/json", JsonConvert.SerializeObject(expectedEndUserSessionResponse));

            // Act
            var response = await context.SystemUnderTest.SessionsEndUserSessionPost();

            // Assert
            response.Body.Should().BeEquivalentTo(expectedEndUserSessionResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ExceptionErrorResponse.Should().BeNull();
        }

        [TestMethod]
        public async Task SessionsEndUserSessionPost_VerifyCustomTimeoutHeaderPresent()
        {
            // Arrange
            using var context = new EmisClientTestsContext(emisExtendedHttpTimeoutSeconds: 6);

            HttpRequestMessage requestMessage = null;
            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "sessions/endusersession")
                .With(request =>
                {
                    requestMessage = request;
                    return true;
                })
                .Respond("application/json", JsonConvert.SerializeObject(new SessionsEndUserSessionPostResponse()));

            // Act
            await context.SystemUnderTest.SessionsEndUserSessionPost();

            // Assert
            requestMessage.Should().NotBeNull();
            requestMessage.Properties.Should()
                .BeEquivalentTo(new Dictionary<string, object> { { "customTimeout", 6 } });
        }

        [TestMethod]
        public async Task SessionsPost_ReturnsASessionResponse_WhenValidlyRequested()
        {
            // Arrange
            using var context = new EmisClientTestsContext();
            var endUserSessionId = "end user session id";
            var expectedResponse = new SessionsPostResponse();
            var requestBody = new SessionsPostRequest();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId)
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "sessions")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(requestBody))
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.SessionsPost(endUserSessionId, requestBody);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task MeApplicationsPost_ReturnsAnApplicationsPostResponse_WhenValidlyRequested()
        {
            // Arrange
            using var context = new EmisClientTestsContext();
            var endUserSessionId = "end user session id";
            var expectedResponse = new MeApplicationsPostResponse();
            var requestBody = new MeApplicationsPostRequest();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId)
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "me/applications")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(requestBody))
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.MeApplicationsPost(endUserSessionId, requestBody);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task MeApplicationsPost_VerifyCustomTimeoutHeaderPresent()
        {
            // Arrange
            using var context = new EmisClientTestsContext(emisExtendedHttpTimeoutSeconds: 6);

            HttpRequestMessage requestMessage = null;
            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "me/applications")
                .With(request =>
                {
                    requestMessage = request;
                    return true;
                })
                .Respond("application/json", JsonConvert.SerializeObject(new MeApplicationsPostResponse()));

            // Act
            await context.SystemUnderTest.MeApplicationsPost("end user session id", new MeApplicationsPostRequest());

            // Assert
            requestMessage.Should().NotBeNull();
            requestMessage.Properties.Should()
                .BeEquivalentTo(new Dictionary<string, object> { { "customTimeout", 6 } });
        }

        [TestMethod]
        public async Task
            MeApplicationsPost_ReturnsAnApplicationsPostResponseWithErrorDetails_WhenUserAlreadyRegistered()
        {
            // Arrange
            using var context = new EmisClientTestsContext();
            var endUserSessionId = "end user session id";
            var expectedResponse = new ExceptionErrorResponse();
            var requestBody = new MeApplicationsPostRequest();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId)
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "me/applications")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(requestBody))
                .Respond(HttpStatusCode.InternalServerError, "application/json",
                    JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.MeApplicationsPost(endUserSessionId, requestBody);

            // Assert
            response.ExceptionErrorResponse.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(500);
        }

        [TestMethod]
        public async Task MeSettingsGet_ReturnsUserSettingsGetResponse_WhenValidlyRequested()
        {
            var context = new EmisClientTestsContext();
            var sessionId = "session id";
            var endUserSessionId = "end user session id";
            var expectedResponse = new MeSettingsGetResponse();

            var emisUserSession = new EmisUserSession
            {
                EndUserSessionId = endUserSessionId,
                SessionId = sessionId,
            };

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>("X-API-SessionId", sessionId),
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Get, "me/settings")
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await context.SystemUnderTest.MeSettingsGet(new EmisRequestParameters(emisUserSession));

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task DemographicsGet_ReturnsADemographicsResponse_WhenValidlyRequested()
        {
            // Arrange
            using var context = new EmisClientTestsContext();
            var userPatientLinkToken = "user patient link token";
            var sessionId = "session id";
            var endUserSessionId = "end user session id";

            var expectedResponse = new DemographicsGetResponse();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>("X-API-SessionId", sessionId),
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Get, "demographics?userPatientLinkToken=" + userPatientLinkToken)
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.DemographicsGet(new EmisRequestParameters
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
            using var context = new EmisClientTestsContext();
            var endUserSessionId = "end user session id";
            var requestBody = new SessionsPostRequest();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId)
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "sessions")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(requestBody))
                .Respond(HttpStatusCode.Forbidden);

            // Act
            var response = await context.SystemUnderTest.SessionsPost(endUserSessionId, requestBody);

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
            using var context = new EmisClientTestsContext();
            var endUserSessionId = "end user session id";
            var requestBody = new SessionsPostRequest();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId)
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "sessions")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(requestBody))
                .Respond(HttpStatusCode.Forbidden, "application/json", string.Empty);

            // Act
            var response = await context.SystemUnderTest.SessionsPost(endUserSessionId, requestBody);

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
            using var context = new EmisClientTestsContext();
            var endUserSessionId = "end user session id";
            var requestBody = new SessionsPostRequest();
            var expectedResponse = new BadRequestErrorResponse();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId)
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "sessions")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(requestBody))
                .Respond(HttpStatusCode.BadRequest, "application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.SessionsPost(endUserSessionId, requestBody);

            // Assert
            response.ExceptionErrorResponse.Should().BeNull();
            response.ErrorResponseBadRequest.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task AllergiesGet_ReturnsAnAllergiesResponse_WhenValidlyRequested()
        {
            // Arrange
            using var context = new EmisClientTestsContext();
            var userPatientLinkToken = "user patient link token";
            var sessionId = "session id";
            var endUserSessionId = "end user session id";

            var expectedResponse = new MedicationRootObject();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>("X-API-SessionId", sessionId),
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Get, "record?userPatientLinkToken=" + userPatientLinkToken + "&itemType=Allergies")
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.MedicalRecordGet(
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
            using var context = new EmisClientTestsContext();
            var userPatientLinkToken = "user patient link token";
            var sessionId = "session id";
            var endUserSessionId = "end user session id";
            var fromDateTime = new DateTimeOffset();
            var toDateTime = new DateTimeOffset();

            var expectedResponse = new PrescriptionRequestsGetResponse();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>("X-API-SessionId", sessionId),
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Get,
                    "prescriptionrequests?userPatientLinkToken=" + userPatientLinkToken + "&filterFromDate="
                    + HttpUtility.UrlEncode(fromDateTime.ToString("O", CultureInfo.InvariantCulture)) + "&filterToDate="
                    + HttpUtility.UrlEncode(toDateTime.ToString("O", CultureInfo.InvariantCulture)))
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.PrescriptionsGet(
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
            using var context = new EmisClientTestsContext();
            var userPatientLinkToken = "user patient link token";
            var sessionId = "session id";
            var endUserSessionId = "end user session id";

            var expectedResponse = new CoursesGetResponse();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>("X-API-SessionId", sessionId),
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Get, "courses?userPatientLinkToken=" + userPatientLinkToken)
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.CoursesGet(new EmisRequestParameters
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
        public async Task CoursesGet_VerifyDefaultTimeIsUsed()
        {
            // Arrange
            using var context = new EmisClientTestsContext(emisExtendedHttpTimeoutSeconds: 6);

            var userPatientLinkToken = "user patient link token";

            HttpRequestMessage requestMessage = null;
            context.MockHttpHandler
                .WhenEmis(HttpMethod.Get, "courses?userPatientLinkToken=" + userPatientLinkToken)
                .With(request =>
                {
                    requestMessage = request;
                    return true;
                })
                .Respond("application/json", JsonConvert.SerializeObject(new CoursesGetResponse()));

            // Act
            await context.SystemUnderTest.CoursesGet(new EmisRequestParameters
            {
                UserPatientLinkToken = userPatientLinkToken
            });

            // Assert
            requestMessage.Should().NotBeNull();
            requestMessage.Properties.Should().BeEmpty();
        }

        [TestMethod]
        public async Task PrescriptionsPost_ReturnsValidResponse_WhenValidlyRequested()
        {
            // Arrange
            using var context = new EmisClientTestsContext();
            var sessionId = "session id";
            var endUserSessionId = "end user session id";
            var prescriptionPostRequest = new PrescriptionRequestsPost();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>("X-API-SessionId", sessionId),
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "prescriptionrequests")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(prescriptionPostRequest))
                .Respond("application/json", "{}");

            // Act
            var response = await context.SystemUnderTest.PrescriptionsPost(
                sessionId,
                endUserSessionId,
                prescriptionPostRequest);

            // Assert
            response.Body.Should().BeOfType<PrescriptionRequestPostResponse>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ExceptionErrorResponse.Should().BeNull();
        }

        [TestMethod]
        public async Task AppointmentsPost_ReturnsValidResponse_WhenValidlyRequested()
        {
            // Arrange
            using var context = new EmisClientTestsContext();
            var sessionId = "session id";
            var endUserSessionId = "end user session id";
            var userPatientLinkToken = "user patient link token";
            var appointmentBookRequest = new AppointmentBookRequest();
            appointmentBookRequest.SlotId = "21";

            var expectedRequest = new BookAppointmentSlotPostRequest(userPatientLinkToken, appointmentBookRequest);

            var expectedResponse = new BookAppointmentSlotPostResponse
            {
                BookingCreated = true
            };

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(
                    "X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>(
                    "X-API-SessionId", sessionId),
            };

            context.MockHttpHandler
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
            var response = await context.SystemUnderTest.AppointmentsPost(
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
            using var context = new EmisClientTestsContext();
            var sessionId = "session id";
            var endUserSessionId = "end user session id";
            var userPatientLinkToken = "user patient link token";
            var slotId = 99;
            var cancellationReason = "cancellation reason";

            var expectedRequest = new CancelAppointmentDeleteRequest(userPatientLinkToken, cancellationReason, slotId);

            var expectedResponse = new CancelAppointmentDeleteResponse
            {
                IsCancelled = true
            };

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(
                    "X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>(
                    "X-API-SessionId", sessionId),
            };

            context.MockHttpHandler
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
            var response = await context.SystemUnderTest.AppointmentsDelete(
                emisRequestParameters,
                slotId,
                new CancellationReason { DisplayName = cancellationReason });

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ExceptionErrorResponse.Should().BeNull();
        }

        [TestMethod]
        public async Task NhsUserPost_ReturnsLinkageDetails_WhenValidRequest()
        {
            // Arrange
            using var context = new EmisClientTestsContext();
            var endUserSessionId = "end user session id";
            var requestBody = new AddNhsUserRequest();
            var expectedResponse = new AddNhsUserResponse();

            var emisUserSession = new EmisUserSession
            {
                EndUserSessionId = endUserSessionId,
            };

            var emisRequestParameters = new EmisRequestParameters(emisUserSession);

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId),
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "users/nhs")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(requestBody))
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.NhsUserPost(emisRequestParameters, requestBody);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task NhsUserPost_VerifyCustomTimeoutHeaderPresent()
        {
            // Arrange
            using var context = new EmisClientTestsContext(emisExtendedHttpTimeoutSeconds: 6);

            var emisUserSession = new EmisUserSession { EndUserSessionId = "end user session id" };
            var emisRequestParameters = new EmisRequestParameters(emisUserSession);
            var requestBody = new AddNhsUserRequest();

            HttpRequestMessage requestMessage = null;
            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "users/nhs")
                .With(request =>
                {
                    requestMessage = request;
                    return true;
                })
                .Respond("application/json", JsonConvert.SerializeObject(new AddNhsUserResponse()));

            // Act
            await context.SystemUnderTest.NhsUserPost(emisRequestParameters, requestBody);

            // Assert
            requestMessage.Should().NotBeNull();
            requestMessage.Properties.Should()
                .BeEquivalentTo(new Dictionary<string, object> { { "customTimeout", 6 } });
        }

        [TestMethod]
        public async Task VerificationsPost_ReturnsLinkageDetails_WhenValidRequest()
        {
            // Arrange
            using var context = new EmisClientTestsContext();
            var emisUserSession = new EmisUserSession { EndUserSessionId = "end user session id" };
            var addVerificationRequest = new AddVerificationRequest();
            var expectedResponse = new AddVerificationResponse();

            var emisRequestParameters = new EmisRequestParameters(emisUserSession);

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", emisUserSession.EndUserSessionId),
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "me/verifications")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(addVerificationRequest))
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response =
                await context.SystemUnderTest.VerificationPost(emisRequestParameters, addVerificationRequest);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
            response.ExceptionErrorResponse.Should().Be(null);
        }

        [TestMethod]
        public async Task VerificationPost_VerifyCustomTimeoutHeaderPresent()
        {
            // Arrange
            using var context = new EmisClientTestsContext(emisExtendedHttpTimeoutSeconds: 6);

            var emisUserSession = new EmisUserSession();
            var addVerificationRequest = new AddVerificationRequest();
            var emisRequestParameters = new EmisRequestParameters(emisUserSession);

            HttpRequestMessage requestMessage = null;
            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "me/verifications")
                .With(request =>
                {
                    requestMessage = request;
                    return true;
                })
                .Respond("application/json", JsonConvert.SerializeObject(new AddVerificationResponse()));

            // Act
            await context.SystemUnderTest.VerificationPost(emisRequestParameters, addVerificationRequest);

            // Assert
            requestMessage.Should().NotBeNull();
            requestMessage.Properties.Should()
                .BeEquivalentTo(new Dictionary<string, object> { { "customTimeout", 6 } });
        }

        [TestMethod]
        public async Task PatientMessagesGet_ReturnsAMessagesGetResponse_ForValidRequest()
        {
            // Arrange
            using var context = new EmisClientTestsContext();
            var userPatientLinkToken = "user patient link token";
            var sessionId = "session id";
            var endUserSessionId = "end user session id";

            var expectedResponse = new MessagesGetResponse();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>("X-API-SessionId", sessionId),
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Get, "messages?userPatientLinkToken=" + userPatientLinkToken)
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.PatientMessagesGet(new EmisRequestParameters
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
            using var context = new EmisClientTestsContext();
            var userPatientLinkToken = "user patient link token";
            var sessionId = "session id";
            var endUserSessionId = "end user session id";

            var expectedResponse = new MessageGetResponse();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>("X-API-SessionId", sessionId),
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Get, "messages/1/?userPatientLinkToken=" + userPatientLinkToken)
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.PatientMessageDetailsGet("1", new EmisRequestParameters
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
            using var context = new EmisClientTestsContext();
            var userPatientLinkToken = "user patient link token";
            var sessionId = "session id";
            var endUserSessionId = "end user session id";

            var expectedResponse = new MessageUpdateResponse();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>("X-API-SessionId", sessionId),
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Put, "messages")
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.PatientMessageUpdatePut(new EmisRequestParameters
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
            using var context = new EmisClientTestsContext();
            var userPatientLinkToken = "user patient link token";
            var sessionId = "session id";
            var endUserSessionId = "end user session id";

            var expectedResponse = new MessageDeleteResponse();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>("X-API-SessionId", sessionId),
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Delete, "messages/1")
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.PatientPracticeMessageDelete(new EmisRequestParameters
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
            using var context = new EmisClientTestsContext();
            var userPatientLinkToken = "user patient link token";
            var sessionId = "session id";
            var endUserSessionId = "end user session id";

            var expectedResponse = new MessageRecipientsResponse();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>("X-API-SessionId", sessionId)
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Get, "messagerecipients?userPatientLinkToken=" + userPatientLinkToken)
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.PatientMessageRecipientsGet(new EmisRequestParameters
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
            using var context = new EmisClientTestsContext();
            var userPatientLinkToken = "user patient link token";
            var sessionId = "session id";
            var endUserSessionId = "end user session id";
            var createPatientMessage = new CreatePatientMessage();

            var expectedRequestContent = new PostMessageRequest(userPatientLinkToken, createPatientMessage);
            var expectedResponse = new MessagePostResponse();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>("X-API-SessionId", sessionId),
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "messages")
                .WithContent(JsonConvert.SerializeObject(expectedRequestContent))
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            // Act
            var response = await context.SystemUnderTest.PatientMessagePost(new EmisRequestParameters
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
        public async Task PatientMessagePost_EmisReturnsUnauthorised_UnauthorisedExceptionThrown()
        {
            // Arrange
            using var context = new EmisClientTestsContext();
            var userPatientLinkToken = "user patient link token";
            var sessionId = "session id";
            var endUserSessionId = "end user session id";
            var createPatientMessage = new CreatePatientMessage();

            var expectedRequestContent = new PostMessageRequest(userPatientLinkToken, createPatientMessage);

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("X-API-EndUserSessionId", endUserSessionId),
                new KeyValuePair<string, string>("X-API-SessionId", sessionId),
            };

            context.MockHttpHandler
                .WhenEmis(HttpMethod.Post, "messages")
                .WithContent(JsonConvert.SerializeObject(expectedRequestContent))
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", "Missing or invalid EndUserSessionId");

            // Act
            Func<Task<EmisApiObjectResponse<MessagePostResponse>>> act = async () =>
                await context.SystemUnderTest.PatientMessagePost(new EmisRequestParameters
                {
                    SessionId = sessionId,
                    EndUserSessionId = endUserSessionId,
                    UserPatientLinkToken = userPatientLinkToken
                }, createPatientMessage);

            // Assert

            await act.Should().ThrowAsync<UnauthorisedGpSystemHttpRequestException>();
        }
    }
}