using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Appointments
{
    [TestClass]
    public class CancellationReasonMapperTests
    {
        private CancellationReasonMapper _systemUnderTest;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new CancellationReasonMapper();
        }

        [TestMethod]
        public void Map_HappyPath_ReturnsAnArrayOfCancellationReasons()
        {
            var descUK = new Description
            {
                Language = "en_UK",
                Text = "Reason no. 1 UK"
            };
            
            var descPL = new Description
            {
                Language = "pl_PL",
                Text = "Powod nr 1 PL"
            };

            var reason1 = new Reason
            {
                Id = "1",
                Descriptions = new[] { descUK, descPL }.ToList()
            };
            
            var reason2 = new Reason
            {
                Id = "2",
                Descriptions = new[] { descPL }.ToList()
            };

            var settings = new SlotSettings
            {
                CancellationReasons = new[] { reason1, reason2 }.ToList()
            };

            var appointmentsResponses = new BookedAppointmentsResponse { Appointments = new BookedAppointments{Settings = settings} };

            var actualResponse = _systemUnderTest.Map(appointmentsResponses);

            var expectedResponse = new[]
            {
                new CancellationReason
                {
                    Id = "1",
                    DisplayName = "Reason no. 1 UK"
                },
                new CancellationReason
                {
                    Id = "2",
                    DisplayName = ""
                }
            };

        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenReasonsInResponseIsEmpty()
        {
            // Arrange
            var settings = new SlotSettings
            {
                CancellationReasons = new List<Reason>()
            };
            
            var bookedAppointmentsResponse = new BookedAppointmentsResponse { Appointments = new BookedAppointments{Settings = settings} };

            // Act
            var actualResponse = _systemUnderTest.Map(bookedAppointmentsResponse);

            // Assert
            actualResponse.Should().BeEmpty();
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSlotsInResponseIsNull()
        {
            // Act
            var actualResponse = _systemUnderTest.Map(null);

            // Assert
            actualResponse.Should().BeEmpty();
        }
    }
}