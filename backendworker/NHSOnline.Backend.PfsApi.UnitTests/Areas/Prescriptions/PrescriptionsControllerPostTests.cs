using System;
using System.Collections.Generic;
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
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Prescriptions
{
    [TestClass]
    public sealed class PrescriptionsControllerPostTests: IDisposable
    {
        private PrescriptionsController _systemUnderTest;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IPrescriptionValidationService> _mockPrescriptionValidationService;
        private Mock<IPrescriptionService> _mockPrescriptionsService;
        private Mock<IAuditor> _mockAuditor;
        private Mock<IErrorReferenceGenerator> _mockErrorReferenceGenerator;
        private ConfigurationSettings _options;

        private P9UserSession _userSession;
        private Guid _patientId;
        private RepeatPrescriptionRequest _repeatPrescriptionRequest;

        private const string CookieDomain = "CookieDomain";
        private const int PrescriptionsDefaultLastNumberMonthsToDisplay = 4;
        private const int DefaultSessionExpiryMinutes  = 10;
        private const int DefaultHttpTimeoutSeconds = 6;
        private const int MinimumAppAge = 16;
        private const int MinimumLinkageAge = 16;

        private string _serviceDeskReference;

        private const string PostRequestAuditType = "RepeatPrescriptions_OrderRepeatMedications_Request";
        private const string PostResponseAuditType = "RepeatPrescriptions_OrderRepeatMedications_Response";

        private const string RequestAuditMessageFormat =
            "Attempting to create a prescription request with course ids: {0}";

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();

            _repeatPrescriptionRequest = new RepeatPrescriptionRequest();

            _userSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), new EmisUserSession(), "im1token");

            _mockPrescriptionsService = new Mock<IPrescriptionService>();
            _mockPrescriptionValidationService = new Mock<IPrescriptionValidationService>();

            _mockAuditor = new Mock<IAuditor>();

            _options = new ConfigurationSettings(CookieDomain, PrescriptionsDefaultLastNumberMonthsToDisplay, DefaultHttpTimeoutSeconds, DefaultSessionExpiryMinutes,
                MinimumAppAge, MinimumLinkageAge);

            _mockGpSystem = new Mock<IGpSystem>();
            _mockGpSystem
                .Setup(x => x.GetPrescriptionService())
                .Returns(_mockPrescriptionsService.Object);
            _mockGpSystem
                .Setup(x => x.GetPrescriptionValidationService())
                .Returns(_mockPrescriptionValidationService.Object);

            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);

            _mockErrorReferenceGenerator = new Mock<IErrorReferenceGenerator>();
            _serviceDeskReference = "service desk ref";

            _systemUnderTest = new PrescriptionsController(
                _options,
                new Mock<ILogger<PrescriptionsController>>().Object,
                _mockGpSystemFactory.Object,
                _mockAuditor.Object,
                _mockErrorReferenceGenerator.Object);
        }

        [TestMethod]
        public async Task Post_PrescriptionsServiceReturnsSuccess_ReturnsCreated()
        {
            // Arrange
            _repeatPrescriptionRequest.CourseIds = new List<string> { "Course 1", "Course 2" };
            _mockPrescriptionsService.Setup(x => x.OrderPrescription(
                It.Is<GpLinkedAccountModel>(
                d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId),
                _repeatPrescriptionRequest))
                .Returns(Task.FromResult((OrderPrescriptionResult)new OrderPrescriptionResult.Success()));

            _mockPrescriptionValidationService
                .Setup(x => x.IsPostValid(_repeatPrescriptionRequest))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Post(_repeatPrescriptionRequest, _patientId, _userSession);

            // Assert
            _mockGpSystem.VerifyAll();
            _mockPrescriptionsService.VerifyAll();
            _mockPrescriptionValidationService.VerifyAll();
            _mockGpSystemFactory.VerifyAll();
            result.Should().BeAssignableTo<CreatedResult>();

            _mockAuditor.Verify(x => x.Audit(PostRequestAuditType, RequestAuditMessageFormat, "Course 1,Course 2"));
            _mockAuditor.Verify(x => x.Audit(PostResponseAuditType, "Repeat prescription request successfully created with course ids: {0}", "Course 1,Course 2"));
        }

        [TestMethod]
        public async Task Post_PrescriptionsServiceReturnsPartialSuccess_ReturnsAccepted()
        {
            // Arrange
            _repeatPrescriptionRequest.CourseIds = new List<string> { "Course 1", "Course 2" };
            var response = new PrescriptionRequestPostPartialSuccessResponse()
                {
                    SuccessfulOrders = new List<Order> { new Order { CourseId = "Success 1" }, new Order { CourseId = "Success 2" } },
                    UnsuccessfulOrders = new List<Order> { new Order { CourseId = "Failed 1" }, new Order { CourseId = "Failed 2" } }
            };
            _mockPrescriptionsService.Setup(x => x.OrderPrescription(
                    It.Is<GpLinkedAccountModel>(
                        d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId),
                    _repeatPrescriptionRequest))
                .Returns(Task.FromResult((OrderPrescriptionResult)new OrderPrescriptionResult.PartialSuccess(response)));

            _mockPrescriptionValidationService
                .Setup(x => x.IsPostValid(_repeatPrescriptionRequest))
                .Returns(true);

            // Act
            var result = await _systemUnderTest.Post(_repeatPrescriptionRequest, _patientId, _userSession);

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

            _mockAuditor.Verify(x => x.Audit(PostRequestAuditType, RequestAuditMessageFormat, "Course 1,Course 2"));
            _mockAuditor.Verify(
                x => x.Audit(
                    PostResponseAuditType,
                    "Partial Success ordering prescription: Attempted to order course ids: {0}, Successful course ids: {1}, Unsuccessful course ids: {2}",
                    "Course 1,Course 2",
                    "Success 1,Success 2",
                    "Failed 1,Failed 2"));
        }

        [TestMethod]
        public async Task Post_ModelValidationFails_ReturnsBadRequest()
        {
            // Arrange
            _repeatPrescriptionRequest.CourseIds = new List<string> { "Course 1", "Course 2" };
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
            var result = await _systemUnderTest.Post(_repeatPrescriptionRequest, _patientId, _userSession);

            // Assert
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockAuditor.Verify(x => x.Audit(PostRequestAuditType, RequestAuditMessageFormat, "Course 1,Course 2"));
            _mockAuditor.Verify(x => x.Audit(PostResponseAuditType, "Error creating prescription request: Bad Request with course ids: {0}", "Course 1,Course 2"));
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
            _repeatPrescriptionRequest.CourseIds = new List<string> { "Course 1", "Course 2" };
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
            var result = await _systemUnderTest.Post(_repeatPrescriptionRequest, _patientId, _userSession);

            // Assert
            _mockPrescriptionsService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(expectedStatusCode);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockAuditor.Verify(x => x.Audit(PostRequestAuditType, RequestAuditMessageFormat, "Course 1,Course 2"));
            _mockAuditor.Verify(x => x.Audit(PostResponseAuditType, expectedAuditResponseMessageFormat, "Course 1,Course 2"));
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}