using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;
using Appointment = NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Appointment;
using Location = NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Location;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Appointments
{
    [TestClass]
    public class AppointmentsMapperTests
    {
        private IFixture _fixture;
        private AppointmentsMapper _systemUnderTest;
        private DateTime _testDate;
        private DateTime _tomorrow;
        private DateTime _today;
        private DateTime _nextMonth;
        private DateTime _twoDaysFromNow;
        private DateTime _lastMonth;
        private Mock<ICurrentDateTimeProvider> _mockCurrentDateTimeProvider;
        private Mock<IDateTimeOffsetProvider> _dateTimeOffsetProviderMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _mockCurrentDateTimeProvider = _fixture.Freeze<Mock<ICurrentDateTimeProvider>>();
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(DateTime.UtcNow);
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            
            _dateTimeOffsetProviderMock = _fixture.Freeze<Mock<IDateTimeOffsetProvider>>();
            
            var logger = _fixture.Create<ILoggerFactory>().CreateLogger<AppointmentsMapper>();
            
            _fixture.Inject(_dateTimeOffsetProviderMock);
            
            _systemUnderTest = new AppointmentsMapper(_dateTimeOffsetProviderMock.Object, logger);
            
            _testDate = DateTime.Today;
            _tomorrow = _testDate.AddDays(1);
            _today = _testDate.AddHours(1);
            _nextMonth = _testDate.AddMonths(1);
            _twoDaysFromNow = _testDate.AddDays(2);
            _lastMonth = _testDate.AddMonths(-1);
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenAppointmentsInResponseIsNull()
        {
            // Arrange
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 23, "Unknown", "Default Session");

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
            var session = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 1, "Timed", "Default Session");
            
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session };

            var appointment =
                CreateAppointment(101, 1, DateTimeHelper.DateTimeToJson(_twoDaysFromNow), DateTimeHelper.DateTimeToJson(_twoDaysFromNow), "Emergency");

            var appointments = new[] { appointment };

            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset(_twoDaysFromNow);

            // Act
            var actualResponse = _systemUnderTest.Map(appointments, null, sessionHolders, sessions);

            // Assert
            var expectedAppointment = new NHSOnline.Backend.GpSystems.Appointments.Models.UpcomingAppointment
            {
                Id = "101",
                Clinicians = new[] { "Dr Crusher" },
                EndTime = slotTime,
                Location = "",
                StartTime = (DateTimeOffset) slotTime,
                Type = "Emergency",
                SessionName = "Default Session"
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
            var session = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 1, "Timed", "Default Session");

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
            var actualResponse = _systemUnderTest.Map(appointments, locations, sessionHolders, Array.Empty<Backend.GpSystems.Suppliers.Emis.Models.Session>());

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
                CreateAppointment(101, 1, DateTimeHelper.DateTimeToJson(_tomorrow), invalidEndTime, "Emergency");

            var appointments = new[] { appointment };

            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 1, "Untimed", "Default Session");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session };
            
            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset(_tomorrow);

            // Act
            var actualResponse = _systemUnderTest.Map(appointments, locations, sessionHolders, sessions);

            // Assert
            var expectedAppointment = new NHSOnline.Backend.GpSystems.Appointments.Models.UpcomingAppointment
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = null,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTime,
                Type = "Emergency",
                SessionName = "Default Session"
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
                CreateAppointment(101, 1, invalidStartTime, DateTimeHelper.DateTimeToJson(_tomorrow), "Emergency");

            var appointment2 =
                CreateAppointment(901, 9, DateTimeHelper.DateTimeToJson(_twoDaysFromNow), DateTimeHelper.DateTimeToJson(_twoDaysFromNow), "Emergency");

            var appointments = new[] { appointmentWithInvalidStartTime, appointment2 };

            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var sessionWithInvalidStartTime = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 1, "Unknown", "sessionWithInvalidStartTime");
            var session = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 9, "Timed", "Default Session");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session, sessionWithInvalidStartTime };
            
            var slotTimeTwoDays = _dateTimeOffsetProviderMock.MockDateTimeOffset(_twoDaysFromNow);

            // Act
            var actualResponse = _systemUnderTest.Map(appointments, locations, sessionHolders, sessions);

            // Assert
            var expectedAppointment = new NHSOnline.Backend.GpSystems.Appointments.Models.UpcomingAppointment
            {
                Id = "901",
                Clinicians = new[] { "Dr House" },
                EndTime = slotTimeTwoDays,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeTwoDays,
                Type = "Emergency",
                SessionName = "Default Session"
            };

            var expectedResponse = new[] { expectedAppointment };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_HappyPath_ReturnsAnEnumerableOfPastAndUpcomingAppointments()
        {
            // Arrange
            var appointment1 =
                CreateAppointment(101, 1, DateTimeHelper.DateTimeToJson(_twoDaysFromNow), DateTimeHelper.DateTimeToJson(_twoDaysFromNow), "Emergency");
            
            var appointment2=
                CreateAppointment(901, 9, DateTimeHelper.DateTimeToJson(_tomorrow), DateTimeHelper.DateTimeToJson(_tomorrow), "Emergency");

            var appointment3 =
                CreateAppointment(102, 9, DateTimeHelper.DateTimeToJson(_nextMonth), DateTimeHelper.DateTimeToJson(_nextMonth), null);
            
            var appointment4 =
                CreateAppointment(103, 1, DateTimeHelper.DateTimeToJson(_today), DateTimeHelper.DateTimeToJson(_today), "Emergency");

            var appointment5 =
                CreateAppointment(104, 9, DateTimeHelper.DateTimeToJson(_lastMonth), DateTimeHelper.DateTimeToJson(_lastMonth), null);

            var appointments = new[] { appointment1, appointment2, appointment3, appointment4, appointment5 };
            
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var sessionWithInvalidStartTime = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 1, "Untimed", "Invalid Start Time Session");
            var session = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 9, "Unknown", "Default Session");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session, sessionWithInvalidStartTime };
            
            var slotTimeTwoDays = _dateTimeOffsetProviderMock.MockDateTimeOffset(_twoDaysFromNow);
            var slotTimeTomorrow = _dateTimeOffsetProviderMock.MockDateTimeOffset(_tomorrow);
            var slotTimeNextMonth = _dateTimeOffsetProviderMock.MockDateTimeOffset(_nextMonth);
            var slotTimeToday = _dateTimeOffsetProviderMock.MockDateTimeOffset(_today);
            var slotTimeLastMonth = _dateTimeOffsetProviderMock.MockDateTimeOffset(_lastMonth);

            // Act
            var actualResponse = _systemUnderTest.Map(appointments, locations, sessionHolders, sessions);

            // Assert
            var expectedAppointment1 = new NHSOnline.Backend.GpSystems.Appointments.Models.UpcomingAppointment
            {
                Id = "901",
                Clinicians = new[]{ "Dr House" },
                EndTime = slotTimeTomorrow,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeTomorrow,
                Type = "Emergency",
                SessionName = "Default Session"
            };
            
            var expectedAppointment2 = new NHSOnline.Backend.GpSystems.Appointments.Models.UpcomingAppointment
            {
                Id = "101",
                Clinicians = new[]{ "Dr House" },
                EndTime = slotTimeTwoDays,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeTwoDays,
                Type = "Emergency",
                SessionName = "Invalid Start Time Session"
            };
            
            var expectedAppointment3 = new NHSOnline.Backend.GpSystems.Appointments.Models.UpcomingAppointment
            {
                Id = "102",
                Clinicians = new[] { "Dr House" },
                EndTime = slotTimeNextMonth,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeNextMonth,
                Type = string.Empty,
                SessionName = "Default Session"
            };
            
            var expectedAppointment4 = new NHSOnline.Backend.GpSystems.Appointments.Models.PastAppointment
            {
                Id = "103",
                Clinicians = new[]{ "Dr House" },
                EndTime = slotTimeToday,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeToday,
                Type = "Emergency",
                SessionName = "Invalid Start Time Session"
            };

            var expectedAppointment5 = new NHSOnline.Backend.GpSystems.Appointments.Models.PastAppointment
            {
                Id = "104",
                Clinicians = new[] { "Dr House" },
                EndTime = slotTimeLastMonth,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeLastMonth,
                Type = string.Empty,
                SessionName = "Default Session"
            };
            
            var expectedResponse = new NHSOnline.Backend.GpSystems.Appointments.Models.Appointment[]{ expectedAppointment1, expectedAppointment2, expectedAppointment3, expectedAppointment4, expectedAppointment5 };
            
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ReturnsNoClinicians_WhenNoneMatched()
        {
            // Arrange
            var appointmentSlotSession1 =
                CreateAppointment(901, 9, DateTimeHelper.DateTimeToJson(_tomorrow), DateTimeHelper.DateTimeToJson(_tomorrow), "Emergency");

            var slotSessions = new[] { appointmentSlotSession1 };

            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[] { 66 }, location.LocationId, 9, "Unknown", "Default Session");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session };

            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset(_tomorrow);
            
            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            var expectedAppointment = new Backend.GpSystems.Appointments.Models.UpcomingAppointment
            {
                Id = "901",
                Clinicians = new List<string>(),
                EndTime = slotTime,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTime,
                Type = "Emergency",
                SessionName = "Default Session"
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

        private static Backend.GpSystems.Suppliers.Emis.Models.Session CreateSession(IEnumerable<int> clinicianIds, int locationId, int sessionId, string sessionType, string sessionName)
        {
            return new Backend.GpSystems.Suppliers.Emis.Models.Session
            {
                ClinicianIds = clinicianIds,
                LocationId = locationId,
                SessionId = sessionId,
                SessionType = sessionType,
                SessionName = sessionName
            };
        }
    }
}
