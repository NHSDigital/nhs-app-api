using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.SharedModels;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Appointments
{
    [TestClass]
    public class AvailableAppointmentsResponseMapperTests
    {
        private IFixture _fixture;
        private AvailableAppointmentsResponseMapper _systemUnderTest;
        private VisionUserSession _userSession;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<AvailableAppointmentsResponseMapper>();
            _userSession = _fixture.Create<VisionUserSession>();
        }
        
        [TestMethod]
        public void Map_ReturnsAppointmentSlots()
        {
            // Arrange
            var visionResponse = _fixture.Create<AvailableAppointmentsResponse>();

            // Act
            var response = _systemUnderTest.Map(visionResponse, _userSession);

            // Assert
            var expectedAppointments = new List<FreeSlot>();

            response.Slots.Should().BeEquivalentTo(expectedAppointments);
        }

        [DataTestMethod]
        [DataRow(Necessity.Optional)]
        [DataRow(Necessity.NotAllowed)]
        public void Map_SetsBookingReasonNecessity(Necessity expectedNecessity)
        {
            // Arrange
            var visionResponse = _fixture.Create<AvailableAppointmentsResponse>();
            _userSession.AppointmentBookingReasonNecessity = expectedNecessity;

            // Act
            var response = _systemUnderTest.Map(visionResponse, _userSession);

            // Assert
            response.BookingReasonNecessity.Should().Be(expectedNecessity);
        }
    }
}