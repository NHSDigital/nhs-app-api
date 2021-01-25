using System;
using System.Globalization;
using System.Linq;
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
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Appointments
{
    [TestClass]
    public sealed class AppointmentsControllerGetTests : IDisposable
    {
        private AppointmentsResponse _appointmentsResponse;
        private AppointmentsController _systemUnderTest;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IAppointmentsService> _mockAppointmentsService;
        private EmisUserSession _gpSession;
        private P9UserSession _userSession;
        private Mock<IAuditor> _mockAuditor;
        private Mock<ILogger<AppointmentsController>> _mockLogger;
        private Mock<IErrorReferenceGenerator> _mockErrorReferenceGenerator;
        private Mock<IAppointmentTypeTransformingVisitor> _mockAppointmentTypeTransformingVisitor;
        private string _serviceDeskReference;
        private Guid _patientGuid;
        private AppointmentsResult.Success _serviceResult;

        private const string RequestAuditType = "Appointments_ViewBooked_Request";
        private const string ResponseAuditType = "Appointments_ViewBooked_Response";

        private const string RequestAuditMessage = "Attempting to view booked appointments";
        private const string ResponseAuditMessageFormat = "Booked appointments successfully viewed - {0} upcoming appointments" +
                                                          " and {1} historical appointments";
        [TestInitialize]
        public void TestInitialize()
        {
            _patientGuid = Guid.NewGuid();

            _gpSession = new EmisUserSession();

            _userSession = new P9UserSession("csrfToken",  "nhsNumber", new CitizenIdUserSession(), _gpSession, "im1token");

            _mockAppointmentsService = new Mock<IAppointmentsService>();

            _mockAuditor = new Mock<IAuditor>();
            _mockLogger = new Mock<ILogger<AppointmentsController>>();
            _mockAppointmentTypeTransformingVisitor = new Mock<IAppointmentTypeTransformingVisitor>();

            _appointmentsResponse = new AppointmentsResponse();
            _serviceResult = new AppointmentsResult.Success(_appointmentsResponse);

            _mockAppointmentsService.Setup(x => x.GetAppointments(
                It.Is<GpLinkedAccountModel>(
                    d => d.GpUserSession == _userSession.GpUserSession
                         && d.PatientId == _patientGuid)))
                .Returns(Task.FromResult((AppointmentsResult)_serviceResult));

            _mockGpSystem = new Mock<IGpSystem>();
            _mockGpSystem
                .Setup(x => x.GetAppointmentsService())
                .Returns(_mockAppointmentsService.Object);

            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);

            _mockErrorReferenceGenerator = new Mock<IErrorReferenceGenerator>();
            _serviceDeskReference = "service desk ref";

            _systemUnderTest = new AppointmentsController(
                _mockLogger.Object,
                _mockGpSystemFactory.Object,
                _mockAuditor.Object,
                new Mock<ISessionCacheService>().Object,
                _mockErrorReferenceGenerator.Object,
                _mockAppointmentTypeTransformingVisitor.Object,
                new Mock<IAnonymousMetricLogger>().Object,
                new Mock<IMetricLogger>().Object);
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfulResult()
        {
            // Act
            var result = await _systemUnderTest.Get(_patientGuid, _userSession, _gpSession);

            // Assert
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(_appointmentsResponse);
            _mockAppointmentsService.Verify();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            var expectedResponseAuditMessage = string.Format(CultureInfo.InvariantCulture, ResponseAuditMessageFormat,
                _appointmentsResponse.UpcomingAppointments.Count(),
                _appointmentsResponse.PastAppointments.Count());
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, expectedResponseAuditMessage));
        }

        [TestMethod]
        public async Task Get_ServiceReturnsResult_SlotTypesAreTransformed()
        {
            // Act
            await _systemUnderTest.Get(_patientGuid, _userSession, _gpSession);

            // Assert
            _mockAppointmentTypeTransformingVisitor.Verify(x => x.Visit(_serviceResult));
        }

        [TestMethod]
        public async Task Get_ReturnsSuccessfulResult_LogsAppointmentCount()
        {
            // Act
            await _systemUnderTest.Get(_patientGuid, _userSession, _gpSession);

            // Assert
            var expectedLogMessage =
                $"Appointment Count: Supplier=Emis OdsCode={_userSession.GpUserSession.OdsCode} " +
                $"Count={_appointmentsResponse.UpcomingAppointments.Count()+_appointmentsResponse.PastAppointments.Count()} " +
                $"UpcomingCount={_appointmentsResponse.UpcomingAppointments.Count()} " +
                $"HistoricalCount={_appointmentsResponse.PastAppointments.Count()}";
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }

        [DataTestMethod]
        [DataRow(typeof(AppointmentsResult.BadRequest), StatusCodes.Status400BadRequest,
            "Booked appointments view unsuccessful due to bad request")]
        [DataRow(typeof(AppointmentsResult.BadGateway), StatusCodes.Status502BadGateway,
            "Booked appointments view unsuccessful due to supplier being unavailable")]
        [DataRow(typeof(AppointmentsResult.InternalServerError), StatusCodes.Status500InternalServerError,
            "Booked appointments view unsuccessful due to internal server error")]
        [DataRow(typeof(AppointmentsResult.Forbidden), StatusCodes.Status403Forbidden,
            "Booked appointments view unsuccessful due to insufficient permissions")]
        public async Task Get_ServiceReturnsErrorResult_ReturnsAppropriateResultObject(
            Type serviceResultType,
            int expectedStatusCode,
            string expectedAuditResponseMessageFormat)
        {
            // Arrange
            var serviceResult = (AppointmentsResult) Activator.CreateInstance(serviceResultType);
            _mockAppointmentsService.Setup(x => x.GetAppointments(
                    It.Is<GpLinkedAccountModel>(
                        d => d.GpUserSession == _userSession.GpUserSession
                             && d.PatientId == _patientGuid)))
                .Returns(Task.FromResult(serviceResult));
            _mockErrorReferenceGenerator.Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Appointments,
                    expectedStatusCode, _userSession.GpUserSession.Supplier))
                .Returns(_serviceDeskReference);

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.Get(_patientGuid, _userSession, _gpSession);

            // Assert
            _mockAppointmentsService.Verify();
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
        public async Task Get_HappyPath_VerifyAllExpectationsOnMocks()
        {
            // Act
            await _systemUnderTest.Get(_patientGuid, _userSession, _gpSession);

            // Assert
            _mockGpSystem.VerifyAll();
            _mockGpSystemFactory.VerifyAll();
            _mockAppointmentsService.VerifyAll();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));

            var expectedResponseAuditMessage = string.Format(CultureInfo.InvariantCulture, ResponseAuditMessageFormat,
                _appointmentsResponse.UpcomingAppointments.Count(),
                _appointmentsResponse.PastAppointments.Count());
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, expectedResponseAuditMessage));
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}
