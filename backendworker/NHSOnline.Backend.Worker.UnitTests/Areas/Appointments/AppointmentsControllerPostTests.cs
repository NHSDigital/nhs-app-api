using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Appointments
{
    [TestClass]
    public class AppointmentsControllerPostTests
    {
        private AppointmentsController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IAppointmentsService> _mockAppointmentsService;
        private AppointmentBookRequest _appointmentBookRequest;
        private EmisUserSession _userSession;
        private Mock<IAuditor> _mockAuditor;

        private const string RequestAuditType = "Appointments_Book_Request";
        private const string ResponseAuditType = "Appointments_Book_Response";

        private const string RequestAuditMessage =
            "Attempting to book appointment with id: {0} and startTime: {1:O}";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _appointmentBookRequest = _fixture.Freeze<AppointmentBookRequest>();

            _userSession = _fixture.Create<EmisUserSession>();

            _mockAppointmentsService = _fixture.Freeze<Mock<IAppointmentsService>>();

            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            _mockAppointmentsService.Setup(x => x.Book(_userSession, _appointmentBookRequest))
                .Returns(Task.FromResult((AppointmentBookResult) new AppointmentBookResult.SuccessfullyBooked()));

            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockGpSystem
                .Setup(x => x.GetAppointmentsService())
                .Returns(_mockAppointmentsService.Object);

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
        }

        [TestMethod]
        public async Task Post_AppointmentsServiceBookReturnsSuccess_ReturnsCreated()
        {
            // Arrange

            // Act
            var result = await _systemUnderTest.Post(_appointmentBookRequest);

            // Assert
            result.Should().BeAssignableTo<CreatedResult>();
        }

        [TestMethod]
        public async Task Post_AppointmentsServiceBookReturnsInsufficientPermissions_ReturnsForbidden()
        {
            // Arrange
            _mockAppointmentsService.Setup(x => x.Book(_userSession, _appointmentBookRequest))
                .Returns(Task.FromResult((AppointmentBookResult) new AppointmentBookResult.InsufficientPermissions()));

            // Act
            var result = await _systemUnderTest.Post(_appointmentBookRequest);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
            _mockAppointmentsService.Verify();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage, _appointmentBookRequest.SlotId,
                _appointmentBookRequest.StartTime));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Unable to book appointment due to insufficent permissions for appointment with id: {0} and startDateTime: {1:O}",
                _appointmentBookRequest.SlotId, _appointmentBookRequest.StartTime));
        }

        [TestMethod]
        public async Task Post_AppointmentsServiceBookReturnsLimitReached_ReturnsLimitReachedStatus()
        {
            // Arrange
            _mockAppointmentsService.Setup(x => x.Book(_userSession, _appointmentBookRequest))
                .Returns(Task.FromResult((AppointmentBookResult) new AppointmentBookResult.AppointmentLimitReached()));

            // Act
            var result = await _systemUnderTest.Post(_appointmentBookRequest);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(Constants.CustomHttpStatusCodes.Status460LimitReached);
            _mockAppointmentsService.Verify();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage, _appointmentBookRequest.SlotId,
                _appointmentBookRequest.StartTime));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Unable to book appointment due appointment limit reached for appointment with id: {0} and startDateTime: {1:O}",
                _appointmentBookRequest.SlotId, _appointmentBookRequest.StartTime));
        }

        [TestMethod]
        public async Task Post_AppointmentsServiceBookReturnsSlotNotAvailable_ReturnsConflict()
        {
            // Arrange
            _mockAppointmentsService.Setup(x => x.Book(_userSession, _appointmentBookRequest))
                .Returns(Task.FromResult((AppointmentBookResult) new AppointmentBookResult.SlotNotAvailable()));

            // Act
            var result = await _systemUnderTest.Post(_appointmentBookRequest);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            _mockAppointmentsService.Verify();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage, _appointmentBookRequest.SlotId,
                _appointmentBookRequest.StartTime));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Unable to book appointment due to appointment being unavailable for appointment with id: {0} and startDateTime: {1:O}",
                _appointmentBookRequest.SlotId, _appointmentBookRequest.StartTime));
        }

        [TestMethod]
        public async Task Post_AppointmentsServiceBookReturnsSupplierSystemUnavailable_ReturnsBadGateway()
        {
            // Arrange
            _mockAppointmentsService.Setup(x => x.Book(_userSession, _appointmentBookRequest))
                .Returns(Task.FromResult((AppointmentBookResult) new AppointmentBookResult.SupplierSystemUnavailable()));

            // Act
            var result = await _systemUnderTest.Post(_appointmentBookRequest);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockAppointmentsService.Verify();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage, _appointmentBookRequest.SlotId,
                _appointmentBookRequest.StartTime));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Unable to book appointment due to unavailable supplier for appointment with id: {0} and startDateTime: {1:O}",
                _appointmentBookRequest.SlotId, _appointmentBookRequest.StartTime));
        }

        [TestMethod]
        public async Task Post_HappyPath_VerifyAllExpectationsOnMocks()
        {
            // Arrange

            // Act
            await _systemUnderTest.Post(_appointmentBookRequest);

            // Assert
            _mockGpSystem.VerifyAll();
            _mockAppointmentsService.VerifyAll();
            _mockGpSystemFactory.VerifyAll();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage, _appointmentBookRequest.SlotId,
                _appointmentBookRequest.StartTime));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Appointment successfully booked for appointment with id: {0} and startDateTime: {1:O}",
                _appointmentBookRequest.SlotId, _appointmentBookRequest.StartTime));
        }
    }
}
