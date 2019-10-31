using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Prescriptions
{
    [TestClass]
    public class CoursesControllerTests
    {
        private CoursesController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private UserSession _userSession;
        private Guid _patientId;
        private Mock<IAuditor> _mockAuditor;
        private Mock<ICourseService> _mockCourseService;
        private Mock<IErrorReferenceGenerator> _mockErrorReferenceGenerator;
        private string _serviceDeskReference;
        private Mock<IGpSystem> _mockGpSystem;
        private CourseListResponse _courseListResponse;

        private const string RequestAuditType = "RepeatPrescriptions_ViewRepeatMedications_Request";
        private const string ResponseAuditType = "RepeatPrescriptions_ViewRepeatMedications_Response";

        private const string RequestAuditMessage = "Attempting to retrieve courses";

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));

            _userSession = _fixture.Create<UserSession>();

            _mockCourseService = _fixture.Freeze<Mock<ICourseService>>();
            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            _courseListResponse = _fixture.Create<CourseListResponse>();
            var result = new GetCoursesResult.Success(_courseListResponse);

            _mockCourseService.Setup(x => x.GetCourses(
                    It.Is<GpLinkedAccountModel>(d =>
                        d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId)))
                .Returns(Task.FromResult((GetCoursesResult) result));

            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockGpSystem
                .Setup(x => x.GetCourseService())
                .Returns(_mockCourseService.Object);
            
            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);

            _mockErrorReferenceGenerator = _fixture.Freeze<Mock<IErrorReferenceGenerator>>();
            _serviceDeskReference = _fixture.Create<string>();

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Items[Constants.HttpContextItems.UserSession]).Returns(_userSession);

            _systemUnderTest = _fixture.Create<CoursesController>();
            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfully()
        {
            // Act
            var result = await _systemUnderTest.Get(_patientId);

            // Assert
            _mockCourseService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(_courseListResponse);

            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage, It.IsAny<object[]>()));
            _mockAuditor.Verify(x =>
                x.Audit(ResponseAuditType, "Courses successfully retrieved - {0} courses", It.IsAny<object[]>()));
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
            var result = await _systemUnderTest.Get(_patientId);

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
    }
}