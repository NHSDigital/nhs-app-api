using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Appointments
{
    [TestClass]
    public class AppointmentsControllerGetTests
    {
        private AppointmentsResponse _appointmentsResponse;
        private AppointmentsController _systemUnderTest;
        private IFixture _fixture;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IAppointmentsService> _mockAppointmentsService;
        private UserSession _userSession;
        private Mock<IAuditor> _mockAuditor;

        private const string RequestAuditType = "Appointments_ViewBooked_Request";
        private const string ResponseAuditType = "Appointments_ViewBooked_Response";

        private const string RequestAuditMessage = "Attempting to view booked appointments";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));

            _userSession = _fixture.Create<UserSession>();

            _mockAppointmentsService = _fixture.Freeze<Mock<IAppointmentsService>>();

            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            _appointmentsResponse = _fixture.Create<AppointmentsResponse>();
            var result = new AppointmentsResult.SuccessfullyRetrieved(_appointmentsResponse);

            _mockAppointmentsService.Setup(x => x.GetAppointments(_userSession.GpUserSession))
                .Returns(Task.FromResult((AppointmentsResult)result));

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
        public async Task Get_ReturnsSuccessfulResult_WhenServiceReturnsSuccessfulResult()
        {
            // Arrange
            var appointmentsResponse = _fixture.Create<AppointmentsResponse>();
            var successResponse = new AppointmentsResult.SuccessfullyRetrieved(appointmentsResponse);

            _mockAppointmentsService.Setup(x => x.GetAppointments(_userSession.GpUserSession))
                .Returns(Task.FromResult((AppointmentsResult) successResponse));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            var value = result.Should().BeAssignableTo<OkObjectResult>().Subject.Value;
            value.Should().BeEquivalentTo(appointmentsResponse);
            _mockAppointmentsService.Verify();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, $"Booked appointments successfully viewed - {appointmentsResponse.UpcomingAppointments.Count()} appointments"));
        }

        [TestMethod]
        public async Task Get_ReturnsBadGateway_WhenServiceReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockAppointmentsService.Setup(x => x.GetAppointments(_userSession.GpUserSession))
                .Returns(Task.FromResult((AppointmentsResult) new AppointmentsResult.SupplierSystemUnavailable()));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockAppointmentsService.Verify();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Booked appointments view unsuccessful due to supplier being unavailable"));
        }

        [TestMethod]
        public async Task Get_ReturnsBadRequest_WhenServiceReturnsBadRequest()
        {
            // Arrange
            var badResult = new AppointmentsResult.BadRequest();
            _mockAppointmentsService.Setup(x => x.GetAppointments(_userSession.GpUserSession))
                .Returns(Task.FromResult((AppointmentsResult)badResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
            _mockAppointmentsService.Verify();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Booked appointments view unsuccessful due to bad request"));
        }

        [TestMethod]
        public async Task Get_ReturnsInternalServerError_WhenServiceReturnsInternalServerError()
        {
            // Arrange
            var badResult = new AppointmentsResult.InternalServerError();
            _mockAppointmentsService.Setup(x => x.GetAppointments(_userSession.GpUserSession))
                .Returns(Task.FromResult((AppointmentsResult)badResult));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            _mockAppointmentsService.Verify();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "Booked appointments view unsuccessful due to internal server error"));
        }

        [TestMethod]
        public async Task Get_HappyPath_VerifyAllExpectationsOnMocks()
        {
            // Arrange

            // Act
            await _systemUnderTest.Get();

            // Assert
            _mockGpSystem.VerifyAll();
            _mockGpSystemFactory.VerifyAll();
            _mockAppointmentsService.VerifyAll();
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, $"Booked appointments successfully viewed - {_appointmentsResponse.UpcomingAppointments.Count()} appointments"));
        }
    }
}
