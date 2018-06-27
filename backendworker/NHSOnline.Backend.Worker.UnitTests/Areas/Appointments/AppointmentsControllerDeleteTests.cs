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

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Appointments
{
    [TestClass]
    public class AppointmentsControllerDeleteTests
    {
        private IFixture _fixture;
        private AppointmentCancelRequest _appointmentCancelRequest;
        private EmisUserSession _userSession;
        private Mock<IAppointmentsService> _mockAppointmentsService;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private AppointmentsController _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _appointmentCancelRequest = _fixture.Freeze<AppointmentCancelRequest>();

            _userSession = _fixture.Create<EmisUserSession>();

            _mockAppointmentsService = _fixture.Freeze<Mock<IAppointmentsService>>();

            var result = new AppointmentCancelResult.SuccessfullyCancelled();
            _mockAppointmentsService.Setup(x => x.Cancel(_userSession, _appointmentCancelRequest))
                .Returns(Task.FromResult((AppointmentCancelResult) result));

            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockGpSystem
                .Setup(x => x.GetAppointmentsService())
                .Returns(_mockAppointmentsService.Object);

            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(SupplierEnum.Emis))
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
        public async Task Delete_AppointmentsServiceCancelReturnsSuccess_ReturnsNoContent()
        {
            // Arrange

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task Delete_AppointmentsServiceCancelReturnsInsufficientPermissions_ReturnsForbidden()
        {
            // Arrange
            var serviceResult = new AppointmentCancelResult.InsufficientPermissions();
            _mockAppointmentsService.Setup(x => x.Cancel(_userSession, _appointmentCancelRequest))
                .Returns(Task.FromResult((AppointmentCancelResult)serviceResult));

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
            _mockAppointmentsService.Verify();
        }

        [TestMethod]
        public async Task Delete_AppointmentsServiceCancelReturnsAppointmentNotCancellable_ReturnsConflict()
        {
            // Arrange
            var serviceResult = new AppointmentCancelResult.AppointmentNotCancellable();
            _mockAppointmentsService.Setup(x => x.Cancel(_userSession, _appointmentCancelRequest))
                .Returns(Task.FromResult((AppointmentCancelResult)serviceResult));

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            _mockAppointmentsService.Verify();
        }

        [TestMethod]
        public async Task Delete_AppointmentsServiceCancelReturnsBadRequest_ReturnsBadRequest()
        {
            // Arrange
            var serviceResult = new AppointmentCancelResult.BadRequest();
            _mockAppointmentsService.Setup(x => x.Cancel(_userSession, _appointmentCancelRequest))
                .Returns(Task.FromResult((AppointmentCancelResult)serviceResult));

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
            _mockAppointmentsService.Verify();
        }

        [TestMethod]
        public async Task Delete_AppointmentsServiceCancelReturnsSupplierSystemUnavailable_ReturnsBadGateway()
        {
            // Arrange
            var serviceResult = new AppointmentCancelResult.SupplierSystemUnavailable();
            _mockAppointmentsService.Setup(x => x.Cancel(_userSession, _appointmentCancelRequest))
                .Returns(Task.FromResult((AppointmentCancelResult)serviceResult));

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockAppointmentsService.Verify();
        }

        [TestMethod]
        public async Task Delete_HappyPath_VerifyAllExpectationsOnMocks()
        {
            // Arrange

            // Act
            await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            _mockGpSystem.VerifyAll();
            _mockAppointmentsService.VerifyAll();
            _mockGpSystemFactory.VerifyAll();
        }
    }
}