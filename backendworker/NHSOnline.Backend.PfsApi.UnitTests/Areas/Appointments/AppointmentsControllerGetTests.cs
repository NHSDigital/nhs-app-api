using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Appointments
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
        private Mock<IServiceJourneyRulesService> _mockServiceJourneyRulesService;

        private const string RequestAuditType = "Appointments_ViewBooked_Request";
        private const string ResponseAuditType = "Appointments_ViewBooked_Response";

        private const string RequestAuditMessage = "Attempting to view booked appointments";
        private const string ResponseAuditMessageFormat = "Booked appointments successfully viewed - {0} upcoming appointments" + 
                                                          " and {1} historical appointments";
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

            _mockServiceJourneyRulesService = _fixture.Freeze<Mock<IServiceJourneyRulesService>>();
            _mockServiceJourneyRulesService.Setup(x => x.IsJourneyEnabled(_userSession.GpUserSession.OdsCode)).Returns(Task.FromResult(true));
            
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
            var expectedResponseAuditMessage = string.Format(CultureInfo.InvariantCulture, ResponseAuditMessageFormat,
                appointmentsResponse.UpcomingAppointments.Count(),
                appointmentsResponse.PastAppointments.Count());
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, expectedResponseAuditMessage));
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
            
            var expectedResponseAuditMessage = string.Format(CultureInfo.InvariantCulture, ResponseAuditMessageFormat,
                _appointmentsResponse.UpcomingAppointments.Count(),
                _appointmentsResponse.PastAppointments.Count());
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, expectedResponseAuditMessage));
        }
    }
}
