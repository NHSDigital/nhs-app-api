using AutoFixture;
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
            Mock<ILogger<VisionAppointmentsValidationService>> logger = new Mock<ILogger<VisionAppointmentsValidationService>>();
            _systemUnderTest = new VisionAppointmentsValidationService(logger.Object);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void IsDeleteValid_NoId_ReturnsFalse(string appointmentId)
        {
            //Arrange
            AppointmentCancelRequest request = new AppointmentCancelRequest()
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
            AppointmentCancelRequest request = new AppointmentCancelRequest()
            {
                AppointmentId = "1234"
            };

            //Act
            var result = _systemUnderTest.IsDeleteValid(request);

            //Assert
            result.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(null, "good reason")]
        [DataRow("1234", "<script>malicious attack</script>")]
        [DataRow("1234", "<script a='1'>malicious attack</script>")]
        public void IsPostValid_InvalidData_ReturnsFalse(string slotId, string bookingReason)
        {
            //Arrange
            AppointmentBookRequest request = new AppointmentBookRequest()
            {
                SlotId = slotId,
                BookingReason = bookingReason
            };

            //Act
            var result = _systemUnderTest.IsPostValid(request);

            //Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsPostValid_LongBookingReason_ReturnsFalse()
        {
            //Arrange
            string bookingReason = string.Join(string.Empty, generateString(151));

            AppointmentBookRequest request = new AppointmentBookRequest()
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
        public void IsPostValid_ValidData_ReturnsData()
        {
            //Arrange
            AppointmentBookRequest request = new AppointmentBookRequest()
            {
                SlotId = "1234",
                BookingReason = "important reason"
            };

            //Act
            var result = _systemUnderTest.IsPostValid(request);

            //Assert
            result.Should().BeTrue();
        }



        private string generateString(int length)
        {
            Fixture fixture = new Fixture();

            string result = string.Join(string.Empty, fixture.CreateMany<char>(length));

            return result;
        }
    }
}
