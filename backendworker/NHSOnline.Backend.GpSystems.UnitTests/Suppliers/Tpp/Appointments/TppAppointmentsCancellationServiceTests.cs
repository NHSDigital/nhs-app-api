using System;
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
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class TppAppointmentsCancellationServiceTests
    {
        private IFixture _fixture;
        private TppUserSession _tppUserSession;
        private Mock<ITppClient> _mockTppClient;
        private TppAppointmentsService _systemUnderTest;
        private AppointmentCancelRequest _request;
        private GpLinkedAccountModel _gpLinkedAccountModel;
        private Guid _patientId;

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _tppUserSession = _fixture.Create<TppUserSession>();
            _gpLinkedAccountModel = new GpLinkedAccountModel(_tppUserSession, _patientId);

            _mockTppClient = _fixture.Freeze<Mock<ITppClient>>();

            _systemUnderTest = _fixture.Create<TppAppointmentsService>();

            _request = new AppointmentCancelRequest
            {
                AppointmentId = _fixture.Create<string>(),
                CancellationReasonId = null // No cancellation reasons supplied to TPP.
            };
        }

        [TestMethod]
        public async Task Cancel_HappyPath_ReturnsSuccessResponse()
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<CancelAppointmentReply>(HttpStatusCode.OK)
            {
                Body = _fixture.Create<CancelAppointmentReply>(),
                ErrorResponse = null
            };

            MockTppClientAppointmentCancelMethod(response);

            // Act
            var result = await _systemUnderTest.Cancel(_gpLinkedAccountModel, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.Success>();
        }
        
        [TestMethod]
        public async Task Cancel_TppClientThrowsHttpRequestException_ReturnsBadGateway()
        {
            // Arrange
            _mockTppClient.Setup(x => x.CancelAppointmentPost(It.IsAny<CancelAppointment>(),
                    _tppUserSession.Suid)).
                Throws<HttpRequestException>()
                .Verifiable();

            // Act            
            var result = await _systemUnderTest.Cancel(_gpLinkedAccountModel, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.BadGateway>();
        }
        
        [DataTestMethod]
        [DataRow(TppApiErrorCodes.StartDateInPast)]
        public async Task Cancel_TppClientReturnsNotCancellableErrorCode_ReturnsAppointmentNotCancellable(string tppErrorCode)
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<CancelAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error{ErrorCode = tppErrorCode}
            };
            
            MockTppClientAppointmentCancelMethod(response);

            // Act            
            var result = await _systemUnderTest.Cancel(_gpLinkedAccountModel, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.AppointmentNotCancellable>();
        }
        
        [DataTestMethod]
        [DataRow(TppApiErrorCodes.AppointmentWithinOneHour)]
        public async Task Cancel_TppClientReturnsTooLateToCancelErrorCode_ReturnsTooLateToCancel(string tppErrorCode)
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<CancelAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error{ErrorCode = tppErrorCode}
            };
            
            MockTppClientAppointmentCancelMethod(response);

            // Act            
            var result = await _systemUnderTest.Cancel(_gpLinkedAccountModel, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.TooLateToCancel>();
        }
        
        [DataTestMethod]
        [DataRow(TppApiErrorCodes.NoAccess)]
        public async Task Cancel_TppClientReturnsInsufficientPermissionsErrorCode_ReturnsForbidden(string tppErrorCode)
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<CancelAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error{ErrorCode = tppErrorCode}
            };
            
            MockTppClientAppointmentCancelMethod(response);

            // Act            
            var result = await _systemUnderTest.Cancel(_gpLinkedAccountModel, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.Forbidden>();
        }
        
        [DataTestMethod]
        [DataRow("40376")]
        [DataRow("230405")]
        public async Task Cancel_TppClientReturnsUnanticipatedErrorCode_ReturnsBadGateway(string tppErrorCode)
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<CancelAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error{ErrorCode = tppErrorCode}
            };
            
            MockTppClientAppointmentCancelMethod(response);

            // Act            
            var result = await _systemUnderTest.Cancel(_gpLinkedAccountModel, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.BadGateway>();
        }

        private void MockTppClientAppointmentCancelMethod(
            TppClient.TppApiObjectResponse<CancelAppointmentReply> response)
        {
            _mockTppClient
                .Setup(
                    x => x.CancelAppointmentPost(
                        It.Is<CancelAppointment>(p => p.ApptId.Equals(_request.AppointmentId, StringComparison.Ordinal)),
                        _tppUserSession.Suid))
                .Returns(Task.FromResult(response))
                .Verifiable();
        }
    }
}
