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
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Appointments
{
    [TestClass]
    public class MicrotestAppointmentsBookingServiceTests
    {
        private const string BookingReason = "I caught a cold!";
        private const string SlotId = "2862517";
        private const string TelephoneNumber = "07123456789";
        private const string MicrotestSuccessfulBookingResponse = "Appointment successfully created.";
        private const string MicrotestForbiddenBookingResponse = "{\n\t \"Error\" : \"The patient does not have the " +
                                                                 "necessary permissions within the GP system. " +
                                                                 "(appointments)\"\n}";
        private const string MicrotestNotAvailableBookingResponse = "{\n\t \"Error\" : \"Conflict. The chosen appointment " +
                                                                    "slot is not available for booking.\"\n}";

        private IFixture _fixture;
        private Mock<IMicrotestClient> _mockMicrotestClient;
        private IAppointmentsService _systemUnderTest;
        private AppointmentBookRequest _request;
        private MicrotestUserSession _microtestUserSession;
        private GpLinkedAccountModel _gpLinkedAccountModel;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _microtestUserSession = _fixture.Create<MicrotestUserSession>();
            
            _mockMicrotestClient = _fixture.Freeze<Mock<IMicrotestClient>>();

            _systemUnderTest = _fixture.Create<MicrotestAppointmentsService>();
            
            _request = new AppointmentBookRequest
            {
                BookingReason = BookingReason,
                SlotId = SlotId,
                TelephoneNumber = TelephoneNumber
            };
            
            _gpLinkedAccountModel = new GpLinkedAccountModel(_microtestUserSession, Guid.NewGuid());
        }

        [TestMethod]
        public async Task Book_HappyPath_ReturnsSuccessResponse()
        {
            // Arrange
            var response = new MicrotestClient.MicrotestApiObjectResponse<string>(HttpStatusCode.OK)
            {
                Body = MicrotestSuccessfulBookingResponse
            };

            MockMicrotestClientAppointmentPostMethod(response);
            
            // Act            
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.Success>();
        }

        [TestMethod]
        public async Task Book_ReturnsForbidden_ReturnsForbiddenResponse()
        {
            // Arrange
            var response = new MicrotestClient.MicrotestApiObjectResponse<string>(HttpStatusCode.Forbidden)
            {
                Body = MicrotestForbiddenBookingResponse
            };

            MockMicrotestClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.Forbidden>();
        }

        [TestMethod]
        public async Task Book_ReturnsConflict_ReturnsSlotNotAvailableResponse()
        {
            // Arrange
            var response = new MicrotestClient.MicrotestApiObjectResponse<string>(HttpStatusCode.Conflict)
            {
                Body = MicrotestNotAvailableBookingResponse
            };

            MockMicrotestClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task Book_BookingReasonNotSupplied_ReturnsBadRequest(string bookingReason)
        {
            // Arrange
            _request.BookingReason = bookingReason;

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            result.Should().BeAssignableTo<AppointmentBookResult.BadRequest>();
        }

        private void MockMicrotestClientAppointmentPostMethod(
            MicrotestClient.MicrotestApiObjectResponse<string> response)
        {
            _mockMicrotestClient.Setup(x => x.AppointmentsPost(
                    ((MicrotestUserSession)_gpLinkedAccountModel.GpUserSession).OdsCode,
                    ((MicrotestUserSession)_gpLinkedAccountModel.GpUserSession).NhsNumber,
                    It.Is<BookAppointmentSlotPostRequest>(p=> 
                        p.BookingReason.Equals(BookingReason, StringComparison.Ordinal)
                        && p.SlotId.Equals(SlotId, StringComparison.Ordinal))
                )
            ).Returns(
                Task.FromResult(
                    response
                )
            ).Verifiable();  
        }  
    }
}