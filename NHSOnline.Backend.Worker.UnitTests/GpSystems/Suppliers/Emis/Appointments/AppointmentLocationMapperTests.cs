using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using Location = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Appointments
{
    [TestClass]
    public class AppointmentLocationMapperTests
    {
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenLocationsAreNull()
        {
            var systemUnderTest = new AppointmentLocationMapper();

            var locations = systemUnderTest.Map((IEnumerable<Location>)null);

            locations.Should().BeEmpty();
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenGivenEmptySetOfLocations()
        {
            var systemUnderTest = new AppointmentLocationMapper();

            var locations = systemUnderTest.Map(new List<Location>());

            locations.Should().BeEmpty();
        }
        
        [TestMethod]
        public void Map_ReturnsExpectedResults_WhenResponseHasSetOfLocations()
        {
            var location1 = new Location
            {
                LocationId = 1,
                LocationName = "Leeds"
            };

            var location2 = new Location
            {
                LocationId = 2,
                LocationName = "London"
            };

            var locationList = new List<Location> { location1, location2 };

            var systemUnderTest = new AppointmentLocationMapper();
            var locations = systemUnderTest.Map(locationList);

            var expectedLocation1 = new Worker.Areas.Appointments.Models.Location
            {
                Id = "1",
                DisplayName = "Leeds"
            };
            
            var expectedLocation2 = new Worker.Areas.Appointments.Models.Location
            {
                Id = "2",
                DisplayName = "London"
            };

            var expectedLocationResults  =  new[]{ expectedLocation1, expectedLocation2 };

            locations.Should().BeEquivalentTo(expectedLocationResults);
        }
    }
}