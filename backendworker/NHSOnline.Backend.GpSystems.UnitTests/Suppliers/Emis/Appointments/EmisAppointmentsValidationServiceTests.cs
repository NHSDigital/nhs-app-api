using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Appointments
{
    [TestClass]
    public class EmisAppointmentsValidationServiceTests
    {
        private EmisAppointmentsValidationService _systemUnderTest;
        private Fixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            var logger = new Mock<ILogger<EmisAppointmentsValidationService>>();
            _systemUnderTest = new EmisAppointmentsValidationService(logger.Object);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void IsDeleteValid_NoId_ReturnsFalse(string appointmentId)
        {
            // Arrange
            var request = new AppointmentCancelRequest
            {
                AppointmentId = appointmentId,
                CancellationReasonId = "1234"
            };

            // Act
            var result = _systemUnderTest.IsDeleteValid(request);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsDeleteValid_ValidId_ReturnsTrue()
        {
            // Arrange
            var request = new AppointmentCancelRequest
            {
                AppointmentId = "1234"
            };

            // Act
            var result = _systemUnderTest.IsDeleteValid(request);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsPostValid_LongBookingReason_ReturnsFalse()
        {
            // Arrange
            var bookingReason = _fixture.CreateStringOfLength(151);

            var request = new AppointmentBookRequest
            {
                SlotId = "1234",
                BookingReason = bookingReason
            };

            // Act
            var result = _systemUnderTest.IsPostValid(request);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsPostValid_ValidData_ReturnsTrue()
        {
            // Arrange
            var request = new AppointmentBookRequest
            {
                SlotId = "1234",
                BookingReason = "important reason"
            };

            // Act
            var result = _systemUnderTest.IsPostValid(request);

            // Assert
            result.Should().BeTrue();
        }
    }
}
