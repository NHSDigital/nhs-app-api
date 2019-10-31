using System;
using System.Linq;
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
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Prescriptions
{
    [TestClass]
    public class PrescriptionsControllerPostTests
    {
        private PrescriptionsController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IPrescriptionValidationService> _mockPrescriptionValidationService;
        private Mock<IPrescriptionService> _mockPrescriptionsService;
        private Mock<IAuditor> _mockAuditor;
        private Mock<IErrorReferenceGenerator> _mockErrorReferenceGenerator;
        private ConfigurationSettings _options;
        
        private UserSession _userSession;
        private Guid _patientId;
        private RepeatPrescriptionRequest _repeatPrescriptionRequest;

        private const string CookieDomain = "CookieDomain";
        private int PrescriptionsDefaultLastNumberMonthsToDisplay;   
        private const int DefaultSessionExpiryMinutes  = 10;
        private const int DefaultHttpTimeoutSeconds = 6;
        private const int MinimumAppAge = 16;
        private const int MinimumLinkageAge = 16;

        private readonly DateTimeOffset? CurrentTermsConditionsEffectiveDate = DateTimeOffset.Now;
        private string _serviceDeskReference;

        private const string PostRequestAuditType = "RepeatPrescriptions_OrderRepeatMedications_Request";
        private const string PostResponseAuditType = "RepeatPrescriptions_OrderRepeatMedications_Response";

        private const string RequestAuditMessageFormat =
            "Attempting to create a prescription request with course ids: {0}";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));

            _patientId = Guid.NewGuid();

            _repeatPrescriptionRequest = _fixture.Create<RepeatPrescriptionRequest>();

            _userSession = _fixture.Create<UserSession>();

            _mockPrescriptionsService = _fixture.Freeze<Mock<IPrescriptionService>>();
            _mockPrescriptionValidationService = _fixture.Freeze<Mock<IPrescriptionValidationService>>();
            
            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            PrescriptionsDefaultLastNumberMonthsToDisplay  = _fixture.Create<int>();

            _options = new ConfigurationSettings(CookieDomain, PrescriptionsDefaultLastNumberMonthsToDisplay, DefaultHttpTimeoutSeconds, DefaultSessionExpiryMinutes, 
            MinimumAppAge, MinimumLinkageAge, CurrentTermsConditionsEffectiveDate);

            _fixture.Inject(_options);
            
            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockGpSystem
                .Setup(x => x.GetPrescriptionService())
                .Returns(_mockPrescriptionsService.Object);
            _mockGpSystem
                .Setup(x => x.GetPrescriptionValidationService())
                .Returns(_mockPrescriptionValidationService.Object);
            
            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);
            
            _mockErrorReferenceGenerator = _fixture.Freeze<Mock<IErrorReferenceGenerator>>();
            _serviceDeskReference = _fixture.Create<string>();

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Items[Constants.HttpContextItems.UserSession]).Returns(_userSession);
            
            _systemUnderTest = _fixture.Create<PrescriptionsController>();
            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Post_PrescriptionsServiceReturnsSuccess_ReturnsCreated()
        {
            // Arrange
            _mockPrescriptionsService.Setup(x => x.OrderPrescription(
                It.Is<GpLinkedAccountModel>(
                d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId),
                _repeatPrescriptionRequest))
                .Returns(Task.FromResult((OrderPrescriptionResult)new OrderPrescriptionResult.Success()));
            
            _mockPrescriptionValidationService
                .Setup(x => x.IsPostValid(_repeatPrescriptionRequest))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Post(_repeatPrescriptionRequest, _patientId);

            // Assert
            _mockGpSystem.VerifyAll();
            _mockPrescriptionsService.VerifyAll();
            _mockPrescriptionValidationService.VerifyAll();
            _mockGpSystemFactory.VerifyAll();
            result.Should().BeAssignableTo<CreatedResult>();

            var courseIds = string.Join(",", _repeatPrescriptionRequest.CourseIds);
            _mockAuditor.Verify(x => x.Audit(PostRequestAuditType, RequestAuditMessageFormat, courseIds));
            _mockAuditor.Verify(x => x.Audit(PostResponseAuditType, "Repeat prescription request successfully created with course ids: {0}", courseIds));
        }

        [TestMethod]
        public async Task Post_PrescriptionsServiceReturnsPartialSuccess_ReturnsAccepted()
        {
            // Arrange
            var response = _fixture.Create<PrescriptionRequestPostPartialSuccessResponse>();
            _mockPrescriptionsService.Setup(x => x.OrderPrescription(
                    It.Is<GpLinkedAccountModel>(
                        d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId),
                    _repeatPrescriptionRequest))
                .Returns(Task.FromResult((OrderPrescriptionResult)new OrderPrescriptionResult.PartialSuccess(response)));
            
            _mockPrescriptionValidationService
                .Setup(x => x.IsPostValid(_repeatPrescriptionRequest))
                .Returns(true);
            
            // Act
            var result = await _systemUnderTest.Post(_repeatPrescriptionRequest, _patientId);
            
            // Assert
            _mockGpSystem.VerifyAll();
            _mockPrescriptionsService.VerifyAll();
            _mockPrescriptionValidationService.VerifyAll();
            _mockGpSystemFactory.VerifyAll();
            var acceptedResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            using (new AssertionScope())
            {
                acceptedResult.StatusCode.Should().Be(StatusCodes.Status202Accepted);
                acceptedResult.Value.Should().BeEquivalentTo(response);
            }

            var courseIds = string.Join(",", _repeatPrescriptionRequest.CourseIds);
            var successfulCourseIds = string.Join(",", response.SuccessfulOrders.Select(x => x.CourseId));
            var unsuccessfulCourseIds = string.Join(",", response.UnsuccessfulOrders.Select(x => x.CourseId));
            _mockAuditor.Verify(x => x.Audit(PostRequestAuditType, RequestAuditMessageFormat, courseIds));
            _mockAuditor.Verify(x => x.Audit(PostResponseAuditType, "Partial Success ordering prescription: Attempted to order course ids: {0}, Successful course ids: {1}, Unsuccessful course ids: {2}", courseIds, successfulCourseIds, unsuccessfulCourseIds));
        }
        
        [TestMethod]
        public async Task Post_ModelValidationFails_ReturnsBadRequest()
        {
            // Arrange
            _mockPrescriptionValidationService
                .Setup(x => x.IsPostValid(_repeatPrescriptionRequest))
                .Returns(false);
            _mockErrorReferenceGenerator.Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Prescriptions,
                    StatusCodes.Status400BadRequest, _userSession.GpUserSession.Supplier))
                .Returns(_serviceDeskReference);
            
            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.Post(_repeatPrescriptionRequest, _patientId);

            // Assert
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }
            
            var courseIds = string.Join(",", _repeatPrescriptionRequest.CourseIds);
            _mockAuditor.Verify(x => x.Audit(PostRequestAuditType, RequestAuditMessageFormat, courseIds));
            _mockAuditor.Verify(x => x.Audit(PostResponseAuditType, "Error creating prescription request: Bad Request with course ids: {0}", courseIds));
        }

        [DataTestMethod]
        [DataRow(typeof(OrderPrescriptionResult.BadRequest), StatusCodes.Status400BadRequest,
            "Error creating prescription request: Bad Request with course ids: {0}")]
        [DataRow(typeof(OrderPrescriptionResult.Forbidden), StatusCodes.Status403Forbidden,
            "Error creating prescription request: Insufficient permissions with course ids: {0}")]
        [DataRow(typeof(OrderPrescriptionResult.CannotReorderPrescription), StatusCodes.Status409Conflict,
            "Error creating prescription request: Cannot Reorder Prescription with course ids: {0}")]
        [DataRow(typeof(OrderPrescriptionResult.MedicationAlreadyOrderedWithinLast30Days), Constants.CustomHttpStatusCodes.Status466MedicationAlreadyOrderedWithinLast30Days,
            "Error ordering prescription: Medication already ordered within last 30 days with course ids: {0}")]
        [DataRow(typeof(OrderPrescriptionResult.InternalServerError), StatusCodes.Status500InternalServerError,
            "Error creating prescription request: Internal Server Error with course ids: {0}")]
        [DataRow(typeof(OrderPrescriptionResult.BadGateway), StatusCodes.Status502BadGateway,
            "Error creating prescription request: Supplier Unavailable with course ids: {0}")]
        public async Task Post_ServiceReturnsErrorResult_ReturnsAppropriateResultObject(
            Type serviceResultType,
            int expectedStatusCode,
            string expectedAuditResponseMessageFormat)
        {
            // Arrange
            var serviceResult = (OrderPrescriptionResult) Activator.CreateInstance(serviceResultType);
            _mockPrescriptionsService.Setup(x => x.OrderPrescription(
                    It.Is<GpLinkedAccountModel>(
                        d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId),
                    _repeatPrescriptionRequest))
                .Returns(Task.FromResult(serviceResult));
            _mockPrescriptionValidationService
                .Setup(x => x.IsPostValid(_repeatPrescriptionRequest))
                .Returns(true);
            _mockErrorReferenceGenerator.Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Prescriptions,
                    expectedStatusCode, _userSession.GpUserSession.Supplier))
                .Returns(_serviceDeskReference);
            
            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.Post(_repeatPrescriptionRequest, _patientId);

            // Assert
            _mockPrescriptionsService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(expectedStatusCode);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            var courseIds = string.Join(",", _repeatPrescriptionRequest.CourseIds);
            _mockAuditor.Verify(x => x.Audit(PostRequestAuditType, RequestAuditMessageFormat, courseIds));
            _mockAuditor.Verify(x => x.Audit(PostResponseAuditType, expectedAuditResponseMessageFormat, courseIds));
        }
    }
}