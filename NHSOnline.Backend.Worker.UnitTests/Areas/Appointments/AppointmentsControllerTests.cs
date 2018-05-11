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
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Appointments;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Appointments
{
    [TestClass]
    public class AppointmentsControllerTests
    {
        private AppointmentsController _systemUnderTest;
        private IFixture _fixture;
        private Mock<ISystemProvider> _mockSystemProvider;
        private Mock<ISystemProviderFactory> _mockSystemProviderFactory;
        private Mock<IAppointmentsService> _mockAppointmentsService;
        private AppointmentBookRequest _appointmentBookRequest;
        private EmisUserSession _userSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _appointmentBookRequest = _fixture.Freeze<AppointmentBookRequest>();

            _userSession = _fixture.Create<EmisUserSession>();

            _mockAppointmentsService = _fixture.Freeze<Mock<IAppointmentsService>>();

            var result = new AppointmentBookResult.SuccessfullyBooked();
            _mockAppointmentsService.Setup(x => x.Book(_userSession, _appointmentBookRequest))
                .Returns(Task.FromResult((AppointmentBookResult) result));

            _mockSystemProvider = _fixture.Freeze<Mock<ISystemProvider>>();
            _mockSystemProvider
                .Setup(x => x.GetAppointmentsService())
                .Returns(_mockAppointmentsService.Object);

            _mockSystemProviderFactory = _fixture.Freeze<Mock<ISystemProviderFactory>>();
            _mockSystemProviderFactory
                .Setup(x => x.CreateSystemProvider(SupplierEnum.Emis))
                .Returns(_mockSystemProvider.Object);

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
            var serviceResult = new AppointmentBookResult.InsufficientPermissions();
            _mockAppointmentsService.Setup(x => x.Book(_userSession, _appointmentBookRequest))
                .Returns(Task.FromResult((AppointmentBookResult) serviceResult));

            // Act
            var result = await _systemUnderTest.Post(_appointmentBookRequest);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
            _mockAppointmentsService.Verify();
        }

        [TestMethod]
        public async Task Post_AppointmentsServiceBookReturnsSlotNotAvailable_ReturnsConflict()
        {
            // Arrange
            var serviceResult = new AppointmentBookResult.SlotNotAvailable();
            _mockAppointmentsService.Setup(x => x.Book(_userSession, _appointmentBookRequest))
                .Returns(Task.FromResult((AppointmentBookResult)serviceResult));

            // Act
            var result = await _systemUnderTest.Post(_appointmentBookRequest);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status409Conflict);
            _mockAppointmentsService.Verify();
        }

        [TestMethod]
        public async Task Post_AppointmentsServiceBookReturnsSupplierSystemUnavailable_ReturnsBadGateway()
        {
            // Arrange
            var serviceResult = new AppointmentBookResult.SupplierSystemUnavailable();
            _mockAppointmentsService.Setup(x => x.Book(_userSession, _appointmentBookRequest))
                .Returns(Task.FromResult((AppointmentBookResult)serviceResult));

            // Act
            var result = await _systemUnderTest.Post(_appointmentBookRequest);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockAppointmentsService.Verify();
        }

        [TestMethod]
        public async Task Post_HappyPath_VerifyAllExpectationsOnMocks()
        {
            // Arrange

            // Act
            await _systemUnderTest.Post(_appointmentBookRequest);

            // Assert
            _mockSystemProvider.VerifyAll();
            _mockAppointmentsService.VerifyAll();
            _mockSystemProviderFactory.VerifyAll();
        }
    }
}