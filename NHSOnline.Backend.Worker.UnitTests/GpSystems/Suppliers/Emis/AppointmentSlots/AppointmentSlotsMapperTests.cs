using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.AppointmentSlots;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Date;
using Location = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.AppointmentSlots
{
    [TestClass]
    public class AppointmentSlotsMapperTests
    {       
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _timeZoneInfoProvider = new TimeZoneInfoProvider();
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
            
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSessionsInAppointmentsSlotsResponseIsNull()
        {
            //given
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 23, "Unknown");

            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Locations = new[]{ location },
                SessionHolders = new[] { sessionHolder },
                Sessions = new[] { session }
            };
            
            //and

            var slotsResponse = new AppointmentsSlotsGetResponse();

            //when
            var mapper = new AppointmentSlotsMapper(_dateTimeOffsetProvider);
            var actualResponse = mapper.Map(slotsResponse, slotsMetadataResponse);

            //then
            actualResponse.Should().BeEquivalentTo(new Slot[0]);
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSessionsInAppointmentsSlotsMetadataResponseIsNull()
        {
            //given
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse();
            
            //and
            var appointmentSlotSession =
                CreateAppointmentsSlotSession(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19");
           
            var slotsResponse = new AppointmentsSlotsGetResponse
            {
                Sessions = new[]{ appointmentSlotSession }
            };

            //when
            var mapper = new AppointmentSlotsMapper(_dateTimeOffsetProvider);
            var actualResponse = mapper.Map(slotsResponse, slotsMetadataResponse);

            //then
            actualResponse.Should().BeEmpty();
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSessionsInAppointmentsSlotsResponseIsEmpty()
        {
            //given
            var slotsResponse = new AppointmentsSlotsGetResponse{
                Sessions = new List<AppointmentSlotSession>()
            };
            
            //and
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 1, "Timed");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Locations = new[]{ location },
                SessionHolders = new[] { sessionHolder },
                Sessions = new[] { session }
            };
            
            //when
            var mapper = new AppointmentSlotsMapper(_dateTimeOffsetProvider);
            var actualResponse = mapper.Map(slotsResponse, slotsMetadataResponse);

            //then
            actualResponse.Should().BeEmpty();
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("2018-05-09T9:59:19")]
        public void Map_ReturnsResponseWithoutEndTime_WhenEndTimeInAppointmentSlotsIsInInvalidFormat(string invalidEndTime)
        {
            //given AppointmentsSlotsGetResponse
            var appointmentSlotSession =
                CreateAppointmentsSlotSession(101, 1, "2018-05-09T10:59:19", invalidEndTime);
            
            var slotsResponse = new AppointmentsSlotsGetResponse
            {
                Sessions = new[]{ appointmentSlotSession }
            };
            
            //given AppointmentSlotsMetadataGetResponse
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 1, "Untimed");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Locations = new[]{ location },
                SessionHolders = new[] { sessionHolder },
                Sessions = new[] { session }
            };
            
            //when
            var mapper = new AppointmentSlotsMapper(_dateTimeOffsetProvider);
            var actualResponse = mapper.Map(slotsResponse, slotsMetadataResponse);

            //then
            var slot = new Slot()
            {
                Id = "101",
                AppointmentSessionId = "1",
                ClinicianIds = new[]{ "55" },
                EndTime = null,
                LocationId = "23",
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19")
            };

            var expectedResponse = new[] { slot };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("2018-05-09T9:59:19")]
        public void Map_ReturnsResponseWithoutSlot_WhenStartTimeInAppointmentSlotsIsInInvalidFormat(string invalidStartTime)
        {
            //given AppointmentsSlotsGetResponse
            var appointmentSlotSessionWithInvalidStartTime =
                CreateAppointmentsSlotSession(101, 1, invalidStartTime, "2018-05-09T10:59:19");
            
            var appointmentSlotSession=
                CreateAppointmentsSlotSession(901, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19");
            
            var slotsResponse = new AppointmentsSlotsGetResponse
            {
                Sessions = new[]{ appointmentSlotSessionWithInvalidStartTime, appointmentSlotSession}
            };
            
            //given AppointmentSlotsMetadataGetResponse
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var sessionWithInvalidStartTime = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 1, "Unknown");
            var session = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 9, "Timed");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Locations = new[]{ location },
                SessionHolders = new[] { sessionHolder },
                Sessions = new[] { session, sessionWithInvalidStartTime }
            };
            
            //when
            var mapper = new AppointmentSlotsMapper(_dateTimeOffsetProvider);
            var actualResponse = mapper.Map(slotsResponse, slotsMetadataResponse);

            //then
            var slot = new Slot()
            {
                Id = "901",
                AppointmentSessionId = "9",
                ClinicianIds = new[]{ "55" },
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-12T10:59:19"),
                LocationId = "23",
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-12T10:59:19"),
            };

            var expectedResponse = new[]{ slot };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Map_HappyPath_ReturnsAnArrayOfSlots()
        {
            //given AppointmentsSlotsGetResponse
            var appointmentSlotSession1 =
                CreateAppointmentsSlotSession(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19");
            
            var appointmentSlotSession2=
                CreateAppointmentsSlotSession(901, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19");
            
            var slotsResponse = new AppointmentsSlotsGetResponse
            {
                Sessions = new[]{ appointmentSlotSession1, appointmentSlotSession2}
            };
            
            //given AppointmentSlotsMetadataGetResponse
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var sessionWithInvalidStartTime = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 1, "Untimed");
            var session = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 9, "Unknown");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Locations = new[]{ location },
                SessionHolders = new[] { sessionHolder },
                Sessions = new[] { session, sessionWithInvalidStartTime }
            };
            
            //when
            var mapper = new AppointmentSlotsMapper(_dateTimeOffsetProvider);
            var actualResponse = mapper.Map(slotsResponse, slotsMetadataResponse);

            //then
            var slot1 = new Slot()
            {
                Id = "901",
                AppointmentSessionId = "9",
                ClinicianIds = new[]{ "55" },
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-12T10:59:19"),
                LocationId = "23",
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-12T10:59:19"),
            };
            
            var slot2 = new Slot()
            {
                Id = "101",
                AppointmentSessionId = "1",
                ClinicianIds = new[]{ "55" },
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19"),
                LocationId = "23",
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19"),
            };

            var expectedResponse = new[]{ slot1, slot2 };
            
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        private AppointmentSlotSession CreateAppointmentsSlotSession(int slotId, int sessionId, string startTime, string endTime)
        {
            var appointmentSlot = new AppointmentSlot()
            {
                SlotId = slotId,
                EndTime = endTime,
                StartTime = startTime,
            };
            
            var appointmentSlotSession = new AppointmentSlotSession()
            {
                SessionId = sessionId,
                Slots = new[]{ appointmentSlot }
            };

            return appointmentSlotSession;
        }

        private Location CreateLocation(int id, string name)
        {
            return new Location
            {
                LocationId = id,
                LocationName = name
            };
        }

        private SessionHolder CreateSessionHolder(int id, string name)
        {
            return new SessionHolder
            {
                ClinicianId = id,
                DisplayName = name
            };
        }

        private Session CreateSession(IEnumerable<int> clinicianIds, int locationId, int sessionId, string sessionType)
        {
            return new Session
            {
                ClinicianIds = clinicianIds,
                LocationId = locationId,
                SessionId = sessionId,
                SessionType = sessionType
            };
        }
    }
}
