using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Prescriptions
{
    [TestClass]
    public sealed class CoursesControllerTests : IDisposable
    {
        private CoursesController _systemUnderTest;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private P9UserSession _userSession;
        private Guid _patientId;
        private Mock<IAuditor> _mockAuditor;
        private Mock<ICourseService> _mockCourseService;
        private Mock<IErrorReferenceGenerator> _mockErrorReferenceGenerator;
        private Mock<ILogger<CoursesController>> _mockLogger;
        private string _serviceDeskReference;
        private Mock<IGpSystem> _mockGpSystem;
        private CourseListResponse _courseListResponse;

        private const string RequestAuditType = "RepeatPrescriptions_ViewRepeatMedications_Request";
        private const string ResponseAuditType = "RepeatPrescriptions_ViewRepeatMedications_Response";

        private const string RequestAuditMessage = "Attempting to retrieve courses";
        private const int MockNumberOfCourses = 10;

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();

            _userSession = new P9UserSession("csrfToken", "nhsNumber", new CitizenIdUserSession(), new EmisUserSession(), "im1token");

            _mockCourseService = new Mock<ICourseService>();
            _mockAuditor = new Mock<IAuditor>();
            _mockLogger = new Mock<ILogger<CoursesController>>();

            _courseListResponse = new CourseListResponse();
            var result = new GetCoursesResult.Success(_courseListResponse, new FilteringCounts
            {
                ReceivedCount = MockNumberOfCourses,
                ReceivedRepeatsCount = MockNumberOfCourses,
                ExcessRepeatsCount = MockNumberOfCourses,
                ReturnedCount = MockNumberOfCourses
            });

            _mockCourseService.Setup(x => x.GetCourses(
                    It.Is<GpLinkedAccountModel>(d =>
                        d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId)))
                .Returns(Task.FromResult((GetCoursesResult) result));

            _mockGpSystem = new Mock<IGpSystem>();
            _mockGpSystem
                .Setup(x => x.GetCourseService())
                .Returns(_mockCourseService.Object);

            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);

            _mockErrorReferenceGenerator = new Mock<IErrorReferenceGenerator>();
            _serviceDeskReference = "service desk ref";

            _systemUnderTest = new CoursesController(
                _mockLogger.Object,
                _mockGpSystemFactory.Object,
                _mockAuditor.Object,
                new Mock<ISessionCacheService>().Object,
                _mockErrorReferenceGenerator.Object);
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Act
            var result = await _systemUnderTest.Get(_patientId, _userSession, _userSession.GpUserSession);

            // Assert
            _mockCourseService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(_courseListResponse);

            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage, It.IsAny<object[]>()));
            _mockAuditor.Verify(x =>
                x.Audit(ResponseAuditType, "Courses successfully retrieved. " +
                                           $"Total courses before filtering: {MockNumberOfCourses}, " +
                                           $"Total courses returned after filtering: {MockNumberOfCourses}"));
        }

        [DataTestMethod]
        [DataRow(typeof(GetCoursesResult.Forbidden), StatusCodes.Status403Forbidden,
            "Error retrieving courses: Insufficient permissions")]
        [DataRow(typeof(GetCoursesResult.InternalServerError), StatusCodes.Status500InternalServerError,
            "Error retrieving courses: Internal Server Error")]
        [DataRow(typeof(GetCoursesResult.BadGateway), StatusCodes.Status502BadGateway,
            "Error retrieving courses: Supplier Unavailable")]
        public async Task Get_ServiceReturnsErrorResult_ReturnsAppropriateResultObject(
            Type serviceResultType,
            int expectedStatusCode,
            string expectedAuditResponseMessageFormat)
        {
            // Arrange
            var serviceResult = (GetCoursesResult) Activator.CreateInstance(serviceResultType);
            _mockCourseService.Setup(x => x.GetCourses(
                    It.Is<GpLinkedAccountModel>(d =>
                    d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId)))
                .Returns(Task.FromResult(serviceResult))
                .Verifiable();
            _mockErrorReferenceGenerator.Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Prescriptions,
                    expectedStatusCode, _userSession.GpUserSession.Supplier))
                .Returns(_serviceDeskReference);

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.Get(_patientId, _userSession, _userSession.GpUserSession);

            // Assert
            _mockCourseService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(expectedStatusCode);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, expectedAuditResponseMessageFormat));
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_LogsCoursesCountWithMaximumAllowanceDiscarded()
        {
            // Act
            await _systemUnderTest.Get(_patientId, _userSession, _userSession.GpUserSession);

            // Assert
            var expectedLogMessage =
                $"Courses Count: Courses Received={MockNumberOfCourses} " +
                $"Courses remaining after filtering out non-repeats={MockNumberOfCourses} " +
                $"Courses filtered out for exceeding maximum allowance={MockNumberOfCourses} " +
                $"Courses Returned={MockNumberOfCourses}";
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_LogsCoursesCountWithMaximumRequestableDiscarded()
        {
            // Arrange
            var result = new GetCoursesResult.Success(_courseListResponse, new FilteringCounts
            {
                ReceivedCount = MockNumberOfCourses,
                ReceivedRepeatsCount = MockNumberOfCourses,
                ExcessRepeatsCount = MockNumberOfCourses,
                ReturnedCount = MockNumberOfCourses
            });
            _mockCourseService.Setup(x => x.GetCourses(
                    It.Is<GpLinkedAccountModel>(d =>
                        d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId)))
                .Returns(Task.FromResult((GetCoursesResult) result));

            // Act
            await _systemUnderTest.Get(_patientId, _userSession, _userSession.GpUserSession);

            // Assert
            var expectedLogMessage =
                $"Courses Count: Courses Received={MockNumberOfCourses} " +
                $"Courses remaining after filtering out non-repeats={MockNumberOfCourses} " +
                $"Courses filtered out for exceeding maximum allowance={MockNumberOfCourses} " +
                $"Courses Returned={MockNumberOfCourses}";
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}