using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentAssertions.Execution;
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
        private Mock<IErrorReferenceGenerator> _mockErrorReferenceGenerator;
        private string _serviceDeskReference;

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
            
            _mockErrorReferenceGenerator = _fixture.Freeze<Mock<IErrorReferenceGenerator>>();
            _serviceDeskReference = _fixture.Create<string>();

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
        public async Task Delete_RequestIsInvalid_ReturnsObjectResultWith400StatusCode()
        {
            // Arrange
            _mockAppointmentsValidationService.Setup(x => x.IsDeleteValid(_appointmentCancelRequest))
                .Returns(false);
            _mockErrorReferenceGenerator.Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Appointments, 
                    StatusCodes.Status400BadRequest, _userSession.GpUserSession.Supplier))
                .Returns(_serviceDeskReference);

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            _mockAppointmentsService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, "Unable to cancel appointment due to a bad request for appointment with id: {0}",
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
            _mockAppointmentsService.Setup(x => x.Cancel(_userSession.GpUserSession, _appointmentCancelRequest))
                .Returns(Task.FromResult(serviceResult));
            _mockErrorReferenceGenerator.Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Appointments, 
                    expectedStatusCode, _userSession.GpUserSession.Supplier))
                .Returns(_serviceDeskReference);

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.Delete(_appointmentCancelRequest);

            // Assert
            _mockAppointmentsService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(expectedStatusCode);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }
            _mockAuditor.Verify(x => x.Audit(RequestAuditType, RequestAuditMessage));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType, expectedAuditResponseMessageFormat,
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

