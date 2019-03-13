using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using Slot = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments.Slot;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Appointments
{
    [TestClass]
    public class AppointmentSlotsMapperTests
    {
        private IFixture _fixture;
        private IAppointmentSlotsResponseMapper _systemUnderTest;
        private IEnumerable<Slot> _microtestSlots;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            var mapper = _fixture.Create<IMicrotestEnumMapper>();

            var logger = _fixture.Create<ILogger<AppointmentSlotsResponseMapper>>();

            _systemUnderTest = new AppointmentSlotsResponseMapper(mapper, logger);

            _microtestSlots = _fixture.CreateMany<Slot>();
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenEmptyCollectionIsPassed()
        {
            // Arrange
            var appointmentSlotsGetResponse = new AppointmentSlotsGetResponse
            {
                Slots = Enumerable.Empty<Slot>()
            };

            // Act
            var actualResponse = _systemUnderTest.Map(appointmentSlotsGetResponse);

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.Slots.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_ProvidedValidAppointSlots_Maps()
        {
            // Arrange
            var appointmentSlotsGetResponse = new AppointmentSlotsGetResponse
            {
                Slots = _microtestSlots
            };

            // Act
            var actualResponse = _systemUnderTest.Map(appointmentSlotsGetResponse);

            // Assert
            actualResponse.Should().NotBeNull();
            foreach (var slot in actualResponse.Slots)
            {
                var microtestSlot = _microtestSlots.First(s => s.Id.Equals(slot.Id, StringComparison.Ordinal));
                slot.Should().BeEquivalentTo(microtestSlot, options => options.Excluding( s => s.Channel));
                slot.Channel.Should().Be(Channel.Unknown);
            }
        }

        [TestMethod]
        public void Map_DoesNotMapAppointmentSlot_WhenStartTimeIsNull()
        {
            // Arrange
            var invalidSlot = _microtestSlots.First();

            invalidSlot.StartTime = null;

            var appointmentSlotsGetResponse = new AppointmentSlotsGetResponse
            {
                Slots = _microtestSlots
            };

            // Act
            var actualResponse = _systemUnderTest.Map(appointmentSlotsGetResponse);

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.Slots.Should().NotContain(slot => slot.Id.Equals(invalidSlot.Id, StringComparison.Ordinal));
            actualResponse.Slots.Count().Should().Be(_microtestSlots.Count() - 1);
        }
    }
}
