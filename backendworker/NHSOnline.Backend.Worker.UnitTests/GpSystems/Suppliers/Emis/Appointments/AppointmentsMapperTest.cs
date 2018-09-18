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

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOS()) });
            _timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
            
            var logger = _fixture.Create<ILoggerFactory>().CreateLogger<AppointmentsMapper>();
            
            _systemUnderTest = new AppointmentsMapper(_dateTimeOffsetProvider, logger);
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
                CreateAppointment(101, 1, "2018-05-09T10:59:19", invalidEndTime, "Emergency");

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
                Clinicians = new[] { "Dr House" },
                EndTime = null,
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19"),
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
                CreateAppointment(101, 1, invalidStartTime, "2018-05-09T10:59:19", "Emergency");

            var appointment2 =
                CreateAppointment(901, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency");

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
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-12T10:59:19"),
                Type = "Emergency"
            };

            var expectedResponse = new[] { expectedAppointment };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_HappyPath_ReturnsAnEnumerableOfAppointments()
        {
            // Arrange
            var appointment1 =
                CreateAppointment(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");
            
            var appointment2=
                CreateAppointment(901, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency");

            var appointment3 =
                CreateAppointment(102, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", null);

            var appointnments = new[] { appointment1, appointment2, appointment3 };
            
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
                Clinicians = new[]{ "Dr House" },
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-12T10:59:19"),
                Type = "Emergency"
            };
            
            var expectedAppointment2 = new NHSOnline.Backend.Worker.Areas.Appointments.Models.Appointment
            {
                Id = "101",
                Clinicians = new[]{ "Dr House" },
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19"),
                Type = "Emergency"
            };

            var expectedAppointment3 = new NHSOnline.Backend.Worker.Areas.Appointments.Models.Appointment
            {
                Id = "102",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-12T10:59:19"),
                Type = string.Empty
            };

            var expectedResponse = new[]{ expectedAppointment1, expectedAppointment2, expectedAppointment3 };
            
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
