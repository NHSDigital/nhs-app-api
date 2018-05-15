using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Bridges.Emis.AppointmentSlots;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;
using Location = NHSOnline.Backend.Worker.Bridges.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis.AppointmentSlots
{
    [TestClass]
    public class AppointmentLocationMapperTests
    {
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenLocationsAreNull()
        {
            var response = new AppointmentSlotsMetadataGetResponse();

            var converter = new AppointmentLocationMapper();

            var locations = converter.Map(response);

            locations.Should().BeEquivalentTo(new Location[0]);
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenResponseHasEmptySetOfLocations()
        {
            var response = new AppointmentSlotsMetadataGetResponse { Locations = new List<Location>() };

            var converter = new AppointmentLocationMapper();

            var locations = converter.Map(response);

            locations.Should().BeEquivalentTo(new Location[0]);
        }
        
        [TestMethod]
        public void Map_ReturnsArray_WhenResponseHasSetOfLocations()
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

            var response = new AppointmentSlotsMetadataGetResponse { Locations = locationList };

            var converter = new AppointmentLocationMapper();
            var locations = converter.Map(response);

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


            var expectedLocationArray = new[]{ expectedLocation1, expectedLocation2 };

            locations.Should().BeEquivalentTo(expectedLocationArray);
        }
    }
}