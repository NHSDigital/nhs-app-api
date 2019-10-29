using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Prescriptions
{
    [TestClass]
    public class EmisPrescriptionServiceTests
    {
        private EmisPrescriptionService _systemUnderTest;
        private Mock<IEmisClient> _emisClient;
        private Mock<IEmisPrescriptionMapper> _emisPrescriptionMapper;
        private EmisConfigurationSettings _settings;
        private EmisUserSession _emisUserSession;
        private IFixture _fixture;
        private RepeatPrescriptionRequest _repeatPrescriptionRequest;
        private Guid _patientId;
        private const string DefaultEmisVersion = "2.1.0.0";
        private static readonly string DefaultEmisApplicationId = Guid.NewGuid().ToString();
        private static readonly Uri BaseUri = new Uri("http://emis_base_url/");
        private const string CertificatePath = "CertificatePath";
        private const string CertificatePassphrase = "CerticiatePassphrase";
        private const int EmisExtendedHttpTimeoutSeconds = 6;
        private const int DefaultHttpTimeoutSeconds = 2;
        private const int PrescriptionsMaxCoursesSoftLimit = 100;
        private const int CoursesMaxCoursesLimit = 100;
        private const string Environment = "testEnv";

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _emisUserSession = _fixture.Create<EmisUserSession>();
            _emisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _emisPrescriptionMapper = _fixture.Freeze<Mock<IEmisPrescriptionMapper>>();
            
            _settings = new EmisConfigurationSettings(BaseUri, DefaultEmisApplicationId, DefaultEmisVersion, CertificatePath, 
                CertificatePassphrase, EmisExtendedHttpTimeoutSeconds, DefaultHttpTimeoutSeconds, CoursesMaxCoursesLimit, PrescriptionsMaxCoursesSoftLimit, 
                Environment);
            _fixture.Inject(_settings);
            _systemUnderTest = _fixture.Create<EmisPrescriptionService>();
            _repeatPrescriptionRequest = new RepeatPrescriptionRequest
            {
                CourseIds = new List<string>
                {
                    "766ecd82-3008-4454-95a5-98c423ce0527",
                    "766ecd82-3008-4454-95a5-98c423ce0527"
                }
            };
        }

        #region Get Prescriptions

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;

            var prescriptionsResponse = _fixture.Create<PrescriptionRequestsGetResponse>();

            _emisClient.Setup(x => x.PrescriptionsGet(
                    It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                    date, toDate))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            // Act
            var result = await _systemUnderTest.GetPrescriptions(
                new GpLinkedAccountModel(_emisUserSession, _patientId), date, toDate);

            // Assert
            _emisClient.Verify(x => x.PrescriptionsGet(
                It.Is<EmisRequestParameters>(
                    e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                         e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                date, toDate));

            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task
            Get_PrescriptionsInResponseAreFilteredSoOnlyPrescriptionsWithRepeatCoursesWhichReturned_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;

            var repeatCourseGuid = Guid.NewGuid().ToString();
            var nonRepeatCourseGuid1 = Guid.NewGuid().ToString();
            var nonRepeatCourseGuid2 = Guid.NewGuid().ToString();

            var prescriptionsResponse = new PrescriptionRequestsGetResponse
            {
                PrescriptionRequests = new List<PrescriptionRequest>
                {
                    new PrescriptionRequest
                    {
                        RequestedMedicationCourses = new List<RequestedMedicationCourse>
                        {
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseGuid = repeatCourseGuid,
                            },
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseGuid = nonRepeatCourseGuid1,
                            }
                        }
                    },
                    new PrescriptionRequest
                    {
                        RequestedMedicationCourses = new List<RequestedMedicationCourse>
                        {
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseGuid = nonRepeatCourseGuid2,
                            },
                        }
                    }
                },
                MedicationCourses = new List<MedicationCourse>
                {
                    new MedicationCourse
                    {
                        MedicationCourseGuid = repeatCourseGuid,
                        PrescriptionType = PrescriptionType.Repeat,
                    },
                    new MedicationCourse
                    {
                        MedicationCourseGuid = nonRepeatCourseGuid1,
                        PrescriptionType = PrescriptionType.Acute,
                    },
                    new MedicationCourse
                    {
                        MedicationCourseGuid = nonRepeatCourseGuid2,
                        PrescriptionType = PrescriptionType.Automatic,
                    }
                }
            };

            _emisClient.Setup(x => x.PrescriptionsGet(
                It.Is<EmisRequestParameters>(
                    e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                         e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                date, toDate))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            var response = new PrescriptionListResponse();
            PrescriptionRequestsGetResponse capturedItemToMap = null;
            _emisPrescriptionMapper.Setup(x => x.Map(It.IsAny<PrescriptionRequestsGetResponse>())).Returns(response)
                .Callback<PrescriptionRequestsGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetPrescriptions(
                new GpLinkedAccountModel(_emisUserSession, _patientId), date, toDate);

            // Assert
            _emisClient.Verify(x => x.PrescriptionsGet(It.Is<EmisRequestParameters>(
                    e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                         e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                date, toDate));
            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>()
                .Subject.Response.Should().Be(response);

            capturedItemToMap.PrescriptionRequests.Should().HaveCount(1);
            capturedItemToMap.PrescriptionRequests.ElementAt(0).RequestedMedicationCourses.Should().HaveCount(1);
            capturedItemToMap.PrescriptionRequests.ElementAt(0).RequestedMedicationCourses.ElementAt(0)
                .RequestedMedicationCourseGuid.Should().Be(repeatCourseGuid);
            capturedItemToMap.MedicationCourses.Should().HaveCount(1);
            capturedItemToMap.MedicationCourses.ElementAt(0).MedicationCourseGuid.Should().Be(repeatCourseGuid);
        }

        [TestMethod]
        public async Task
            Get_PrescriptionsInResponseAreOrderedByDateRequestedDescending_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;

            var repeatCourseGuidExpectedFirst = Guid.NewGuid().ToString();
            var repeatCourseGuidExpectedSecond = Guid.NewGuid().ToString();
            var repeatCourseGuidExpectedThird = Guid.NewGuid().ToString();

            var prescriptionsResponse = new PrescriptionRequestsGetResponse
            {
                PrescriptionRequests = new List<PrescriptionRequest>
                {
                    new PrescriptionRequest
                    {
                        DateRequested = new DateTimeOffset(new DateTime(2000, 1, 2)),
                        RequestedMedicationCourses = new List<RequestedMedicationCourse>
                        {
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseGuid = repeatCourseGuidExpectedSecond,
                            }
                        }
                    },
                    new PrescriptionRequest
                    {
                        DateRequested = new DateTimeOffset(new DateTime(2000, 1, 1)),
                        RequestedMedicationCourses = new List<RequestedMedicationCourse>
                        {
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseGuid = repeatCourseGuidExpectedThird,
                            },
                        }
                    },
                    new PrescriptionRequest
                    {
                        DateRequested = new DateTimeOffset(new DateTime(2000, 1, 3)),
                        RequestedMedicationCourses = new List<RequestedMedicationCourse>
                        {
                            new RequestedMedicationCourse
                            {
                                RequestedMedicationCourseGuid = repeatCourseGuidExpectedFirst,
                            },
                        }
                    },
                },
                MedicationCourses = new List<MedicationCourse>
                {
                    new MedicationCourse
                    {
                        MedicationCourseGuid = repeatCourseGuidExpectedThird,
                        PrescriptionType = PrescriptionType.Repeat,
                    },
                    new MedicationCourse
                    {
                        MedicationCourseGuid = repeatCourseGuidExpectedFirst,
                        PrescriptionType = PrescriptionType.Repeat,
                    },
                    new MedicationCourse
                    {
                        MedicationCourseGuid = repeatCourseGuidExpectedSecond,
                        PrescriptionType = PrescriptionType.Repeat,
                    }
                }
            };

            _emisClient.Setup(x => x.PrescriptionsGet(
                It.Is<EmisRequestParameters>(
                    e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                         e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                date, toDate))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            var response = new PrescriptionListResponse();
            PrescriptionRequestsGetResponse capturedItemToMap = null;
            _emisPrescriptionMapper.Setup(x => x.Map(It.IsAny<PrescriptionRequestsGetResponse>())).Returns(response)
                .Callback<PrescriptionRequestsGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetPrescriptions(
                new GpLinkedAccountModel(_emisUserSession, _patientId), date, toDate);

            // Assert
            _emisClient.Verify(x => x.PrescriptionsGet(It.Is<EmisRequestParameters>(
                    e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                         e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                date, toDate));

            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>()
                .Subject.Response.Should().Be(response);

            capturedItemToMap.PrescriptionRequests.Should().HaveCount(3);

            capturedItemToMap.PrescriptionRequests.ElementAt(0).RequestedMedicationCourses.Should().HaveCount(1);
            capturedItemToMap.PrescriptionRequests.ElementAt(0).RequestedMedicationCourses.ElementAt(0)
                .RequestedMedicationCourseGuid.Should().Be(repeatCourseGuidExpectedFirst);

            capturedItemToMap.PrescriptionRequests.ElementAt(1).RequestedMedicationCourses.Should().HaveCount(1);
            capturedItemToMap.PrescriptionRequests.ElementAt(1).RequestedMedicationCourses.ElementAt(0)
                .RequestedMedicationCourseGuid.Should().Be(repeatCourseGuidExpectedSecond);

            capturedItemToMap.PrescriptionRequests.ElementAt(2).RequestedMedicationCourses.Should().HaveCount(1);
            capturedItemToMap.PrescriptionRequests.ElementAt(2).RequestedMedicationCourses.ElementAt(0)
                .RequestedMedicationCourseGuid.Should().Be(repeatCourseGuidExpectedThird);
        }

        [DataTestMethod]
        [DataRow(PrescriptionsMaxCoursesSoftLimit + 1, PrescriptionsMaxCoursesSoftLimit)]
        [DataRow(PrescriptionsMaxCoursesSoftLimit, PrescriptionsMaxCoursesSoftLimit)]
        [DataRow(PrescriptionsMaxCoursesSoftLimit - 1, PrescriptionsMaxCoursesSoftLimit - 1)]
        public async Task Get_PrescriptionsInResponseAreLimitedToMax_WhenSuccessfulResponseFromEmis(
            int numberOfCoursesToCreate, int expectedNumberOfPrescriptions)
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;

            var prescriptionRequests = new List<PrescriptionRequest>();
            var medicationCourses = new List<MedicationCourse>();

            for (var i = 0; i < numberOfCoursesToCreate; i++)
            {
                var courseGuid = Guid.NewGuid().ToString();

                prescriptionRequests.Add(new PrescriptionRequest
                {
                    DateRequested = new DateTimeOffset(new DateTime(2000, 1, 2)),
                    RequestedMedicationCourses = new List<RequestedMedicationCourse>
                    {
                        new RequestedMedicationCourse
                        {
                            RequestedMedicationCourseGuid = courseGuid,
                        }
                    }
                });

                medicationCourses.Add(new MedicationCourse
                {
                    MedicationCourseGuid = courseGuid,
                    PrescriptionType = PrescriptionType.Repeat,
                });
            }

            var prescriptionsResponse = new PrescriptionRequestsGetResponse
            {
                PrescriptionRequests = prescriptionRequests,
                MedicationCourses = medicationCourses,
            };

            _emisClient.Setup(x => x.PrescriptionsGet(It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                    date, toDate))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = prescriptionsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null,
                    }));

            var response = new PrescriptionListResponse();
            PrescriptionRequestsGetResponse capturedItemToMap = null;
            _emisPrescriptionMapper.Setup(x => x.Map(It.IsAny<PrescriptionRequestsGetResponse>())).Returns(response)
                .Callback<PrescriptionRequestsGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetPrescriptions(
                new GpLinkedAccountModel(_emisUserSession, _patientId), date, toDate);

            // Assert
            _emisClient.Verify(x => x.PrescriptionsGet(It.Is<EmisRequestParameters>(
                    e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                         e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                date, toDate));

            result.Should().BeAssignableTo<GetPrescriptionsResult.Success>()
                .Subject.Response.Should().Be(response);

            capturedItemToMap.PrescriptionRequests.Should().HaveCount(expectedNumberOfPrescriptions);
        }

        [TestMethod]
        public async Task Get_ReturnsSupplierSystemUnavailable_WhenErrorReceivedFromEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;

            _emisClient.Setup(x => x.PrescriptionsGet(It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                    date, toDate))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>(HttpStatusCode
                            .InternalServerError)
                        { ExceptionErrorResponse = errorResponse }));

            // Act
            var result = await _systemUnderTest.GetPrescriptions(
                new GpLinkedAccountModel(_emisUserSession, _patientId), date, toDate);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.BadGateway>();
        }

        [TestMethod]
        public async Task Get_ReturnsBadGateway_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;

            _emisClient.Setup(x => x.PrescriptionsGet(It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                    date, toDate))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetPrescriptions(
                new GpLinkedAccountModel(_emisUserSession, _patientId), date, toDate);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.BadGateway>();
            _emisClient.Verify();
        }

        [TestMethod]
        public async Task Get_ReturnsInternalServerError_WhenNullExceptionOccursCallingEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;

            _emisClient.Setup(x => x.PrescriptionsGet(It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                    date, toDate))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = null,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            // Act
            var result = await _systemUnderTest.GetPrescriptions(
                new GpLinkedAccountModel(_emisUserSession, _patientId), date, toDate);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.InternalServerError>();
            _emisClient.Verify();
        }

        [TestMethod]
        public async Task Get_ReturnsForbidden_WhenErrorReceivedFromEmis()
        {
            // Arrange
            var date = DateTimeOffset.Now;
            var toDate = DateTimeOffset.Now;

            var notEnabledError = EmisApiErrorMessages.EmisService_NotEnabledForUser;
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = notEnabledError;

            _emisClient.Setup(x => x.PrescriptionsGet(It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                    date, toDate))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestsGetResponse>(HttpStatusCode.Forbidden)
                        { ExceptionErrorResponse = errorResponse }));

            // Act
            var result = await _systemUnderTest.GetPrescriptions(
                new GpLinkedAccountModel(_emisUserSession, _patientId), date, toDate);

            // Assert
            result.Should().BeAssignableTo<GetPrescriptionsResult.Forbidden>();
        }

        #endregion

        #region Post Prescription

        [TestMethod]
        public async Task Post_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            _emisClient.Setup(x => x.PrescriptionsPost(_emisUserSession.SessionId,
                    _emisUserSession.EndUserSessionId, It.IsAny<PrescriptionRequestsPost>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestPostResponse>(HttpStatusCode.OK)));

            // Act
            var result = await _systemUnderTest.OrderPrescription(new GpLinkedAccountModel(_emisUserSession, _patientId), _repeatPrescriptionRequest);

            // Assert
            _emisClient.Verify(x => x.PrescriptionsPost(_emisUserSession.SessionId,
                _emisUserSession.EndUserSessionId, It.IsAny<PrescriptionRequestsPost>()));

            result.Should().BeAssignableTo<OrderPrescriptionResult.Success>()
                .Subject.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Post_ReturnsConflict_WhenErrorReceivedFromEmis()
        {
            // Arrange
            var alreadyOrderedError = EmisApiErrorMessages.Prescriptions_AlreadyOrderedLast30Days;
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyOrderedError;

            _emisClient.Setup(x => x.PrescriptionsPost(_emisUserSession.SessionId,
                    _emisUserSession.EndUserSessionId, It.IsAny<PrescriptionRequestsPost>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestPostResponse>(HttpStatusCode.InternalServerError)
                        { ExceptionErrorResponse = errorResponse }));

            // Act
            var result = await _systemUnderTest.OrderPrescription(new GpLinkedAccountModel(_emisUserSession, _patientId), _repeatPrescriptionRequest);

            // Assert
            result.Should().BeAssignableTo<OrderPrescriptionResult.MedicationAlreadyOrderedWithinLast30Days>();
        }

        [TestMethod]
        public async Task Post_ReturnsForbidden_WhenErrorReceivedFromEmis()
        {
            // Arrange
            var notEnabledError = EmisApiErrorMessages.EmisService_NotEnabledForUser;
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = notEnabledError;

            _emisClient.Setup(x => x.PrescriptionsPost(_emisUserSession.SessionId,
                    _emisUserSession.EndUserSessionId, It.IsAny<PrescriptionRequestsPost>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestPostResponse>(HttpStatusCode.Forbidden)
                        { ExceptionErrorResponse = errorResponse }));

            // Act
            var result = await _systemUnderTest.OrderPrescription(new GpLinkedAccountModel(_emisUserSession, _patientId), _repeatPrescriptionRequest);

            // Assert
            result.Should().BeAssignableTo<OrderPrescriptionResult.Forbidden>();
        }

        [TestMethod]
        public async Task Post_ReturnsBadRequest_WhenErrorReceivedFromEmis()
        {
            // Arrange
            var errorResponse = _fixture.Create<BadRequestErrorResponse>();
            errorResponse.Message = "The request is invalid.";
            errorResponse.ModelState = new Dictionary<string, string[]>()
            {
                { "requestModel.MedicationCourseGuids[0]", new[] { "An error has occurred." } }
            };

            _emisClient.Setup(x => x.PrescriptionsPost(_emisUserSession.SessionId,
                    _emisUserSession.EndUserSessionId, It.IsAny<PrescriptionRequestsPost>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestPostResponse>(HttpStatusCode.BadRequest)
                        { ErrorResponseBadRequest = errorResponse }));

            // Act
            var result = await _systemUnderTest.OrderPrescription( new GpLinkedAccountModel(_emisUserSession, _patientId), _repeatPrescriptionRequest);

            // Assert
            result.Should().BeAssignableTo<OrderPrescriptionResult.BadRequest>();
        }

        [TestMethod]
        public async Task Post_ReturnsBadGateway_WhenErrorReceivedFromEmis()
        {
            // Arrange
            const string generalError = "general error";
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = generalError;

            _emisClient.Setup(x => x.PrescriptionsPost(_emisUserSession.SessionId,
                    _emisUserSession.EndUserSessionId, It.IsAny<PrescriptionRequestsPost>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<PrescriptionRequestPostResponse>(HttpStatusCode
                            .InternalServerError)
                        { ExceptionErrorResponse = errorResponse }));

            // Act
            var result = await _systemUnderTest.OrderPrescription( new GpLinkedAccountModel(_emisUserSession, _patientId), _repeatPrescriptionRequest);

            // Assert
            result.Should().BeAssignableTo<OrderPrescriptionResult.BadGateway>();
        }

        #endregion
    }
}
