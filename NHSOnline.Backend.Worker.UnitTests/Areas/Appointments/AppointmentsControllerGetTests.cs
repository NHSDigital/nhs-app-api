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
    public class AppointmentsControllerGetTests
    {
        private MyAppointmentsResponse _myAppointmentsResponse;
        private AppointmentsController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IAppointmentsService> _mockAppointmentsService;
        private EmisUserSession _userSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _userSession = _fixture.Create<EmisUserSession>();

            _mockAppointmentsService = _fixture.Freeze<Mock<IAppointmentsService>>();

            _myAppointmentsResponse = _fixture.Create<MyAppointmentsResponse>();
            var result = new MyAppointmentsResult.SuccessfullyRetrieved(_myAppointmentsResponse);

            _mockAppointmentsService.Setup(x => x.GetMyAppointments(_userSession, false, null))
                .Returns(Task.FromResult((MyAppointmentsResult)result));

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
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfulResult()
        {
            // Arrange
            var badResult = new MyAppointmentsResult.BadRequest();
            _mockAppointmentsService.Setup(x => x.GetMyAppointments(_userSession, false, null))
                .Returns(Task.FromResult((MyAppointmentsResult)badResult));

            // Act
            var result = await _systemUnderTest.Get(false);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
            _mockAppointmentsService.Verify();
        }


        [TestMethod]
        public async Task Get_ReturnsBadGateway_WhenServiceReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var badResult = new MyAppointmentsResult.SupplierSystemUnavailable();
            _mockAppointmentsService.Setup(x => x.GetMyAppointments(_userSession, false, null))
                .Returns(Task.FromResult((MyAppointmentsResult)badResult));

            // Act
            var result = await _systemUnderTest.Get(false);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockAppointmentsService.Verify();
        }

        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenServiceReturnsBadRequest()
        {
            // Arrange
            var badResult = new MyAppointmentsResult.BadRequest();
            _mockAppointmentsService.Setup(x => x.GetMyAppointments(_userSession, false, null))
                .Returns(Task.FromResult((MyAppointmentsResult)badResult));

            // Act
            var result = await _systemUnderTest.Get(false);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
            _mockAppointmentsService.Verify();
        }

        [TestMethod]
        public async Task Get_ReturnsInternalServerError_WhenServiceReturnsInternalServerError()
        {
            // Arrange
            var badResult = new MyAppointmentsResult.InternalServerError();
            _mockAppointmentsService.Setup(x => x.GetMyAppointments(_userSession, false, null))
                .Returns(Task.FromResult((MyAppointmentsResult)badResult));

            // Act
            var result = await _systemUnderTest.Get(false);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            _mockAppointmentsService.Verify();
        }

        [TestMethod]
        public async Task Get_HappyPath_VerifyAllExpectationsOnMocks()
        {
            // Arrange

            // Act
            await _systemUnderTest.Get(false);

            // Assert
            _mockGpSystem.VerifyAll();
            _mockGpSystemFactory.VerifyAll();
            _mockAppointmentsService.VerifyAll();
        }
    }
}
