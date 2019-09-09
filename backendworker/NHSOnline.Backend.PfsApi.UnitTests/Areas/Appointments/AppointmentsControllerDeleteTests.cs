using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Appointments
{
    [TestClass]
    public class AppointmentsControllerDeleteTests
    {
        private IFixture _fixture;
        private AppointmentCancelRequest _appointmentCancelRequest;
        private UserSession _userSession;
        private Mock<IAppointmentsService> _mockAppointmentsService;
        private Mock<IAppointmentsValidationService> _mockAppointmentsValidationService;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private AppointmentsController _systemUnderTest;
        private Mock<IAuditor> _mockAuditor;

        private const string RequestAuditType = "Appointments_Cancel_Request";
        private const string ResponseAuditType = "Appointments_Cancel_Response";

        private string RequestAuditMessage;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            
            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));

            _appointmentCancelRequest = _fixture.Freeze<AppointmentCancelRequest>();

            _userSession = _fixture.Create<UserSession>();

            _mockAppointmentsService = _fixture.Freeze<Mock<IAppointmentsService>>();

            _mockAppointmentsValidationService = _fixture.Freeze<Mock<IAppointmentsValidationService>>();

            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            _mockAppointmentsService.Setup(x => x.Cancel(_userSession.GpUserSession, _appointmentCancelRequest))
                .Returns(Task.FromResult((AppointmentCancelResult) new AppointmentCancelResult.Success()));

            _mockAppointmentsValidationService.Setup(x => x.IsDeleteValid(_appointmentCancelRequest))
                .Returns(true);

            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockGpSystem
                .Setup(x => x.GetAppointmentsService())
                .Returns(_mockAppointmentsService.Object);

            _mockGpSystem
                .Setup(x => x.GetAppointmentsValidationService())
                .Returns(_mockAppointmentsValidationService.Object);

            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Items[Constants.HttpContextItems.UserSession]).Returns(_userSession);

            _systemUnderTest = _fixture.Create<AppointmentsController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            RequestAuditMessage = $"Attempting to cancel appointment with id: {_appointmentCancelRequest.AppointmentId}";
        }

        [TestMethod]
        public async Task Delete_AppointmentsServiceCancelReturnsSuccess_ReturnsNoContent()
        {
            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task Delete_AppointmentsServiceCancelReturnsInsufficientPermissions_ReturnsForbidden()
        {
            // Arrange
            var serviceResult = new AppointmentCancelResult.Forbidden();
            _mockAppointmentsService.Setup(x => x.Cancel(_userSession.GpUserSession, _appointmentCancelRequest))
                .Returns(Task.FromResult((AppointmentCancelResult)serviceResult));

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
            _mockAppointmentsService.Verify();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Unable to cancel appointment due to insufficient permissions for appointment with id: {0}",
                _appointmentCancelRequest.AppointmentId));
        }

        [TestMethod]
        public async Task Delete_AppointmentsServiceCancelReturnsAppointmentNotCancellable_ReturnsConflict()
        {
            // Arrange
            var badResult = new AppointmentCancelResult.AppointmentNotCancellable();
            _mockAppointmentsService.Setup(x => x.Cancel(_userSession.GpUserSession, _appointmentCancelRequest))
                .Returns(Task.FromResult((AppointmentCancelResult)badResult));

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            _mockAppointmentsService.Verify();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Unable to cancel appointment due to it not being cancellable appointment with id: {0}",
                _appointmentCancelRequest.AppointmentId));
        }
        
        [TestMethod]
        public async Task Delete_AppointmentsServiceCancelReturnsTooLateToCancel_ReturnsTooLateStatus()
        {
            // Arrange
            var badResult = new AppointmentCancelResult.TooLateToCancel();
            _mockAppointmentsService.Setup(x => x.Cancel(_userSession.GpUserSession, _appointmentCancelRequest))
                .Returns(Task.FromResult((AppointmentCancelResult)badResult));

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(Constants.CustomHttpStatusCodes.Status461TooLate);
            _mockAppointmentsService.Verify();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Unable to cancel appointment due to it being too late to cancel with id: {0}",
                _appointmentCancelRequest.AppointmentId));
        }

        [TestMethod]
        public async Task Delete_AppointmentsServiceCancelReturnsBadRequest_ReturnsBadRequest()
        {
            // Arrange
            var badResult = new AppointmentCancelResult.BadRequest();
            _mockAppointmentsService.Setup(x => x.Cancel(_userSession.GpUserSession, _appointmentCancelRequest))
                .Returns(Task.FromResult((AppointmentCancelResult)badResult));

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
            _mockAppointmentsService.Verify();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Unable to cancel appointment due to a bad request for appointment with id: {0}",
                _appointmentCancelRequest.AppointmentId));
        }

        [TestMethod]
        public async Task Delete_AppointmentsServiceCancelReturnsBadGateway_ReturnsBadGateway()
        {
            // Arrange
            var badResult = new AppointmentCancelResult.BadGateway();
            _mockAppointmentsService.Setup(x => x.Cancel(_userSession.GpUserSession, _appointmentCancelRequest))
                .Returns(Task.FromResult((AppointmentCancelResult)badResult));

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockAppointmentsService.Verify();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Unable to cancel appointment due to unavailable supplier for appointment with id: {0}",
                _appointmentCancelRequest.AppointmentId));
        }

        [TestMethod]
        public async Task Delete_HappyPath_VerifyAllExpectationsOnMocks()
        {
            // Act
            await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            _mockGpSystem.VerifyAll();
            _mockAppointmentsService.VerifyAll();
            _mockGpSystemFactory.VerifyAll();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Appointment successfully cancelled for appointment with id: {0}",
                _appointmentCancelRequest.AppointmentId));
        }
    }
}

