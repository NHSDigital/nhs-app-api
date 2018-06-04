using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.AppointmentSlots;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;
using NHSOnline.Backend.Worker.Support.Date;
using Location = NHSOnline.Backend.Worker.Bridges.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis.AppointmentSlots
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
            _sut = new AppointmentSlotsResponseMapper(_dateTimeOffsetProvider);
        }
        
        [TestMethod]
        public void Map_WhenSessionInAppointmentsSlotsResponseIsNull_ReturnsEmptySetOfSlots()
        {
            // Arrange
            var slotsResponse = new AppointmentsSlotsGetResponse();
            
            var location = CreateLocation(23, "Lees");
            var session = CreateSession(location.LocationId, 1, "General Appointment Session", "Timed");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Sessions = new[] { session }
            };

            var expectedAppointmentSession = new AppointmentSession
            {
                DisplayName = "Timed",
                Id ="1"
            };
            
            var expectedResponse = new AppointmentSlotsResponse
            {
                Clinicians = new Clinician[0],
                AppointmentSessions = new[] { expectedAppointmentSession },
                Locations = new Worker.Areas.Appointments.Models.Location[0],
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
            var slotsResponse = new AppointmentsSlotsGetResponse
            {
                Sessions = new List<AppointmentSlotSession>()
            };
            
            var location = CreateLocation(23, "Lees");
            var session = CreateSession(location.LocationId, 1, "General Appointment Session", "Timed");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Sessions = new[] { session }
            };

            var expectedAppointmentSession = new AppointmentSession
            {
                DisplayName = "Timed",
                Id = Convert.ToString(session.SessionId)
            };
            
            var expectedResponse = new AppointmentSlotsResponse
            {
                Clinicians = new Clinician[0],
                AppointmentSessions = new[] { expectedAppointmentSession },
                Locations = new Worker.Areas.Appointments.Models.Location[0],
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
                CreateAppointmentsSlotSession(1, 2, "2018-05-09T10:59:19", "2018-05-09T10:59:19");

            var slotsResponse = new AppointmentsSlotsGetResponse
            {
                Sessions = new[] { appointmentSlotSession }
            };

            var expectedClinician = new Clinician
            {
                Id = "34",
                DisplayName = "Dr Who"
            };
            var expectedLocation = new Worker.Areas.Appointments.Models.Location
            {
                Id = "14",
                DisplayName = "Leeds"
            };
            
            var expectedResponse = new AppointmentSlotsResponse
            {
                Clinicians = new[] { expectedClinician },
                AppointmentSessions = new AppointmentSession[0],
                Locations = new[] { expectedLocation },
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
                Sessions = new Worker.Bridges.Emis.Models.Session[0],
                Locations = new[] { location },
                SessionHolders = new[] { sessionHolder }
                
            };

            var appointmentSlotSession =
                CreateAppointmentsSlotSession(1, 2, "2018-05-09T10:59:19", "2018-05-09T10:59:19");

            var slotsResponse = new AppointmentsSlotsGetResponse
            {
                Sessions = new[] { appointmentSlotSession }
            };

            var expectedClinician = new Clinician
            {
                Id = "34",
                DisplayName = "Dr Who"
            };
            
            var expectedLocation = new Worker.Areas.Appointments.Models.Location
            {
                Id = "14",
                DisplayName = "Leeds"
            };
            
            var expectedResponse = new AppointmentSlotsResponse
            {
                Clinicians = new[] { expectedClinician },
                AppointmentSessions = new AppointmentSession[0],
                Locations = new[] { expectedLocation },
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
                CreateAppointmentsSlotSession(1, 2, "2018-05-09T10:59:19", "2018-05-09T10:59:19");

            var slotsResponse = new AppointmentsSlotsGetResponse
            {
                Sessions = new[] { appointmentSlotSession }
            };

            var expectedClinician = new Clinician
            {
                Id = "34",
                DisplayName = "Dr Who"
            };

            var exoectedAppointmentSession = new AppointmentSession
            {
                Id = "77",
                DisplayName = "Timed",
            };
            
            var expectedSlot = new Slot
            {
                Id = "1",
                AppointmentSessionId = "2",
                ClinicianIds = new string[0],
                LocationId = "",
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19").ToUniversalTime(),
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19").ToUniversalTime(),
            };
            
            var expectedResponse = new AppointmentSlotsResponse
            {
                Clinicians = new[] { expectedClinician },
                AppointmentSessions = new[] { exoectedAppointmentSession },
                Locations = new Worker.Areas.Appointments.Models.Location[0],
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
            var session = new Worker.Bridges.Emis.Models.Session
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
                CreateAppointmentsSlotSession(1, 2, "2018-05-09T10:59:19", "2018-05-09T10:59:19");

            var slotsResponse = new AppointmentsSlotsGetResponse
            {
                Sessions = new[] { appointmentSlotSession }
            };

            var exoectedAppointmentSession = new AppointmentSession
            {
                Id = "77",
                DisplayName = "Timed"
            };

            var expectedLocation = new Worker.Areas.Appointments.Models.Location
            {
                Id = "365",
                DisplayName = "Leeds"
            };
            
            var expectedSlot = new Slot
            {
                Id = "1",
                AppointmentSessionId = "2",
                ClinicianIds = new string[0],
                LocationId = "",
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19").ToUniversalTime(),
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19").ToUniversalTime(),
                
            };
            
            var expectedResponse = new AppointmentSlotsResponse
            {
                Clinicians = new Clinician[0],
                AppointmentSessions = new[] { exoectedAppointmentSession },
                Locations = new[] { expectedLocation },
                Slots = new[] { expectedSlot }
            };

            // Act
            var actualResponse = _sut.Map(slotsResponse, slotsMetadataResponse);
            
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        private static AppointmentSlotSession CreateAppointmentsSlotSession(int slotId, int sessionId, string startTime, string endTime)
        {
            var appointmentSlot = new AppointmentSlot
            {
                SlotId = slotId,
                EndTime = endTime,
                StartTime = startTime,
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
