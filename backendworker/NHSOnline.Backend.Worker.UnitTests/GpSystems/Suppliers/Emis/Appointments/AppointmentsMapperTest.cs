using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Temporal;
using Appointment = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Appointment;
using Location = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Appointments
{
    [TestClass]
    public class AppointmentsMapperTests
    {
        private IFixture _fixture;
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        private AppointmentsMapper _systemUnderTest;
        private DateTime _tomorrow;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            _timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
            
            var logger = _fixture.Create<ILoggerFactory>().CreateLogger<AppointmentsMapper>();
            
            _systemUnderTest = new AppointmentsMapper(_dateTimeOffsetProvider, logger);
            
            _tomorrow = DateTime.Now.AddDays(1);
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
                CreateAppointment(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");

            var appointments = new[] { appointment };

            // Act
            var actualResponse = _systemUnderTest.Map(appointments, locations, sessionHolders, null);

            // Assert
            actualResponse.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_ReturnsNoLocation_WhenLocationsInResponseIsNull()
        {
            // Arrange
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr Crusher");
            var session = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 1, "Timed");
            
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session };

            var appointment =
                CreateAppointment(101, 1, UnitTestHelpers.DateTimeToJson(_tomorrow), UnitTestHelpers.DateTimeToJson(_tomorrow), "Emergency");

            var appointments = new[] { appointment };

            // Act
            var actualResponse = _systemUnderTest.Map(appointments, null, sessionHolders, sessions);

            // Assert
            var expectedAppointment = new NHSOnline.Backend.Worker.GpSystems.Appointments.Models.UpcomingAppointment
            {
                Id = "101",
                Clinicians = new[] { "Dr Crusher" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow),
                Location = "",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow),
                Type = "Emergency"
            };
            var expectedResponse = new[] { expectedAppointment };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
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
                CreateAppointment(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");

            var appointments = new[] { appointment };

            // Act
            var actualResponse = _systemUnderTest.Map(appointments, locations, sessionHolders, Array.Empty<Worker.GpSystems.Suppliers.Emis.Models.Session>());

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
                CreateAppointment(101, 1, UnitTestHelpers.DateTimeToJson(_tomorrow), invalidEndTime, "Emergency");

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
            var expectedAppointment = new NHSOnline.Backend.Worker.GpSystems.Appointments.Models.UpcomingAppointment
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = null,
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow),
                Type = "Emergency"
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
                CreateAppointment(101, 1, invalidStartTime, UnitTestHelpers.DateTimeToJson(_tomorrow), "Emergency");

            var appointment2 =
                CreateAppointment(901, 9, UnitTestHelpers.DateTimeToJson(_tomorrow.AddDays(1)), UnitTestHelpers.DateTimeToJson(_tomorrow.AddDays(1)), "Emergency");

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
            var expectedAppointment = new NHSOnline.Backend.Worker.GpSystems.Appointments.Models.UpcomingAppointment
            {
                Id = "901",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow.AddDays(1)),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow.AddDays(1)),
                Type = "Emergency"
            };

            var expectedResponse = new[] { expectedAppointment };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_HappyPath_ReturnsAnEnumerableOfPastAndUpcomingAppointments()
        {
            // Arrange
            var appointment1 =
                CreateAppointment(101, 1, UnitTestHelpers.DateTimeToJson(_tomorrow.AddDays(1)), UnitTestHelpers.DateTimeToJson(_tomorrow.AddDays(1)), "Emergency");
            
            var appointment2=
                CreateAppointment(901, 9, UnitTestHelpers.DateTimeToJson(_tomorrow), UnitTestHelpers.DateTimeToJson(_tomorrow), "Emergency");

            var appointment3 =
                CreateAppointment(102, 9, UnitTestHelpers.DateTimeToJson(_tomorrow.AddMonths(1)), UnitTestHelpers.DateTimeToJson(_tomorrow.AddMonths(1)), null);
            
            var appointment4 =
                CreateAppointment(103, 1, UnitTestHelpers.DateTimeToJson(_tomorrow.AddDays(-1)), UnitTestHelpers.DateTimeToJson(_tomorrow.AddDays(-1)), "Emergency");

            var appointment5 =
                CreateAppointment(104, 9, UnitTestHelpers.DateTimeToJson(_tomorrow.AddMonths(-1)), UnitTestHelpers.DateTimeToJson(_tomorrow.AddMonths(-1)), null);

            var appointnments = new[] { appointment1, appointment2, appointment3, appointment4, appointment5 };
            
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
            var expectedAppointment1 = new NHSOnline.Backend.Worker.GpSystems.Appointments.Models.UpcomingAppointment
            {
                Id = "901",
                Clinicians = new[]{ "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow),
                Type = "Emergency"
            };
            
            var expectedAppointment2 = new NHSOnline.Backend.Worker.GpSystems.Appointments.Models.UpcomingAppointment
            {
                Id = "101",
                Clinicians = new[]{ "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow.AddDays(1)),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow.AddDays(1)),
                Type = "Emergency"
            };
            
            var expectedAppointment3 = new NHSOnline.Backend.Worker.GpSystems.Appointments.Models.UpcomingAppointment
            {
                Id = "102",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow.AddMonths(1)),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow.AddMonths(1)),
                Type = string.Empty
            };
            
            var expectedAppointment4 = new NHSOnline.Backend.Worker.GpSystems.Appointments.Models.PastAppointment
            {
                Id = "103",
                Clinicians = new[]{ "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow.AddDays(-1)),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow.AddDays(-1)),
                Type = "Emergency"
            };

            var expectedAppointment5 = new NHSOnline.Backend.Worker.GpSystems.Appointments.Models.PastAppointment
            {
                Id = "104",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow.AddMonths(-1)),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow.AddMonths(-1)),
                Type = string.Empty
            };
            
            var expectedResponse = new NHSOnline.Backend.Worker.GpSystems.Appointments.Models.Appointment[]{ expectedAppointment1, expectedAppointment2, expectedAppointment3, expectedAppointment4, expectedAppointment5 };
            
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ReturnsNoClinicians_WhenNoneMatched()
        {
            // Arrange
            var appointmentSlotSession1 =
                CreateAppointment(901, 9, UnitTestHelpers.DateTimeToJson(_tomorrow), UnitTestHelpers.DateTimeToJson(_tomorrow), "Emergency");

            var slotSessions = new[] { appointmentSlotSession1 };

            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[] { 66 }, location.LocationId, 9, "Unknown");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session };

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            var expectedAppointment = new Worker.GpSystems.Appointments.Models.UpcomingAppointment
            {
                Id = "901",
                Clinicians = new List<string>(),
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(_tomorrow),
                Type = "Emergency"
            };

            var expectedResponse = new[] { expectedAppointment };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        private static Appointment CreateAppointment(int slotId, int sessionId, string startTime, string endTime, string slotTypeName)
        {
            var appointment = new Appointment
            {
                SlotId = slotId,
                EndTime = endTime,
                StartTime = startTime,
                SessionId = sessionId,
                SlotTypeName = slotTypeName
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

        private static Worker.GpSystems.Suppliers.Emis.Models.Session CreateSession(IEnumerable<int> clinicianIds, int locationId, int sessionId, string sessionType)
        {
            return new Worker.GpSystems.Suppliers.Emis.Models.Session
            {
                ClinicianIds = clinicianIds,
                LocationId = locationId,
                SessionId = sessionId,
                SessionType = sessionType
            };
        }
    }
}
