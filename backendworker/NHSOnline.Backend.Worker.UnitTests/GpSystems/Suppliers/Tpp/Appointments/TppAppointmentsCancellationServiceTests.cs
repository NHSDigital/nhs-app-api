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
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class TppAppointmentsCancellationServiceTests
    {
        private IFixture _fixture;
        private TppUserSession _userSession;
        private Mock<ITppClient> _mockTppClient;
        private TppAppointmentsService _systemUnderTest;
        private AppointmentCancelRequest _request;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _userSession = _fixture.Create<TppUserSession>();

            _mockTppClient = _fixture.Freeze<Mock<ITppClient>>();

            _systemUnderTest = _fixture.Create<TppAppointmentsService>();

            _request = new AppointmentCancelRequest
            {
                AppointmentId = _fixture.Create<string>(),
                CancellationReasonId = null // No cancellation reasons supplied to TPP.
            };
        }

        [TestMethod]
        public async Task Cancel_HappyPath_ReturnsSuccessfullyCancelledResponse()
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<CancelAppointmentReply>(HttpStatusCode.OK)
            {
                Body = _fixture.Create<CancelAppointmentReply>(),
                ErrorResponse = null
            };

            MockTppClientAppointmentCancelMethod(response);

            // Act
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.SuccessfullyCancelled>();
        }
        
        [TestMethod]
        public async Task Cancel_TppClientThrowsHttpRequestException_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockTppClient.Setup(x => x.CancelAppointmentPost(It.IsAny<CancelAppointment>(),
                    _userSession.Suid)).
                Throws<HttpRequestException>()
                .Verifiable();

            // Act            
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.SupplierSystemUnavailable>();
        }
        
        [DataTestMethod]
        [DataRow(TppApiErrorCodes.StartDateInPast)]
        [DataRow(TppApiErrorCodes.AppointmentWithinOneHour)]
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
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.AppointmentNotCancellable>();
        }
        
        [DataTestMethod]
        [DataRow(TppApiErrorCodes.NoAccess)]
        public async Task Cancel_TppClientReturnsInsuffientPermissionsErrorCode_ReturnsInsufficientPermissions(string tppErrorCode)
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<CancelAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error{ErrorCode = tppErrorCode}
            };
            
            MockTppClientAppointmentCancelMethod(response);

            // Act            
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.InsufficientPermissions>();
        }
        
        [DataTestMethod]
        [DataRow("40376")]
        [DataRow("230405")]
        public async Task Cancel_TppClientReturnsUnanticipatedErrorCode_ReturnsSupplierSystemUnavailable(string tppErrorCode)
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<CancelAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error{ErrorCode = tppErrorCode}
            };
            
            MockTppClientAppointmentCancelMethod(response);

            // Act            
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.SupplierSystemUnavailable>();
        }

        private void MockTppClientAppointmentCancelMethod(
            TppClient.TppApiObjectResponse<CancelAppointmentReply> response)
        {
            _mockTppClient
                .Setup(
                    x => x.CancelAppointmentPost(
                        It.Is<CancelAppointment>(p => p.ApptId == _request.AppointmentId), _userSession.Suid))
                .Returns(Task.FromResult(response))
                .Verifiable();
        }
    }
}
