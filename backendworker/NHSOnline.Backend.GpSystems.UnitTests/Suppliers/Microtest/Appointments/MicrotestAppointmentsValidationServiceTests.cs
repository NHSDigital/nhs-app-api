using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Appointments
{
    [TestClass]
    public class MicrotestAppointmentsValidationServiceTests
    {
        private MicrotestAppointmentsValidationService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            var logger = new Mock<ILogger<MicrotestAppointmentsValidationService>>();
            _systemUnderTest = new MicrotestAppointmentsValidationService(logger.Object);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void IsDeleteValid_NoId_ReturnsFalse(string appointmentId)
        {
            //Arrange
            var request = new AppointmentCancelRequest()
            {
                AppointmentId = appointmentId,
                CancellationReasonId = "1234"
            };

            //Act
            var result = _systemUnderTest.IsDeleteValid(request);

            //Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsDeleteValid_ValidId_ReturnsTrue()
        {
            //Arrange
            var request = new AppointmentCancelRequest()
            {
                AppointmentId = "1234"
            };

            //Act
            var result = _systemUnderTest.IsDeleteValid(request);

            //Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsPostValid_LongBookingReason_ReturnsFalse()
        {
            //Arrange
            var bookingReason = string.Join(string.Empty, GenerateString(151));

            var request = new AppointmentBookRequest()
            {
                SlotId = "1234",
                BookingReason = bookingReason
            };

            //Act
            var result = _systemUnderTest.IsPostValid(request);

            //Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsPostValid_ValidData_ReturnsTrue()
        {
            //Arrange
            var request = new AppointmentBookRequest()
            {
                SlotId = "1234",
                BookingReason = "important reason"
            };

            //Act
            var result = _systemUnderTest.IsPostValid(request);

            //Assert
            result.Should().BeTrue();
        }

        private string GenerateString(int length)
        {
            var fixture = new Fixture();

            var result = string.Join(string.Empty, fixture.CreateMany<char>(length));

            return result;
        }
    }
}
