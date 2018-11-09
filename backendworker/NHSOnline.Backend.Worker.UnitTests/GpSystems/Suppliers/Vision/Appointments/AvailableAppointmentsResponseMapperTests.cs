using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Appointments
{
    [TestClass]
    public class AvailableAppointmentsResponseMapperTests
    {
        private IFixture _fixture;
        private AvailableAppointmentsResponseMapper _systemUnderTest;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<AvailableAppointmentsResponseMapper>();
        }
        
        [TestMethod]
        public void Map_ReturnsAppointmentSlots()
        {
            // Arrange
            var visionResponse = _fixture.Create<AvailableAppointmentsResponse>();

            // Act
            var response = _systemUnderTest.Map(visionResponse);

            // Assert
            var expectedAppointments = new List<FreeSlot>();

            response.Slots.Should().BeEquivalentTo(expectedAppointments);
        }
    }
}