using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Appointments
{
    [TestClass]
    public class EmisAppointmentsCancellationServiceTests
    {
        private IFixture _fixture;
        private EmisUserSession _emisUserSession;
        private Mock<IEmisClient> _mockEmisClient;
        private EmisAppointmentsService _systemUnderTest;
        private AppointmentCancelRequest _request;
        private string _cancellationReasonText;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _emisUserSession = _fixture.Create<EmisUserSession>();

            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();

            _systemUnderTest = _fixture.Create<EmisAppointmentsService>();

            _request = new AppointmentCancelRequest
            {
                AppointmentId = _fixture.Create<int>().ToString(CultureInfo.InvariantCulture),
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
                ExceptionErrorResponse = null,
                ErrorResponseBadRequest = null
            };

            MockEmisClientAppointmentCancelMethod(response);

            // Act
            var result = await _systemUnderTest.Cancel(_emisUserSession, _request);

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
            var result = await _systemUnderTest.Cancel(_emisUserSession, _request);

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
            var result = await _systemUnderTest.Cancel(_emisUserSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.InsufficientPermissions>();
        }

        [TestMethod]
        public async Task Cancel_AppointmentIsInThePast_ReturnsNotCancellable()
        {
            // Arrange
            var errorResponse = _fixture.Create<StandardErrorResponse>();
            errorResponse.InternalResponseCode = (int) EmisApiErrorCode.ProvidedAppointmentSlotInPast;

            var response = new EmisClient.EmisApiObjectResponse<CancelAppointmentDeleteResponse>(HttpStatusCode
                    .BadRequest)
                { StandardErrorResponse = errorResponse };

            MockEmisClientAppointmentCancelMethod(response);

            // Act
            var result = await _systemUnderTest.Cancel(_emisUserSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.AppointmentNotCancellable>();
        }


        [TestMethod]
        public async Task Cancel_AppointmentIsInThePastException_ReturnsNotCancellable()
        {
            // Arrange
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = EmisApiErrorMessages.AppointmentsDelete_InThePast;

            var response = new EmisClient.EmisApiObjectResponse<CancelAppointmentDeleteResponse>(HttpStatusCode
                    .InternalServerError)
                { ExceptionErrorResponse = errorResponse };

            MockEmisClientAppointmentCancelMethod(response);

            // Act            
            var result = await _systemUnderTest.Cancel(_emisUserSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.AppointmentNotCancellable>();
        }

        [TestMethod]
        public async Task Cancel_EmisReturnsUnknownError_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = "Unknown Error";

            var response = new EmisClient.EmisApiObjectResponse<CancelAppointmentDeleteResponse>(HttpStatusCode
                    .InternalServerError)
                { ExceptionErrorResponse = errorResponse };

            MockEmisClientAppointmentCancelMethod(response);

            // Act            
            var result = await _systemUnderTest.Cancel(_emisUserSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Cancel_EmisClientReturnsConflict_ReturnsNotCancellable()
        {
            // Arrange
            var response = new EmisClient.EmisApiObjectResponse<CancelAppointmentDeleteResponse>(HttpStatusCode
                .Conflict);

            MockEmisClientAppointmentCancelMethod(response);

            // Act            
            var result = await _systemUnderTest.Cancel(_emisUserSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.AppointmentNotCancellable>();
        }

        [TestMethod]
        public async Task Cancel_AppointmentNotFound_ReturnsNotCancellable()
        {
            // Arrange
            var errorResponse = _fixture.Create<StandardErrorResponse>();

            var response = new EmisClient.EmisApiObjectResponse<CancelAppointmentDeleteResponse>(HttpStatusCode
                    .NotFound)
                { StandardErrorResponse = errorResponse };

            MockEmisClientAppointmentCancelMethod(response);

            // Act
            var result = await _systemUnderTest.Cancel(_emisUserSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.AppointmentNotCancellable>();
        }
        
        [TestMethod]
        public async Task Cancel_AppointmentNotFoundException_ReturnsNotCancellable()
        {
            // Arrange
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = EmisApiErrorMessages.AppointmentsDelete_NotFound;

            var response = new EmisClient.EmisApiObjectResponse<CancelAppointmentDeleteResponse>(HttpStatusCode
                    .InternalServerError)
                { ExceptionErrorResponse = errorResponse };

            MockEmisClientAppointmentCancelMethod(response);

            // Act            
            var result = await _systemUnderTest.Cancel(_emisUserSession, _request);

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
            var result = await _systemUnderTest.Cancel(_emisUserSession, _request);

            // Assert
            result.Should().BeAssignableTo<AppointmentCancelResult.BadRequest>();
        }

        [TestMethod]
        public async Task Cancel_CancellationReasonIdNotInListOfCancellationReasons_ReturnsBadRequest()
        {
            // Arrange
            _request.CancellationReasonId = _fixture.Create<string>();

            // Act
            var result = await _systemUnderTest.Cancel(_emisUserSession, _request);

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
            var result = await _systemUnderTest.Cancel(_emisUserSession, _request);

            // Assert
            result.Should().BeAssignableTo<AppointmentCancelResult.BadRequest>();
        }

        private void MockEmisClientAppointmentCancelMethod(
            EmisClient.EmisApiObjectResponse<CancelAppointmentDeleteResponse> response)
        {
            _mockEmisClient.Setup(x => x.AppointmentsDelete(
                It.Is<EmisHeaderParameters>(p =>
                    p.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal)
                    && p.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal)),
                It.Is<CancelAppointmentDeleteRequest>(p =>
                    p.CancellationReason.Equals(_cancellationReasonText, StringComparison.Ordinal)
                    && p.SlotId == long.Parse(_request.AppointmentId, CultureInfo.InvariantCulture)
                    && p.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal))
                )
            ).Returns(Task.FromResult(response)).Verifiable();
        }
    }
}
