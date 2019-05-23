using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Appointments
{
    [TestClass]
    public class MicrotestAppointmentsCancellationServiceTests
    {
        private const string AppointmentIdToBeCancelled = "182";
        private const string CancellationReasonId = "R1_NoLongerRequired";
        private const string CancellationDisplayName = "Appointment no longer required";
        private IAppointmentsService _systemUnderTest;
        private MicrotestUserSession _microtestUserSession;
        private AppointmentCancelRequest _appointmentCancelRequest;
        private Mock<IMicrotestClient> _mockMicrotestClient;
        private IFixture _fixture;
        private Mock<ICancellationReasonService> _cancellationReasonService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _appointmentCancelRequest = new AppointmentCancelRequest
            {
                AppointmentId = AppointmentIdToBeCancelled,
                CancellationReasonId = CancellationReasonId
            };
            _microtestUserSession = _fixture.Create<MicrotestUserSession>();
            _mockMicrotestClient = _fixture.Freeze<Mock<IMicrotestClient>>();
            _cancellationReasonService = _fixture.Freeze<Mock<ICancellationReasonService>>();
            _systemUnderTest = _fixture.Create<MicrotestAppointmentsService>();
        }

        [TestMethod]
        public async Task Cancel_HappyPath_ReturnsSuccessResponse()
        {
            var cancellationReason = new CancellationReason
            {
                Id = CancellationReasonId,
                DisplayName = CancellationDisplayName
            };

            _cancellationReasonService.Setup(x => x.TryGetCancellationReason(CancellationReasonId,
                    out cancellationReason))
                .Returns(true)
                .Verifiable();

            //Arrange
            var response = new MicrotestClient.MicrotestApiObjectResponse<string>(HttpStatusCode.NoContent);
            MockMicrotestClientAppointmentDeleteMethod(response);

            //Act
            var cancelRequest = await _systemUnderTest.Cancel(_microtestUserSession, _appointmentCancelRequest);

            //Assert
            _cancellationReasonService.VerifyAll();
            _mockMicrotestClient.Verify();
            cancelRequest.Should().BeAssignableTo<AppointmentCancelResult.Success>();
        }

        [TestMethod]
        public async Task Cancel_ReturnsForbidden_ReturnsForbiddenResponse()
        {
            var cancellationReason = new CancellationReason
            {
                Id = CancellationReasonId,
                DisplayName = CancellationDisplayName
            };

            _cancellationReasonService.Setup(x => x.TryGetCancellationReason(CancellationReasonId,
                    out cancellationReason))
                .Returns(true)
                .Verifiable();

            //Arrange
            var response = new MicrotestClient.MicrotestApiObjectResponse<string>(HttpStatusCode.Forbidden);
            MockMicrotestClientAppointmentDeleteMethod(response);

            //Act
            var cancelRequest = await _systemUnderTest.Cancel(_microtestUserSession, _appointmentCancelRequest);

            //Assert
            _cancellationReasonService.VerifyAll();
            _mockMicrotestClient.Verify();
            cancelRequest.Should().BeAssignableTo<AppointmentCancelResult.Forbidden>();
        }

        private void MockMicrotestClientAppointmentDeleteMethod(
            MicrotestClient.MicrotestApiObjectResponse<string> response)
        {
            _mockMicrotestClient.Setup(x => x.AppointmentsDelete(
                    _microtestUserSession.OdsCode,
                    _microtestUserSession.NhsNumber,
                    It.Is<CancelAppointmentDeleteRequest>(
                        p => p.AppointmentId.Equals(AppointmentIdToBeCancelled, StringComparison.Ordinal) &&
                             p.CancelReason.Equals(CancellationDisplayName, StringComparison.Ordinal))
                ))
                .Returns(Task.FromResult(response))
                .Verifiable();
        }
    }
}