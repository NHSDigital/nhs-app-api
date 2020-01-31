using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.PfsApi.Areas.Appointments;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Appointments
{
    [TestClass]
    public class AppointmentSlotsLoggingVisitorTests
    {
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture();
        }

        [TestMethod]
        public void DeriveFurthestSlotDays_NoSlots_IsNull()
        {
            // Arrange
            var slots = Array.Empty<Slot>();

            // Act
            var furthestSlotDays = AppointmentSlotsLoggingVisitor.DeriveFurthestSlotDays(slots);

            // Assert
            furthestSlotDays.Should().BeNull();
        }

        [TestMethod]
        public void DeriveFurthestSlotDays_OneSlotStartingToday_IsZero()
        {
            // Arrange
            var slots = _fixture.CreateMany<Slot>(1).ToArray();
            slots.First().StartTime = DateTime.UtcNow;

            // Act
            var furthestSlotDays = AppointmentSlotsLoggingVisitor.DeriveFurthestSlotDays(slots);

            // Assert
            furthestSlotDays.Should().Be(0);
        }

        [TestMethod]
        public void DeriveFurthestSlotDays_MultipleSlots_ReturnsMaximumDays()
        {
            // Arrange
            var slots = _fixture.CreateMany<Slot>(3).ToArray();
            slots.ElementAt(0).StartTime = DateTime.UtcNow.AddDays(1);
            slots.ElementAt(1).StartTime = DateTime.UtcNow.AddDays(5);
            slots.ElementAt(2).StartTime = DateTime.UtcNow.AddDays(2);

            // Act
            var furthestSlotDays = AppointmentSlotsLoggingVisitor.DeriveFurthestSlotDays(slots);

            // Assert
            furthestSlotDays.Should().Be(5);
        }
    }
}