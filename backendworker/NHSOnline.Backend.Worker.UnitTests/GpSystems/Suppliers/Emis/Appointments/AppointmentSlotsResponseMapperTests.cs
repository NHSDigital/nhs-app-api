using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Date;
using Location = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Appointments
{
    [TestClass]
    public class AppointmentSlotsResponseMapperTests
    {
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        private IAppointmentSlotsResponseMapper _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _timeZoneInfoProvider = new TimeZoneInfoProvider();
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
            _sut = new AppointmentSlotsResponseMapper(
                new AppointmentSlotsMapper(_dateTimeOffsetProvider));
        }
        
        [TestMethod]
        public void Map_WhenSessionInAppointmentsSlotsResponseIsNull_ReturnsEmptySetOfSlots()
        {
            // Arrange
            var slotsResponse = new AppointmentSlotsGetResponse();
            
            var location = CreateLocation(23, "Lees");
            var session = CreateSession(location.LocationId, 1, "General Appointment Session", "Timed");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Sessions = new[] { session }
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = new Slot[0]
            };

            // Act
            var actualResponse = _sut.Map(slotsResponse, slotsMetadataResponse);
            
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Map_WhenSessionInAppointmentsSlotsResponseIsEmpty_ReturnsEmptySetOfSlots()
        {
            // Arrange
            var slotsResponse = new AppointmentSlotsGetResponse
            {
                Sessions = new List<AppointmentSlotSession>()
            };
            
            var location = CreateLocation(23, "Lees");
            var session = CreateSession(location.LocationId, 1, "General Appointment Session", "Timed");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Sessions = new[] { session }
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = new Slot[0]
            };

            // Act
            var actualResponse = _sut.Map(slotsResponse, slotsMetadataResponse);
            
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Map_WhenSessionInAppointmentsSlotsMetadataResponseIsNull_ReturnsEmptySetOfSlots()
        {
            // Arrange
            var location = CreateLocation(14, "Leeds");
            var sessionHolder = CreateSessionHolder(34, "Dr Who");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Locations = new[] { location },
                SessionHolders = new[] { sessionHolder }
                
            };

            var appointmentSlotSession =
                CreateAppointmentsSlotSession(1, 2, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");

            var slotsResponse = new AppointmentSlotsGetResponse
            {
                Sessions = new[] { appointmentSlotSession }
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = new Slot[0]
            };
            // Act
            var actualResponse = _sut.Map(slotsResponse, slotsMetadataResponse);
            
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Map_WhenSessionInAppointmentsSlotsMetadataResponseIsEmpty_ReturnsEmptySetOfSlots()
        {
            // Arrange
            var location = CreateLocation(14, "Leeds");
            var sessionHolder = CreateSessionHolder(34, "Dr Who");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Sessions = new Session[0],
                Locations = new[] { location },
                SessionHolders = new[] { sessionHolder }
                
            };

            var appointmentSlotSession =
                CreateAppointmentsSlotSession(1, 2, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");

            var slotsResponse = new AppointmentSlotsGetResponse
            {
                Sessions = new[] { appointmentSlotSession }
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = new Slot[0]
            };

            // Act
            var actualResponse = _sut.Map(slotsResponse, slotsMetadataResponse);
            
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Map_WhenNoLocationsAreAvailable_ReturnsEmptySetOfLocations()
        {
            // Arrange
            var sessionHolder = CreateSessionHolder(34, "Dr Who");
            var session = new Session
            {
                SessionId = 77,
                SessionName = "General Session Appointment",
                SessionType = "Timed"
            };
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Sessions = new[]{ session },
                Locations = new Location[0],
                SessionHolders = new[] { sessionHolder }
            };

            var appointmentSlotSession =
                CreateAppointmentsSlotSession(1, 77, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");

            var slotsResponse = new AppointmentSlotsGetResponse
            {
                Sessions = new[] { appointmentSlotSession }
            };

            var expectedSlot = new Slot
            {
                Id = "1",
                Clinicians = new string[0],
                Location = "",
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19").ToUniversalTime(),
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19").ToUniversalTime(),
                Type = "General Session Appointment - Emergency"
            };
            
            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = new[] { expectedSlot }
            };

            // Act
            var actualResponse = _sut.Map(slotsResponse, slotsMetadataResponse);
            
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Map_WhenNoCliniciansAreAvailable_ReturnsEmptySetOfClinicians()
        {
            // Arrange
            var session = new Session
            {
                SessionId = 77,
                SessionType = "Timed",
                SessionName = "General Session Appointment"
            };
            var location = CreateLocation(365, "Leeds");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Sessions = new[]{ session },
                Locations = new[] { location },
                SessionHolders = new SessionHolder[0],
            };

            var appointmentSlotSession =
                CreateAppointmentsSlotSession(1, 77, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");

            var slotsResponse = new AppointmentSlotsGetResponse
            {
                Sessions = new[] { appointmentSlotSession }
            };

            var expectedSlot = new Slot
            {
                Id = "1",
                Clinicians = new string[0],
                Location = "",
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19").ToUniversalTime(),
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19").ToUniversalTime(),
                Type = "General Session Appointment - Emergency"
            };
            
            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = new[] { expectedSlot }
            };

            // Act
            var actualResponse = _sut.Map(slotsResponse, slotsMetadataResponse);
            
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        private static AppointmentSlotSession CreateAppointmentsSlotSession(int slotId, int sessionId, string startTime, string endTime, string slotTypeName)
        {
            var appointmentSlot = new AppointmentSlot
            {
                SlotId = slotId,
                EndTime = endTime,
                StartTime = startTime,
                SlotTypeName = slotTypeName
            };
            
            var appointmentSlotSession = new AppointmentSlotSession
            {
                SessionId = sessionId,
                Slots = new[]{ appointmentSlot }
            };

            return appointmentSlotSession;
        }

        private static Location CreateLocation(int id, string name)
        {
            return new Location
            {
                LocationId = id,
                LocationName = name
            };
        }

        private static SessionHolder CreateSessionHolder(int id, string name)
        {
            return new SessionHolder
            {
                ClinicianId = id,
                DisplayName = name
            };
        }

        private static Session CreateSession(int locationId, int sessionId, string name, string sessionType)
        {
            return new Session
            {
                LocationId = locationId,
                SessionId = sessionId,
                SessionName = name,
                SessionType = sessionType
            };
        }
    }
}
