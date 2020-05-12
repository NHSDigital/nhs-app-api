using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Prescriptions
{
    [TestClass]
    public class EmisCourseServiceTests
    {
        private EmisCourseService _systemUnderTest;
        private Mock<IEmisClient> _emisClient;
        private Mock<IEmisPrescriptionMapper> _emisPrescriptionMapper;
        private EmisConfigurationSettings _settings;
        private EmisUserSession _emisUserSession;
        private IFixture _fixture;
        private Guid _patientId;
        private Mock<ILogger<EmisCourseService>> _mockLogger;
        private const string DefaultEmisVersion = "2.1.0.0";
        private static readonly string DefaultEmisApplicationId = Guid.NewGuid().ToString();
        private static readonly Uri BaseUri = new Uri("http://emis_base_url/");
        private const string CertificatePath = "CertificatePath";
        private const string CertificatePassphrase = "CerticiatePassphrase";
        private const int CoursesMaxCoursesLimit = 100;
        private const int EmisExtendedHttpTimeoutSeconds = 6;
        private const int DefaultHttpTimeoutSeconds = 2;
        private const int PrescriptionsMaxCoursesSoftLimit = 100;
        private const string Environment = "environment";
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _emisUserSession = _fixture.Build<EmisUserSession>()
                .With(x => x.Id, _patientId)
                .Create();

            _mockLogger = _fixture.Freeze<Mock<ILogger<EmisCourseService>>>();

            _emisClient = _fixture.Freeze<Mock<IEmisClient>>();

            _emisPrescriptionMapper = _fixture.Freeze<Mock<IEmisPrescriptionMapper>>();
            _settings = new EmisConfigurationSettings(BaseUri, DefaultEmisApplicationId, DefaultEmisVersion, CertificatePath,
                CertificatePassphrase, EmisExtendedHttpTimeoutSeconds, DefaultHttpTimeoutSeconds, CoursesMaxCoursesLimit, PrescriptionsMaxCoursesSoftLimit,
                Environment);
            _fixture.Inject(_settings);
            _systemUnderTest = _fixture.Create<EmisCourseService>();
            _sampleSuccessStatusCodes = new List<HttpStatusCode>()
            {
                HttpStatusCode.OK
            };
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessResponseForHappyPath_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var coursesResponse = _fixture.Create<CoursesGetResponse>();

            _emisClient.Setup(x => x.CoursesGet(It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.CoursesGet, _sampleSuccessStatusCodes)
                    {
                        Body = coursesResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            // Act
            var result = await _systemUnderTest.GetCourses( new GpLinkedAccountModel(_emisUserSession, _patientId));

            // Assert
            _emisClient.Verify(x => x.CoursesGet((It.Is<EmisRequestParameters>(
                    e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                         e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)))));

            result.Should().BeAssignableTo<GetCoursesResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task
            Get_CoursesInResponseAreFilteredSoOnlyRepeatCoursesWhichCanBeRequestedAreReturned_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var expectedMedicationCourseGuid = Guid.NewGuid().ToString();
            var coursesResponse = new CoursesGetResponse
            {
                Courses = new List<MedicationCourse>
                {
                    new MedicationCourse
                    {
                        CanBeRequested = false,
                        PrescriptionType = PrescriptionType.Repeat,
                        MedicationCourseGuid = Guid.NewGuid().ToString(),
                    },
                    new MedicationCourse
                    {
                        CanBeRequested = true,
                        PrescriptionType = PrescriptionType.Repeat,
                        MedicationCourseGuid = expectedMedicationCourseGuid,
                    },
                    new MedicationCourse
                    {
                        CanBeRequested = false,
                        PrescriptionType = PrescriptionType.Repeat,
                        MedicationCourseGuid = Guid.NewGuid().ToString(),
                    },
                    new MedicationCourse
                    {
                        CanBeRequested = true,
                        PrescriptionType = PrescriptionType.Unknown,
                        MedicationCourseGuid = Guid.NewGuid().ToString(),
                    },
                }
            };

            _emisClient.Setup(x => x.CoursesGet(
                    It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.CoursesGet, _sampleSuccessStatusCodes)
                    {
                        Body = coursesResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            var response = new CourseListResponse();
            CoursesGetResponse capturedItemToMap = null;
            _emisPrescriptionMapper.Setup(x => x.Map(It.IsAny<CoursesGetResponse>())).Returns(response)
                .Callback<CoursesGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_emisUserSession, _patientId));

            // Assert
            _emisClient.Verify(x => x.CoursesGet((It.Is<EmisRequestParameters>(
                e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                     e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                     e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)))));
            result.Should().BeAssignableTo<GetCoursesResult.Success>()
                .Subject.Response.Should().NotBeNull();

            var getCoursesResult = (GetCoursesResult.Success) result;
            getCoursesResult.Response.Should().Be(response);

            capturedItemToMap.Courses.Should().HaveCount(1);
            capturedItemToMap.Courses.ElementAt(0).MedicationCourseGuid.Should().Be(expectedMedicationCourseGuid);
        }

        [DataTestMethod]
        [DataRow(CoursesMaxCoursesLimit + 1, CoursesMaxCoursesLimit)]
        [DataRow(CoursesMaxCoursesLimit, CoursesMaxCoursesLimit)]
        [DataRow(CoursesMaxCoursesLimit - 1, CoursesMaxCoursesLimit - 1)]
        public async Task Get_CoursesInResponseAreLimitedToMax_WhenSuccessfulResponseFromEmis(
            int numberOfCoursesToCreate, int expectedNumberOfCourses)
        {
            // Arrange
            var medicationCourses = new List<MedicationCourse>();

            for (var i = 0; i < numberOfCoursesToCreate; i++)
            {
                medicationCourses.Add(
                    new MedicationCourse
                    {
                        CanBeRequested = true,
                        PrescriptionType = PrescriptionType.Repeat,
                        MedicationCourseGuid = Guid.NewGuid().ToString(),
                    });
            }

            var coursesResponse = new CoursesGetResponse
            {
                Courses = medicationCourses,
            };

            _emisClient.Setup(x => x.CoursesGet(
                    It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.CoursesGet, _sampleSuccessStatusCodes)
                    {
                        Body = coursesResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            var response = new CourseListResponse();
            CoursesGetResponse capturedItemToMap = null;
            _emisPrescriptionMapper.Setup(x => x.Map(It.IsAny<CoursesGetResponse>())).Returns(response)
                .Callback<CoursesGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_emisUserSession, _patientId));

            // Assert
            _emisClient.Verify(x => x.CoursesGet((It.Is<EmisRequestParameters>(
                e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                     e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                     e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)))));
            result.Should().BeAssignableTo<GetCoursesResult.Success>()
                .Subject.Response.Should().NotBeNull();

            var getCoursesResult = (GetCoursesResult.Success) result;
            getCoursesResult.Response.Should().Be(response);
            capturedItemToMap.Courses.Should().HaveCount(expectedNumberOfCourses);
        }

        [TestMethod]
        public async Task Get_CoursesInResponseAreOrderedByName_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var coursesResponse = new CoursesGetResponse
            {
                Courses = new List<MedicationCourse>
                {
                    new MedicationCourse
                    {
                        CanBeRequested = true,
                        PrescriptionType = PrescriptionType.Repeat,
                        MedicationCourseGuid = Guid.NewGuid().ToString(),
                        Name = "b"
                    },
                    new MedicationCourse
                    {
                        CanBeRequested = true,
                        PrescriptionType = PrescriptionType.Repeat,
                        MedicationCourseGuid = Guid.NewGuid().ToString(),
                        Name = "d"
                    },
                    new MedicationCourse
                    {
                        CanBeRequested = true,
                        PrescriptionType = PrescriptionType.Repeat,
                        MedicationCourseGuid = Guid.NewGuid().ToString(),
                        Name = "c"
                    },
                    new MedicationCourse
                    {
                        CanBeRequested = true,
                        PrescriptionType = PrescriptionType.Repeat,
                        MedicationCourseGuid = Guid.NewGuid().ToString(),
                        Name = "a"
                    },
                }
            };

            _emisClient.Setup(x => x.CoursesGet(
                    It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.CoursesGet, _sampleSuccessStatusCodes)
                    {
                        Body = coursesResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            var response = new CourseListResponse();
            CoursesGetResponse capturedItemToMap = null;
            _emisPrescriptionMapper.Setup(x => x.Map(It.IsAny<CoursesGetResponse>())).Returns(response)
                .Callback<CoursesGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_emisUserSession, _patientId));

            // Assert
            _emisClient.Verify(x => x.CoursesGet(
                It.Is<EmisRequestParameters>(
                    e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                         e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal))));
            result.Should().BeAssignableTo<GetCoursesResult.Success>();
            ((GetCoursesResult.Success) result).Response.Should().NotBeNull();

            var getCoursesResult = (GetCoursesResult.Success) result;
            getCoursesResult.Response.Should().Be(response);

            capturedItemToMap.Courses.Should().HaveCount(4);
            capturedItemToMap.Courses.ElementAt(0).Name.Should().Be("a");
            capturedItemToMap.Courses.ElementAt(1).Name.Should().Be("b");
            capturedItemToMap.Courses.ElementAt(2).Name.Should().Be("c");
            capturedItemToMap.Courses.ElementAt(3).Name.Should().Be("d");
        }

        [TestMethod]
        public async Task Get_CoursesSuccessfullyLogsOutWhetherCertainDatesArePopulated()
        {
            // Arrange
            var coursesResponse = new CoursesGetResponse
            {
                Courses = new List<MedicationCourse>
                {
                    new MedicationCourse
                    {
                        MostRecentIssueDate = new DateTimeOffset(new DateTime(2000, 1, 2)),
                        ReviewDate = new DateTimeOffset(new DateTime(2022, 1, 20)),
                        NextIssueDate = new DateTimeOffset(new DateTime(2021, 5, 2)),
                        CanBeRequested = true,
                        PrescriptionType = PrescriptionType.Repeat,
                        MedicationCourseGuid = Guid.NewGuid().ToString(),
                        Name = "Course 1"
                    },
                    new MedicationCourse
                    {
                        MostRecentIssueDate = new DateTimeOffset(new DateTime(2000, 1, 2)),
                        ReviewDate = null,
                        NextIssueDate = new DateTimeOffset(new DateTime(2021, 5, 2)),
                        CanBeRequested = true,
                        PrescriptionType = PrescriptionType.Repeat,
                        MedicationCourseGuid = Guid.NewGuid().ToString(),
                        Name = "Course 2"
                    },
                    new MedicationCourse
                    {
                        MostRecentIssueDate = null,
                        ReviewDate = new DateTimeOffset(new DateTime(2022, 1, 20)),
                        NextIssueDate = new DateTimeOffset(new DateTime(2021, 5, 2)),
                        CanBeRequested = true,
                        PrescriptionType = PrescriptionType.Repeat,
                        MedicationCourseGuid = Guid.NewGuid().ToString(),
                        Name = "Course 3"
                    },
                    new MedicationCourse
                    {
                        MostRecentIssueDate = new DateTimeOffset(new DateTime(2000, 1, 2)),
                        ReviewDate = new DateTimeOffset(new DateTime(2022, 1, 20)),
                        NextIssueDate = null,
                        CanBeRequested = true,
                        PrescriptionType = PrescriptionType.Repeat,
                        MedicationCourseGuid = Guid.NewGuid().ToString(),
                        Name = "Course 4"
                    },
                }
            };

            _emisClient.Setup(x => x.CoursesGet(
                    It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.CoursesGet, _sampleSuccessStatusCodes)
                    {
                        Body = coursesResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            var response = new CourseListResponse();
            CoursesGetResponse capturedItemToMap = null;
            _emisPrescriptionMapper.Setup(x => x.Map(It.IsAny<CoursesGetResponse>())).Returns(response)
                .Callback<CoursesGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_emisUserSession, _patientId));

            // Assert

            var getCoursesResult = (GetCoursesResult.Success) result;
            getCoursesResult.Response.Should().Be(response);

            capturedItemToMap.Courses.Should().HaveCount(4);

            var expectedLogMessage =
                $"Prescription date data logging: MostRecentIssueDate populated=\"3 / 4\" " +
                $"NextIssueDate populated=\"3 / 4\" " +
                $"ReviewDate populated=\"3 / 4\"";

            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }

        [TestMethod]
        public async Task Get_Returns502SystemUnavailable_WhenErrorReceivedFromEmis()
        {
            // Arrange
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;

            _emisClient.Setup(x => x.CoursesGet(
                    It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal))
                    ))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.InternalServerError, RequestsForSuccessOutcome.CoursesGet, _sampleSuccessStatusCodes)
                        { ExceptionErrorResponse = errorResponse }));

            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_emisUserSession, _patientId));

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.BadGateway>();
        }

        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;

            _emisClient.Setup(x => x.CoursesGet(
                    It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal))))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_emisUserSession, _patientId));

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.BadGateway>();
            _emisClient.Verify();
        }

        [TestMethod]
        public async Task Get_ReturnsInternalServerError_WhenNullExceptionOccursCallingEmis()
        {
            // Arrange
            _emisClient.Setup(x => x.CoursesGet(
                    It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.CoursesGet, _sampleSuccessStatusCodes)
                    {
                        Body = null,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_emisUserSession, _patientId));

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.InternalServerError>();
            _emisClient.Verify();
        }

        [TestMethod]
        public async Task Get_ReturnsForbidden_WhenErrorReceivedFromEmis()
        {
            // Arrange
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = EmisApiErrorMessages.EmisService_NotEnabledForUser;

            _emisClient.Setup(x => x.CoursesGet(
                    It.Is<EmisRequestParameters>(
                        e => e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                             e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                             e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.InternalServerError, RequestsForSuccessOutcome.CoursesGet, _sampleSuccessStatusCodes)
                        { ExceptionErrorResponse = errorResponse }));

            // Act
            var result = await _systemUnderTest.GetCourses(new GpLinkedAccountModel(_emisUserSession, _patientId));

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.Forbidden>();
        }
    }
}