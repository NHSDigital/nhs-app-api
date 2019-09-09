using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class TppAppointmentsValidationServiceTests
    {
        private TppAppointmentsValidationService _systemUnderTest;
        private Fixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
            var logger = new Mock<ILogger<TppAppointmentsValidationService>>();
            _systemUnderTest = new TppAppointmentsValidationService(logger.Object);
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
        [DataRow("", null)]
        [DataRow("1234", "<script>malicious attack</script>")]
        public void IsPostValid_InvalidData_ReturnsFalse(string slotId, string bookingReason)
        {
            // Arrange
            var request = new AppointmentBookRequest()
            {
                SlotId = slotId,
                BookingReason = bookingReason,
                EndTime = DateTime.Now,
                StartTime = DateTime.Now
            };

            // Act
            var result = _systemUnderTest.IsPostValid(request);

            // Assert
            result.Should().BeFalse();
        }

        [DataTestMethod]
        public void IsPostValid_NoDates_ReturnsFalse()
        {
            // Arrange
            var request = new AppointmentBookRequest
            {
                SlotId = "1234"
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
            var bookingReason = _fixture.CreateStringOfLength(151);

            var request = new AppointmentBookRequest
            {
                SlotId = "1234",
                BookingReason = bookingReason,
                EndTime = DateTime.Now,
                StartTime = DateTime.Now
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
                BookingReason = "important reason",
                EndTime = DateTime.Now,
                StartTime = DateTime.Now
            };

            // Act
            var result = _systemUnderTest.IsPostValid(request);

            // Assert
            result.Should().BeTrue();
        }
    }
}
