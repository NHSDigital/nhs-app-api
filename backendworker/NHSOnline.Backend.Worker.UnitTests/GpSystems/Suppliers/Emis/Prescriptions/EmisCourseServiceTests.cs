using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Prescriptions;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Prescriptions
{
    [TestClass]
    public class EmisCourseServiceTests
    {
        private EmisCourseService _systemUnderTest;
        private Mock<IEmisClient> _emisClient;
        private Mock<IEmisPrescriptionMapper> _emisPrescriptionMapper;
        private IOptions<ConfigurationSettings> _options;
        private EmisUserSession _userSession;
        private IFixture _fixture;

        private const int CoursesMaxCoursesLimit = 100;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _emisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _userSession = _fixture.Freeze<EmisUserSession>();
            _emisPrescriptionMapper = _fixture.Freeze<Mock<IEmisPrescriptionMapper>>();
            _options = Options.Create(new ConfigurationSettings
            {
                CoursesMaxCoursesLimit = CoursesMaxCoursesLimit
            });
            _fixture.Inject(_options);
            _systemUnderTest = _fixture.Create<EmisCourseService>();
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var coursesResponse = _fixture.Create<CoursesGetResponse>();

            _emisClient.Setup(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId,
                    _userSession.EndUserSessionId))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK)
                    {
                        Body = coursesResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            _emisClient.Verify(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId,
                _userSession.EndUserSessionId));
            result.Should().BeAssignableTo<GetCoursesResult.SuccessfullyRetrieved>();
            ((GetCoursesResult.SuccessfullyRetrieved) result).Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task
            Get_CoursesInResponseAreFilteredSoOnlyRepeatCoursesWhichCanBeRequestedAreReturned_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            string expectedMedicationCourseGuid = Guid.NewGuid().ToString();
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

            _emisClient.Setup(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId,
                    _userSession.EndUserSessionId))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK)
                    {
                        Body = coursesResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            var response = new CourseListResponse();
            CoursesGetResponse capturedItemToMap = null;
            _emisPrescriptionMapper.Setup(x => x.Map(It.IsAny<CoursesGetResponse>())).Returns(response)
                .Callback<CoursesGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            _emisClient.Verify(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId,
                _userSession.EndUserSessionId));
            result.Should().BeAssignableTo<GetCoursesResult.SuccessfullyRetrieved>();
            ((GetCoursesResult.SuccessfullyRetrieved) result).Response.Should().NotBeNull();

            var getCoursesResult = (GetCoursesResult.SuccessfullyRetrieved) result;
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

            for (int i = 0; i < numberOfCoursesToCreate; i++)
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

            _emisClient.Setup(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId,
                    _userSession.EndUserSessionId))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK)
                    {
                        Body = coursesResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            var response = new CourseListResponse();
            CoursesGetResponse capturedItemToMap = null;
            _emisPrescriptionMapper.Setup(x => x.Map(It.IsAny<CoursesGetResponse>())).Returns(response)
                .Callback<CoursesGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            _emisClient.Verify(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId,
                _userSession.EndUserSessionId));
            result.Should().BeAssignableTo<GetCoursesResult.SuccessfullyRetrieved>();
            ((GetCoursesResult.SuccessfullyRetrieved) result).Response.Should().NotBeNull();

            var getCoursesResult = (GetCoursesResult.SuccessfullyRetrieved) result;
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

            _emisClient.Setup(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId,
                    _userSession.EndUserSessionId))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK)
                    {
                        Body = coursesResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            var response = new CourseListResponse();
            CoursesGetResponse capturedItemToMap = null;
            _emisPrescriptionMapper.Setup(x => x.Map(It.IsAny<CoursesGetResponse>())).Returns(response)
                .Callback<CoursesGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            _emisClient.Verify(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId,
                _userSession.EndUserSessionId));
            result.Should().BeAssignableTo<GetCoursesResult.SuccessfullyRetrieved>();
            ((GetCoursesResult.SuccessfullyRetrieved) result).Response.Should().NotBeNull();

            var getCoursesResult = (GetCoursesResult.SuccessfullyRetrieved) result;
            getCoursesResult.Response.Should().Be(response);

            capturedItemToMap.Courses.Should().HaveCount(4);
            capturedItemToMap.Courses.ElementAt(0).Name.Should().Be("a");
            capturedItemToMap.Courses.ElementAt(1).Name.Should().Be("b");
            capturedItemToMap.Courses.ElementAt(2).Name.Should().Be("c");
            capturedItemToMap.Courses.ElementAt(3).Name.Should().Be("d");
        }

        [TestMethod]
        public async Task Get_Returns502SystemUnavailable_WhenErrorReceivedFromEmis()
        {
            // Arrange
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;

            _emisClient.Setup(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId,
                    _userSession.EndUserSessionId))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.InternalServerError)
                        { ErrorResponse = errorResponse }));

            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            const string alreadyLinkedErrorMessage = "Error occurred";
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;

            _emisClient.Setup(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId,
                    _userSession.EndUserSessionId))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.SupplierSystemUnavailable>();
            _emisClient.Verify();
        }

        [TestMethod]
        public async Task Get_ReturnsInternalServerError_WhenNullExceptionOccursCallingEmis()
        {
            // Arrange
            _emisClient.Setup(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId,
                    _userSession.EndUserSessionId))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK)
                    {
                        Body = null,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.InternalServerError>();
            _emisClient.Verify();
        }

        [TestMethod]
        public async Task Get_ReturnsForbidden_WhenErrorReceivedFromEmis()
        {
            // Arrange
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = EmisApiErrorMessages.EmisService_NotEnabledForUser;

            _emisClient.Setup(x => x.CoursesGet(_userSession.UserPatientLinkToken, _userSession.SessionId,
                    _userSession.EndUserSessionId))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<CoursesGetResponse>(HttpStatusCode.InternalServerError)
                        { ErrorResponse = errorResponse }));

            // Act
            var result = await _systemUnderTest.GetCourses(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.SupplierNotEnabled>();
        }
    }
}