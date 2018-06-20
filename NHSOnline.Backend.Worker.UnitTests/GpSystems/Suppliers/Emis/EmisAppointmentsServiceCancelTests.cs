using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis
{
    [TestClass]
    public class EmisAppointmentsServiceCancelTests
    {
        private IFixture _fixture;
        private EmisUserSession _userSession;
        private Mock<IEmisClient> _mockEmisClient;
        private EmisAppointmentsService _systemUnderTest;
        private AppointmentCancelRequest _request;
        private string _cancellationReasonText;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _userSession = _fixture.Create<EmisUserSession>();

            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();

            _systemUnderTest = _fixture.Create<EmisAppointmentsService>();

            _request = new AppointmentCancelRequest
            {
                AppointmentId = _fixture.Create<int>().ToString(),
                CancellationReasonId = "R1_NoLongerRequired"
            };

            _cancellationReasonText = "No longer required";
        }

        [TestMethod]
        public async Task Cancel_HappyPath_ReturnsSuccessfullyCancelledResponse()
        {
            // Arrange
            var response = new EmisClient.EmisApiObjectResponse<CancelAppointmentDeleteResponse>(HttpStatusCode.Created)
            {
                Body = new CancelAppointmentDeleteResponse { IsCancelled = true },
                ErrorResponse = null,
                ErrorResponseBadRequest = null
            };

            MockEmisClientAppointmentCancelMethod(response);

            // Act
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.SuccessfullyCancelled>();
        }

        [TestMethod]
        public async Task Cancel_EmisClientThrowsHttpRequestExceptionFromAppointments_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockEmisClient.Setup(x => x.AppointmentsDelete(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<CancelAppointmentDeleteRequest>())).
                Throws<HttpRequestException>()
                .Verifiable();

            // Act            
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Cancel_EmisReturnsForbidden_ReturnsInsufficientPermissions()
        {
            // Arrange
            var response = new EmisClient.EmisApiObjectResponse<CancelAppointmentDeleteResponse>(HttpStatusCode
                .Forbidden);

            MockEmisClientAppointmentCancelMethod(response);

            // Act            
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.InsufficientPermissions>();
        }

        [TestMethod]
        public async Task Cancel_AppointmentIsInThePast_ReturnsNotCancellable()
        {
            // Arrange
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = EmisApiErrorMessages.AppointmentsDelete_InThePast;

            var response = new EmisClient.EmisApiObjectResponse<CancelAppointmentDeleteResponse>(HttpStatusCode
                    .InternalServerError)
                { ErrorResponse = errorResponse };

            MockEmisClientAppointmentCancelMethod(response);

            // Act            
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.AppointmentNotCancellable>();
        }

        [TestMethod]
        public async Task Cancel_AppointmentNotFound_ReturnsNotCancellable()
        {
            // Arrange
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = EmisApiErrorMessages.AppointmentsDelete_NotFound;

            var response = new EmisClient.EmisApiObjectResponse<CancelAppointmentDeleteResponse>(HttpStatusCode
                    .InternalServerError)
                { ErrorResponse = errorResponse };

            MockEmisClientAppointmentCancelMethod(response);

            // Act            
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.AppointmentNotCancellable>();
        }

        [TestMethod]
        public async Task Cancel_EmisClientReturnsConflict_ReturnsNotCancellable()
        {
            // Arrange
            var response = new EmisClient.EmisApiObjectResponse<CancelAppointmentDeleteResponse>(HttpStatusCode
                .Conflict);

            MockEmisClientAppointmentCancelMethod(response);

            // Act            
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.AppointmentNotCancellable>();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task Cancel_CancellationReasonIdNullOrEmpty_ReturnsBadRequest(string cancellationReasonId)
        {
            // Arrange
            _request.CancellationReasonId = cancellationReasonId;

            // Act
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            result.Should().BeAssignableTo<AppointmentCancelResult.BadRequest>();
        }

        [TestMethod]
        public async Task Cancel_CancellationReasonIdNotInListOfCancellationReasons_ReturnsBadRequest()
        {
            // Arrange
            _request.CancellationReasonId = _fixture.Create<string>();

            // Act
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            result.Should().BeAssignableTo<AppointmentCancelResult.BadRequest>();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("foo")]
        [DataRow("9223372036854775808")]
        public async Task Cancel_AppointmentIdNotParsableAsInt64_ReturnsBadRequest(string appointmentId)
        {
            // Arrange
            _request.AppointmentId = appointmentId;

            // Act
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            result.Should().BeAssignableTo<AppointmentCancelResult.BadRequest>();
        }

        private void MockEmisClientAppointmentCancelMethod(
            EmisClient.EmisApiObjectResponse<CancelAppointmentDeleteResponse> response)
        {
            _mockEmisClient.Setup(x => x.AppointmentsDelete(
                It.Is<EmisHeaderParameters>(p =>
                    p.EndUserSessionId == _userSession.EndUserSessionId && p.SessionId == _userSession.SessionId),
                It.Is<CancelAppointmentDeleteRequest>(p =>
                    p.CancellationReason == _cancellationReasonText
                    && p.SlotId == long.Parse(_request.AppointmentId)
                    && p.UserPatientLinkToken == _userSession.UserPatientLinkToken
                    )
                )
            ).Returns(Task.FromResult(response)).Verifiable();
        }
    }
}