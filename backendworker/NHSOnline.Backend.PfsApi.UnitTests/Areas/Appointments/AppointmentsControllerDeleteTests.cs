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
using NHSOnline.Backend.PfsApi.Areas.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Appointments
{
    [TestClass]
    public sealed class AppointmentsControllerDeleteTests : IDisposable
    {
        private AppointmentCancelRequest _appointmentCancelRequest;
        private EmisUserSession _gpSession;
        private Mock<IAppointmentsService> _mockAppointmentsService;
        private Mock<IAppointmentsValidationService> _mockAppointmentsValidationService;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private AppointmentsController _systemUnderTest;
        private Mock<IAuditor> _mockAuditor;
        private Mock<IErrorReferenceGenerator> _mockErrorReferenceGenerator;
        private string _serviceDeskReference;
        private Guid _patientId;
        private P9UserSession _userSession;

        private const string RequestAuditType = "Appointments_Cancel_Request";
        private const string ResponseAuditType = "Appointments_Cancel_Response";

        private const string AppointmentSessionName = "Mock Session Name";
        private const string AppointmentSlotType = "Mock Slot Type";

        private string _requestAuditMessage;
        private Mock<IAnonymousMetricLogger> _mockAnonymousMetricLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();

            _appointmentCancelRequest = new AppointmentCancelRequest()
            {
                SessionName = AppointmentSessionName,
                SlotType = AppointmentSlotType
            };

            _gpSession = new EmisUserSession();

            _userSession = new P9UserSession("csrfToken", "nhsNumber", new CitizenIdUserSession(), _gpSession, "im1token");

            _mockAppointmentsService = new Mock<IAppointmentsService>();

            _mockAppointmentsValidationService = new Mock<IAppointmentsValidationService>();

            _mockAuditor = new Mock<IAuditor>();
            _mockAnonymousMetricLogger = new Mock<IAnonymousMetricLogger>();

            _mockAppointmentsService.Setup(x => x.Cancel(
                It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId),
                    _appointmentCancelRequest))
                .Returns(Task.FromResult((AppointmentCancelResult) new AppointmentCancelResult.Success()));

            _mockAppointmentsValidationService.Setup(x => x.IsDeleteValid(_appointmentCancelRequest))
                .Returns(true);

            _mockGpSystem = new Mock<IGpSystem>();
            _mockGpSystem
                .Setup(x => x.GetAppointmentsService())
                .Returns(_mockAppointmentsService.Object);

            _mockGpSystem
                .Setup(x => x.GetAppointmentsValidationService())
                .Returns(_mockAppointmentsValidationService.Object);

            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);

            _mockErrorReferenceGenerator = new Mock<IErrorReferenceGenerator>();
            _serviceDeskReference = "Error reference";

            _systemUnderTest = new AppointmentsController(
                new Mock<ILogger<AppointmentsController>>().Object,
                _mockGpSystemFactory.Object,
                _mockAuditor.Object,
                new Mock<ISessionCacheService>().Object,
                _mockErrorReferenceGenerator.Object,
                new Mock<IAppointmentTypeTransformingVisitor>().Object,
                _mockAnonymousMetricLogger.Object,
                new Mock<IMetricLogger>().Object);

            _requestAuditMessage = $"Attempting to cancel appointment with id: {_appointmentCancelRequest.AppointmentId}";
        }

        [TestMethod]
        public async Task Delete_AppointmentsServiceCancelReturnsSuccess_ReturnsNoContent()
        {
            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest, _patientId, _gpSession, _userSession);

            // Assert
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task Delete_MetricLogCreated()
        {
            // Arrange
            AppointmentMetricData appointmentMetricData = null;
            _mockAnonymousMetricLogger.Setup(x => x.AppointmentCancelResult(It.IsAny<AppointmentMetricData>()))
                .Returns(Task.CompletedTask)
                .Callback<AppointmentMetricData>((data) => appointmentMetricData = data);

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest, _patientId, _gpSession, _userSession);

            // Assert
            result.Should().BeAssignableTo<NoContentResult>();
            appointmentMetricData.ToKeyValuePairs().Should().BeEquivalentTo(new[]
            {
                new KeyValuePair<string, string>("SessionName", AppointmentSessionName),
                new KeyValuePair<string, string>("SlotType", AppointmentSlotType),
                new KeyValuePair<string, string>("StatusCode", "204")
            });
        }

        [TestMethod]
        public async Task Delete_RequestIsInvalid_ReturnsObjectResultWith400StatusCode()
        {
            // Arrange
            _mockAppointmentsValidationService.Setup(x => x.IsDeleteValid(_appointmentCancelRequest))
                .Returns(false);
            _mockErrorReferenceGenerator.Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Appointments,
                    StatusCodes.Status400BadRequest, _gpSession.Supplier))
                .Returns(_serviceDeskReference);

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest, _patientId, _gpSession, _userSession);

            // Assert
            _mockAppointmentsService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }
            _mockAuditor.Verify(x => x.PreOperationAudit(RequestAuditType, _requestAuditMessage));
            _mockAuditor.Verify(x => x.PostOperationAudit(ResponseAuditType, "Unable to cancel appointment due to a bad request for appointment with id: {0}",
                _appointmentCancelRequest.AppointmentId));
        }

        [DataTestMethod]
        [DataRow(typeof(AppointmentCancelResult.BadRequest), StatusCodes.Status400BadRequest,
            "Unable to cancel appointment due to a bad request for appointment with id: {0}")]
        [DataRow(typeof(AppointmentCancelResult.Forbidden), StatusCodes.Status403Forbidden,
            "Unable to cancel appointment due to insufficient permissions for appointment with id: {0}")]
        [DataRow(typeof(AppointmentCancelResult.AppointmentNotCancellable), StatusCodes.Status409Conflict,
            "Unable to cancel appointment due to it not being cancellable appointment with id: {0}")]
        [DataRow(typeof(AppointmentCancelResult.TooLateToCancel), Constants.CustomHttpStatusCodes.Status461TooLate,
            "Unable to cancel appointment due to it being too late to cancel with id: {0}")]
        [DataRow(typeof(AppointmentCancelResult.BadGateway), StatusCodes.Status502BadGateway,
            "Unable to cancel appointment due to unavailable supplier for appointment with id: {0}")]
        [DataRow(typeof(AppointmentCancelResult.InternalServerError), StatusCodes.Status500InternalServerError,
            "Unable to cancel appointment due to internal server error for appointment with id: {0}")]
        public async Task Delete_ServiceReturnsErrorResult_ReturnsAppropriateResultObject(
            Type serviceResultType,
            int expectedStatusCode,
            string expectedAuditResponseMessageFormat)
        {
            // Arrange
            var serviceResult = (AppointmentCancelResult) Activator.CreateInstance(serviceResultType);
            _mockAppointmentsService.Setup(x => x.Cancel(
                    It.Is<GpLinkedAccountModel>(
                        d => d.GpUserSession == _userSession.GpUserSession
                             && d.PatientId == _patientId),
                    _appointmentCancelRequest))
                .Returns(Task.FromResult(serviceResult));
            _mockErrorReferenceGenerator.Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Appointments,
                    expectedStatusCode, _userSession.GpUserSession.Supplier))
                .Returns(_serviceDeskReference);

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest, _patientId, _gpSession, _userSession);

            // Assert
            _mockAppointmentsService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(expectedStatusCode);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }
            _mockAuditor.Verify(x => x.PreOperationAudit(RequestAuditType, _requestAuditMessage));
            _mockAuditor.Verify(x => x.PostOperationAudit(ResponseAuditType, expectedAuditResponseMessageFormat,
                _appointmentCancelRequest.AppointmentId));
        }

        [TestMethod]
        public async Task Delete_HappyPath_VerifyAllExpectationsOnMocks()
        {
            // Act
            await _systemUnderTest.Delete(_appointmentCancelRequest, _patientId, _gpSession, _userSession);

            // Assert
            _mockGpSystem.VerifyAll();
            _mockAppointmentsService.VerifyAll();
            _mockGpSystemFactory.VerifyAll();
            _mockAuditor.Verify(x => x.PreOperationAudit(RequestAuditType, _requestAuditMessage));
            _mockAuditor.Verify(x => x.PostOperationAudit(ResponseAuditType,
                "Appointment successfully cancelled for appointment with id: {0}",
                _appointmentCancelRequest.AppointmentId));
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}

