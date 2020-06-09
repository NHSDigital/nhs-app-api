using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Appointments
{
    [TestClass]
    public class VisionAppointmentsValidationServiceTests
    {
        private VisionAppointmentsValidationService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            var logger = new Mock<ILogger<VisionAppointmentsValidationService>>();
            _systemUnderTest = new VisionAppointmentsValidationService(logger.Object);
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

        [DataTestMethod]
        [DataRow(null, "good reason")]
        [DataRow("1234", "<script>malicious attack</script>")]
        [DataRow("1234", "<script a='1'>malicious attack</script>")]
        public void IsPostValid_InvalidData_ReturnsFalse(string slotId, string bookingReason)
        {
            // Arrange
            var request = new AppointmentBookRequest
            {
                SlotId = slotId,
                BookingReason = bookingReason
            };

            // Act
            var result = _systemUnderTest.IsPostValid(request);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsPostValid_LongBookingReason_ReturnsFalse()
        {
            // Arrange
            var bookingReason = new string('a', 151);

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
        public void IsPostValid_ValidData_ReturnsData()
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
