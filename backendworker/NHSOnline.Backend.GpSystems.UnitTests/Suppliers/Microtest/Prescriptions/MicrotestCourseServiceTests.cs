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
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Prescriptions;
using NHSOnline.Backend.Support;
using Course = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions.Course;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Prescriptions
{
    [TestClass]
    public class MicrotestCourseServiceTests
    {
        private MicrotestCourseService _systemUnderTest;
        private Mock<IMicrotestClient> _microtestClient;
        private Mock<IMicrotestPrescriptionMapper> _microtestPrescriptionMapper;
        private MicrotestConfigurationSettings _settings;
        private MicrotestUserSession _microtestUserSession;
        private IFixture _fixture;
        private ILogger<MicrotestCourseService> _logger;

        private const int CoursesMaxCoursesLimit = 100;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _microtestUserSession = _fixture.Create<MicrotestUserSession>();
            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _microtestUserSession));

            _logger = Mock.Of<ILogger<MicrotestCourseService>>();

            _microtestClient = _fixture.Freeze<Mock<IMicrotestClient>>();
            _microtestPrescriptionMapper = _fixture.Freeze<Mock<IMicrotestPrescriptionMapper>>();
            _settings = new MicrotestConfigurationSettings(null, string.Empty, string.Empty, string.Empty, 0, CoursesMaxCoursesLimit);

            _fixture.Inject(_settings);
            _fixture.Inject(_logger);
            _systemUnderTest = _fixture.Create<MicrotestCourseService>();
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromMicrotest()
        {
            // Arrange
            var coursesResponse = _fixture.Create<CoursesGetResponse>();

            _microtestClient.Setup(x => x.CoursesGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber))
                .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK)
                    {
                        Body = coursesResponse,
                    }));

            // Act
            var result = await _systemUnderTest.GetCourses(_microtestUserSession);

            // Assert
            _microtestClient.Verify(x => x.CoursesGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber));
            result.Should().BeAssignableTo<GetCoursesResult.Success>();
            ((GetCoursesResult.Success)result).Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task
            Get_CoursesInResponseAreFilteredSoOnlyRepeatCoursesWhichCanBeRequestedAreReturned_WhenSuccessfulResponseFromMicrotest()
        {
            // Arrange
            string expectedMedicationCourseGuid = Guid.NewGuid().ToString();
            var coursesResponse = new CoursesGetResponse
            {
                Courses = new List<Course>
                {
                    new Course
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = _fixture.Create<string>(),
                        Dosage = _fixture.Create<string>(),
                        Quantity = "1 tablet",
                        Status = "Other",
                    },
                    new Course
                    {
                        Id = expectedMedicationCourseGuid,
                        Name = _fixture.Create<string>(),
                        Dosage = _fixture.Create<string>(),
                        Quantity = "1 tablet",
                        Status = MedicationStatus.Repeat,
                    },
                    new Course
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = _fixture.Create<string>(),
                        Dosage = _fixture.Create<string>(),
                        Quantity = "1 tablet",
                        Status = "Other",
                    },
                    new Course
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = _fixture.Create<string>(),
                        Dosage = _fixture.Create<string>(),
                        Quantity = "1 tablet",
                        Status = "Other",
                    },
                }
            };

            _microtestClient.Setup(x => x.CoursesGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber))
                .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK)
                    {
                        Body = coursesResponse,
                    }));

            var response = new CourseListResponse();
            CoursesGetResponse capturedItemToMap = null;
            _microtestPrescriptionMapper.Setup(x => x.Map(It.IsAny<CoursesGetResponse>())).Returns(response)
                .Callback<CoursesGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetCourses(_microtestUserSession);

            // Assert
            _microtestClient.Verify(x => x.CoursesGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber));
            result.Should().BeAssignableTo<GetCoursesResult.Success>();
            ((GetCoursesResult.Success)result).Response.Should().NotBeNull();

            var getCoursesResult = (GetCoursesResult.Success)result;
            getCoursesResult.Response.Should().Be(response);

            capturedItemToMap.Courses.Should().HaveCount(1);
            capturedItemToMap.Courses.ElementAt(0).Id.Should().Be(expectedMedicationCourseGuid);
        }

        [DataTestMethod]
        [DataRow(CoursesMaxCoursesLimit + 1, CoursesMaxCoursesLimit)]
        [DataRow(CoursesMaxCoursesLimit, CoursesMaxCoursesLimit)]
        [DataRow(CoursesMaxCoursesLimit - 1, CoursesMaxCoursesLimit - 1)]
        public async Task Get_CoursesInResponseAreLimitedToMax_WhenSuccessfulResponseFromMicrotest(
            int numberOfCoursesToCreate, int expectedNumberOfCourses)
        {
            // Arrange
            var courses = new List<Course>();

            for (int i = 0; i < numberOfCoursesToCreate; i++)
            {
                courses.Add(
                    new Course
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = _fixture.Create<string>(),
                        Dosage = _fixture.Create<string>(),
                        Quantity = "1 tablet",
                        Status = PrescriptionType.Repeat,
                    });
            }

            var coursesResponse = new CoursesGetResponse
            {
                Courses = courses,
            };

            _microtestClient.Setup(x => x.CoursesGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber))
                .Returns(
                    Task.FromResult(
                        new MicrotestClient.MicrotestApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK)
                        {
                            Body = coursesResponse
                        }));

            var response = new CourseListResponse();
            CoursesGetResponse capturedItemToMap = null;
            _microtestPrescriptionMapper.Setup(x => x.Map(It.IsAny<CoursesGetResponse>())).Returns(response)
                .Callback<CoursesGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetCourses(_microtestUserSession);

            // Assert
            _microtestClient.Verify(x => x.CoursesGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber));
            result.Should().BeAssignableTo<GetCoursesResult.Success>();
            ((GetCoursesResult.Success)result).Response.Should().NotBeNull();

            var getCoursesResult = (GetCoursesResult.Success)result;
            getCoursesResult.Response.Should().Be(response);
            capturedItemToMap.Courses.Should().HaveCount(expectedNumberOfCourses);
        }

        [TestMethod]
        public async Task Get_CoursesInResponseAreOrderedByName_WhenSuccessfulResponseFromMicrotest()
        {
            // Arrange
            var coursesResponse = new CoursesGetResponse
            {
                Courses = new List<Course>
                {
                    new Course
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "a",
                        Dosage = _fixture.Create<string>(),
                        Quantity = "1 tablet",
                        Status = PrescriptionType.Repeat,
                    },
                    new Course
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "c",
                        Dosage = _fixture.Create<string>(),
                        Quantity = "1 tablet",
                        Status = PrescriptionType.Repeat,
                    },
                    new Course
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "b",
                        Dosage = _fixture.Create<string>(),
                        Quantity = "1 tablet",
                        Status = PrescriptionType.Repeat,
                    },
                    new Course
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "d",
                        Dosage = _fixture.Create<string>(),
                        Quantity = "1 tablet",
                        Status = PrescriptionType.Repeat,
                    },
                }
            };

            _microtestClient.Setup(x => x.CoursesGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber))
                .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK)
                    {
                        Body = coursesResponse,
                    }));

            var response = new CourseListResponse();
            CoursesGetResponse capturedItemToMap = null;
            _microtestPrescriptionMapper.Setup(x => x.Map(It.IsAny<CoursesGetResponse>())).Returns(response)
                .Callback<CoursesGetResponse>((x) => { capturedItemToMap = x; });

            // Act
            var result = await _systemUnderTest.GetCourses(_microtestUserSession);

            // Assert
            _microtestClient.Verify(x => x.CoursesGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber));
            result.Should().BeAssignableTo<GetCoursesResult.Success>();
            ((GetCoursesResult.Success)result).Response.Should().NotBeNull();

            var getCoursesResult = (GetCoursesResult.Success)result;
            getCoursesResult.Response.Should().Be(response);

            capturedItemToMap.Courses.Should().HaveCount(4);
            capturedItemToMap.Courses.ElementAt(0).Name.Should().Be("a");
            capturedItemToMap.Courses.ElementAt(1).Name.Should().Be("b");
            capturedItemToMap.Courses.ElementAt(2).Name.Should().Be("c");
            capturedItemToMap.Courses.ElementAt(3).Name.Should().Be("d");
        }

        [TestMethod]
        public async Task Get_Returns502SystemUnavailable_WhenErrorReceivedFromMicrotest()
        {
            // Arrange
            _microtestClient.Setup(x => x.CoursesGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber))
                .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<CoursesGetResponse>(HttpStatusCode
                        .InternalServerError)));

            // Act
            var result = await _systemUnderTest.GetCourses(_microtestUserSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.BadGateway>();
        }

        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenHttpExceptionOccursCallingMicrotest()
        {
            // Arrange
            _microtestClient.Setup(x => x.CoursesGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetCourses(_microtestUserSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.BadGateway>();
            _microtestClient.Verify();
        }

        [TestMethod]
        public async Task Get_ReturnsInternalServerError_WhenNullExceptionOccursCallingMicrotest()
        {
            // Arrange
            _microtestClient.Setup(x => x.CoursesGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber))
                .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<CoursesGetResponse>(HttpStatusCode.OK)
                    {
                        Body = null,
                    }));

            // Act
            var result = await _systemUnderTest.GetCourses(_microtestUserSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.InternalServerError>();
            _microtestClient.Verify();
        }
        
        [TestMethod]
        public async Task Get_Returns403ForbiddenResponse_WhenForbiddenExceptionOccursCallingMicrotest()
        {
            // Arrange
            _microtestClient.Setup(x => x.CoursesGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber))
                .Returns(Task.FromResult(
                    new MicrotestClient.MicrotestApiObjectResponse<CoursesGetResponse>(HttpStatusCode
                        .Forbidden)));

            // Act
            var result = await _systemUnderTest.GetCourses(_microtestUserSession);

            // Assert
            result.Should().BeAssignableTo<GetCoursesResult.Forbidden>();
            _microtestClient.Verify();
        }
    }
}
