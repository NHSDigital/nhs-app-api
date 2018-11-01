using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Models.Appointments;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Microtest.Appointments
{
    [TestClass]
    public class AppointmentSlotsMapperTests
    {
        private IFixture _fixture;
        private IAppointmentSlotsResponseMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            var logger = _fixture.Create<ILoggerFactory>().CreateLogger<AppointmentSlotsResponseMapper>();            

            _systemUnderTest = new AppointmentSlotsResponseMapper();            
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenEmptyCollectionIsPassed()
        {
            // Arrange
            var appointmentSlotsGetResponse = new AppointmentSlotsGetResponse
            {
                Slots = Enumerable.Empty<Worker.GpSystems.Suppliers.Microtest.Models.Appointments.Slot>(),
            };

            // Act
            var actualResponse = _systemUnderTest.Map(appointmentSlotsGetResponse);

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.Slots.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_DoesntMapAppointmentSlot_WhenStartTimeIsNull()
        {
            // Arrange
            var appointmentSlotsGetResponse = new AppointmentSlotsGetResponse
            {
                Slots = new[]
                {
                    new Worker.GpSystems.Suppliers.Microtest.Models.Appointments.Slot
                    {
                        Id = "242",
                        Clinicians = new List<string> { "Dr Spears" },
                        Type = "Emergency",
                        StartTime = null,
                        EndTime = new DateTime(2000, 1, 2),
                        Duration = "15 min",
                        Location = "Room 1",
                    },
                },
            };

            // Act
            var actualResponse = _systemUnderTest.Map(appointmentSlotsGetResponse);

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.Slots.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSessionsInAppointmentsSlotsResponseIsNull()
        {
            // Arrange
            var appointmentSlotsGetResponse = new AppointmentSlotsGetResponse
            {
                Slots = new List<Worker.GpSystems.Suppliers.Microtest.Models.Appointments.Slot>
                {
                    new Worker.GpSystems.Suppliers.Microtest.Models.Appointments.Slot
                    {
                        Id = "132",
                        Clinicians = new List<string> { "Dr Spears" },
                        Type = "Emergency",
                        StartTime = new DateTime(2000, 1, 1),
                        EndTime = new DateTime(2000, 1, 2),
                        Duration = "15 min",
                        Location = "Room 1",
                    }
                },
            };

            // Act
            var actualResponse = _systemUnderTest.Map(appointmentSlotsGetResponse);

            // Assert
            var expectedSlot = new Worker.Areas.Appointments.Models.Slot
            {
                Id = "132",
                Clinicians = new[] { "Dr Spears" },
                StartTime = new DateTime(2000, 1, 1),
                EndTime = new DateTime(2000, 1, 2),
                Type = "Emergency",
                Location = "Room 1",
            };
            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = new[] { expectedSlot },
            };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
