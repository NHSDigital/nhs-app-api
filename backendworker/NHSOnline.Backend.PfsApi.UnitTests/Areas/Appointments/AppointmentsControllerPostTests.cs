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
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Appointments
{
    [TestClass]
    public sealed class AppointmentsControllerPostTests : IDisposable
    {
        private AppointmentsController _systemUnderTest;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IAppointmentsService> _mockAppointmentsService;
        private Mock<IAppointmentsValidationService> _mockAppointmentsValidationService;
        private AppointmentBookRequest _appointmentBookRequest;
        private EmisUserSession _gpSession;
        private P9UserSession _userSession;
        private Mock<IAuditor> _mockAuditor;
        private Mock<IErrorReferenceGenerator> _mockErrorReferenceGenerator;
        private string _serviceDeskReference;
        private Guid _patientId;
        private Mock<IAnonymousMetricLogger> _mockAnonymousMetricLogger;

        private const string RequestAuditType = "Appointments_Book_Request";
        private const string ResponseAuditType = "Appointments_Book_Response";

        private const string AppointmentSessionName = "Mock Session Name";
        private const string AppointmentSlotType = "Mock Slot Type";

        private string RequestAuditMessage()  =>
            $"Attempting to book appointment with id: { _appointmentBookRequest.SlotId} and startTime: {_appointmentBookRequest.StartTime:O}";

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();

            _gpSession = new EmisUserSession();

            _userSession = new P9UserSession("csrfToken", "nhsNumber", new CitizenIdUserSession(), _gpSession, "im1token");

            _appointmentBookRequest = new AppointmentBookRequest()
            {
                SessionName = AppointmentSessionName,
                SlotType = AppointmentSlotType,
            };

            _mockAppointmentsService = new Mock<IAppointmentsService>();

            _mockAppointmentsValidationService = new Mock<IAppointmentsValidationService>();

            _mockAppointmentsValidationService.Setup(x => x.IsPostValid(_appointmentBookRequest))
                .Returns(true);

            _mockAuditor = new Mock<IAuditor>();
            _mockAnonymousMetricLogger = new Mock<IAnonymousMetricLogger>();

            _mockAppointmentsService.Setup(x => x.Book(
                    It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId),
                    _appointmentBookRequest))
                .Returns(Task.FromResult((AppointmentBookResult) new AppointmentBookResult.Success()));

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
            _serviceDeskReference = "service desk ref";

            _systemUnderTest = new AppointmentsController(
                new Mock<ILogger<AppointmentsController>>().Object,
                _mockGpSystemFactory.Object,
                _mockAuditor.Object,
                new Mock<ISessionCacheService>().Object,
                _mockErrorReferenceGenerator.Object,
                new Mock<IAppointmentTypeTransformingVisitor>().Object,
                _mockAnonymousMetricLogger.Object,
                new Mock<IMetricLogger>().Object);
        }

        [TestMethod]
        public async Task Post_AppointmentsServiceBookReturnsSuccess_ReturnsCreated()
        {
            // Act
            var result = await _systemUnderTest.Post(_appointmentBookRequest, _patientId, _gpSession, _userSession);

            // Assert
            result.Should().BeAssignableTo<CreatedResult>();
        }

        [TestMethod]
        public async Task Post_MetricLogCreated()
        {
            // Arrange
            AppointmentMetricData appointmentMetricData = null;
            _mockAnonymousMetricLogger.Setup(x => x.AppointmentBookResult(It.IsAny<AppointmentMetricData>()))
                .Returns(Task.CompletedTask)
                .Callback<AppointmentMetricData>((data)=> appointmentMetricData = data);

            // Act
            var result = await _systemUnderTest.Post(_appointmentBookRequest, _patientId, _gpSession, _userSession);

            // Assert
            result.Should().BeAssignableTo<CreatedResult>();
            appointmentMetricData.ToKeyValuePairs().Should().BeEquivalentTo(new[]
            {
                new KeyValuePair<string, string>("SessionName", AppointmentSessionName),
                new KeyValuePair<string, string>("SlotType", AppointmentSlotType),
                new KeyValuePair<string, string>("StatusCode", "201")
            });
        }

        [TestMethod]
        public async Task Post_RequestIsInvalid_ReturnsObjectResultWith400StatusCode()
        {
            // Arrange
            _mockAppointmentsValidationService.Setup(x => x.IsPostValid(_appointmentBookRequest))
                .Returns(false);
            _mockErrorReferenceGenerator.Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Appointments,
                    StatusCodes.Status400BadRequest, _userSession.GpUserSession.Supplier))
                .Returns(_serviceDeskReference);

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.Post(_appointmentBookRequest, _patientId, _gpSession, _userSession);

            // Assert
            _mockAppointmentsService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage()));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Unable to book appointment due to bad request for appointment with id: {0} and startDateTime: {1:O}",
                _appointmentBookRequest.SlotId, _appointmentBookRequest.StartTime));
        }

        [DataTestMethod]
        [DataRow(typeof(AppointmentBookResult.Forbidden),StatusCodes.Status403Forbidden,
            "Unable to book appointment due to insufficient permissions for appointment with id: {0} and startDateTime: {1:O}")]
        [DataRow(typeof(AppointmentBookResult.SlotNotAvailable),StatusCodes.Status409Conflict,
            "Unable to book appointment due to appointment being unavailable for appointment with id: {0} and startDateTime: {1:O}")]
        [DataRow(typeof(AppointmentBookResult.BadGateway),StatusCodes.Status502BadGateway,
            "Unable to book appointment due to unavailable supplier for appointment with id: {0} and startDateTime: {1:O}")]
        [DataRow(typeof(AppointmentBookResult.BadRequest),StatusCodes.Status400BadRequest,
            "Unable to book appointment due to bad request for appointment with id: {0} and startDateTime: {1:O}")]
        [DataRow(typeof(AppointmentBookResult.AppointmentLimitReached),Constants.CustomHttpStatusCodes.Status460LimitReached,
            "Unable to book appointment due to appointment limit reached for appointment with id: {0} and startDateTime: {1:O}")]
        [DataRow(typeof(AppointmentBookResult.InternalServerError),StatusCodes.Status500InternalServerError,
            "Unable to book appointment due to internal server error for appointment with id: {0} and startDateTime: {1:O}")]
        public async Task Post_ServiceReturnsErrorResult_ReturnsAppropriateResultObject(
            Type serviceResultType,
            int expectedStatusCode,
            string expectedAuditResponseMessageFormat)
        {
            // Arrange
            var serviceResult = (AppointmentBookResult) Activator.CreateInstance(serviceResultType);
            _mockAppointmentsService.Setup(x => x.Book(
                It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession && d.PatientId == _patientId),
                    _appointmentBookRequest))
                .Returns(Task.FromResult(serviceResult));
            _mockErrorReferenceGenerator.Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Appointments,
                    expectedStatusCode, _userSession.GpUserSession.Supplier))
                .Returns(_serviceDeskReference);

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.Post(_appointmentBookRequest, _patientId, _gpSession, _userSession);

            // Assert
            _mockAppointmentsService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(expectedStatusCode);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage()));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, expectedAuditResponseMessageFormat,
                _appointmentBookRequest.SlotId, _appointmentBookRequest.StartTime));
        }

        [TestMethod]
        public async Task Post_HappyPath_VerifyAllExpectationsOnMocks()
        {
            // Act
            await _systemUnderTest.Post(_appointmentBookRequest, _patientId, _gpSession, _userSession);

            // Assert
            _mockGpSystem.VerifyAll();
            _mockAppointmentsService.VerifyAll();
            _mockGpSystemFactory.VerifyAll();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage()));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Appointment successfully booked for appointment with id: {0} and startDateTime: {1:O}",
                _appointmentBookRequest.SlotId, _appointmentBookRequest.StartTime));
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}
