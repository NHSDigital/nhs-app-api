using System;
using System.Globalization;
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
    public class MicrotestAppointmentsBookingServiceTests
    {
        private const string BookingReason = "I caught a cold!";
        private const string SlotId = "2862517";
        private const string TelephoneNumber = "07123456789";
        private const string MicrotestSuccessfulBookingResponse = "Appointment successfully created.";

        private IFixture _fixture;
        private Mock<IMicrotestClient> _mockMicrotestClient;
        private IAppointmentsService _systemUnderTest;
        private AppointmentBookRequest _request;
        private MicrotestUserSession _microtestUserSession;
        
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
        }

        [TestMethod]
        public async Task Book_HappyPath_ReturnsSuccessfullyBookedResponse()
        {
            // Arrange
            var response = new MicrotestClient.MicrotestApiObjectResponse<string>(HttpStatusCode.OK)
            {
                Body = MicrotestSuccessfulBookingResponse
            };

            MockMicrotestClientAppointmentPostMethod(response);
            
            // Act            
            var result = await _systemUnderTest.Book(_microtestUserSession, _request);

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SuccessfullyBooked>();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task Book_BookingReasonNotSupplied_ReturnsBadRequest(string bookingReason)
        {
            // Arrange
            _request.BookingReason = bookingReason;

            // Act
            var result = await _systemUnderTest.Book(_microtestUserSession, _request);

            // Assert
            result.Should().BeAssignableTo<AppointmentBookResult.BadRequest>();
        }

        private void MockMicrotestClientAppointmentPostMethod(
            MicrotestClient.MicrotestApiObjectResponse<string> response)
        {
            _mockMicrotestClient.Setup(x => x.BookAppointmentSlotPost(
                    It.Is<BookAppointmentSlotPostRequest>(p=>
                        p.BookingReason.Equals(BookingReason, StringComparison.Ordinal)
                        && p.SlotId == Convert.ToInt64(SlotId, CultureInfo.InvariantCulture)),
                    It.IsAny<MicrotestUserSession>()
                )
            ).Returns(
                Task.FromResult(
                    response
                )
            ).Verifiable();  
        }  
    }
}