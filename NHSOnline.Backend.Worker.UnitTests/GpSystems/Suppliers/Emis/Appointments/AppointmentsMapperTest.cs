using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Date;
using Appointment = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Appointment;
using Location = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Appointments
{
    [TestClass]
    public class AppointmentsMapperTests
    {       
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        private AppointmentsMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _timeZoneInfoProvider = new TimeZoneInfoProvider();
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
            _systemUnderTest = new AppointmentsMapper(_dateTimeOffsetProvider);
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenAppointmentsInResponseIsNull()
        {
            // Arrange
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 23, "Unknown");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session };

            // Act
            var actualResponse = _systemUnderTest.Map(null, locations, sessionHolders, sessions);

            // Assert
            actualResponse.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSessionsInResponseIsNull()
        {
            // Arrange
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };

            var appointment =
                CreateAppointment(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19");

            var appointments = new[] { appointment };

            // Act
            var actualResponse = _systemUnderTest.Map(appointments, locations, sessionHolders, null);

            // Assert
            actualResponse.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenAppointmentsInResponseIsEmpty()
        {
            // Arrange
            var appointments = new List<Appointment>();

            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 1, "Timed");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session };

            // Act
            var actualResponse = _systemUnderTest.Map(appointments, locations, sessionHolders, sessions);

            // Assert
            actualResponse.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSessionsInResponseIsEmpty()
        {
            // Arrange
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };

            var appointment =
                CreateAppointment(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19");

            var appointments = new[] { appointment };

            // Act
            var actualResponse = _systemUnderTest.Map(appointments, locations, sessionHolders, new Session[]{});

            // Assert
            actualResponse.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("2018-05-09T9:59:19")]
        public void Map_ReturnsResponseWithoutEndTime_WhenEndTimeInAppointmentIsInInvalidFormat(string invalidEndTime)
        {
            // Arrange
            var appointment =
                CreateAppointment(101, 1, "2018-05-09T10:59:19", invalidEndTime);

            var appointments = new[] { appointment };

            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 1, "Untimed");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session };

            // Act
            var actualResponse = _systemUnderTest.Map(appointments, locations, sessionHolders, sessions);

            // Assert
            var expectedAppointment = new NHSOnline.Backend.Worker.Areas.Appointments.Models.Appointment
            {
                Id = "101",
                AppointmentSessionId = "1",
                ClinicianIds = new[] { "55" },
                EndTime = null,
                LocationId = "23",
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19")
            };
            var expectedResponse = new[] { expectedAppointment };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("2018-05-09T9:59:19")]
        public void Map_ReturnsResponseWithoutSlot_WhenStartTimeInAppointmentIsInInvalidFormat(string invalidStartTime)
        {
            // Arrange
            var appointmentWithInvalidStartTime =
                CreateAppointment(101, 1, invalidStartTime, "2018-05-09T10:59:19");

            var appointment2 =
                CreateAppointment(901, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19");

            var appointments = new[] { appointmentWithInvalidStartTime, appointment2 };

            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var sessionWithInvalidStartTime = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 1, "Unknown");
            var session = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 9, "Timed");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session, sessionWithInvalidStartTime };

            // Act
            var actualResponse = _systemUnderTest.Map(appointments, locations, sessionHolders, sessions);

            // Assert
            var expectedAppointment = new NHSOnline.Backend.Worker.Areas.Appointments.Models.Appointment
            {
                Id = "901",
                AppointmentSessionId = "9",
                ClinicianIds = new[] { "55" },
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-12T10:59:19"),
                LocationId = "23",
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-12T10:59:19"),
            };

            var expectedResponse = new[] { expectedAppointment };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_HappyPath_ReturnsAnEnumerableOfAppointments()
        {
            // Arrange
            var appointment1 =
                CreateAppointment(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19");
            
            var appointment2=
                CreateAppointment(901, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19");

            var appointnments = new[] { appointment1, appointment2 };
            
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var sessionWithInvalidStartTime = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 1, "Untimed");
            var session = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 9, "Unknown");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session, sessionWithInvalidStartTime };

            // Act
            var actualResponse = _systemUnderTest.Map(appointnments, locations, sessionHolders, sessions);

            // Assert
            var expectedAppointment1 = new NHSOnline.Backend.Worker.Areas.Appointments.Models.Appointment
            {
                Id = "901",
                AppointmentSessionId = "9",
                ClinicianIds = new[]{ "55" },
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-12T10:59:19"),
                LocationId = "23",
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-12T10:59:19"),
            };
            
            var expectedAppointment2 = new NHSOnline.Backend.Worker.Areas.Appointments.Models.Appointment
            {
                Id = "101",
                AppointmentSessionId = "1",
                ClinicianIds = new[]{ "55" },
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19"),
                LocationId = "23",
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19"),
            };

            var expectedResponse = new[]{ expectedAppointment1, expectedAppointment2 };
            
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        private static Appointment CreateAppointment(int slotId, int sessionId, string startTime, string endTime)
        {
            var appointment = new Appointment
            {
                SlotId = slotId,
                EndTime = endTime,
                StartTime = startTime,
                SessionId = sessionId
            };

            return appointment;
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

        private static Session CreateSession(IEnumerable<int> clinicianIds, int locationId, int sessionId, string sessionType)
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
