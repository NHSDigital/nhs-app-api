using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Appointments
{
    [TestClass]
    public class AppointmentSlotsMapperTests
    {
        private IFixture _fixture;
        private AppointmentSlotsMapper _systemUnderTest;
        private Mock<IEmisEnumMapper> _mockEmisEnumMapper;
        private Mock<ILogger<AppointmentSlotsMapper>> _mockLogger;
        private Mock<IDateTimeOffsetProvider> _dateTimeOffsetProviderMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            
            _dateTimeOffsetProviderMock = _fixture.Freeze<Mock<IDateTimeOffsetProvider>>();
            
            _mockEmisEnumMapper = _fixture.Freeze<Mock<IEmisEnumMapper>>();
         
            _mockLogger = _fixture.Freeze<Mock<ILogger<AppointmentSlotsMapper>>>();
            _systemUnderTest = new AppointmentSlotsMapper(_dateTimeOffsetProviderMock.Object, _mockLogger.Object,_mockEmisEnumMapper.Object);
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSessionsInAppointmentsSlotsResponseIsNull()
        {
            // Arrange
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 23, "Unknown");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session };
            
            // Act
            var actualResponse = _systemUnderTest.Map(null, locations, sessionHolders, sessions);

            // Assert
            actualResponse.Should().BeEmpty();
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSessionsInAppointmentsSlotsMetadataResponseIsNull()
        {
            // Arrange
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };

            var appointmentSlotSession =
                CreateAppointmentsSlotSession(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency", "Unknown");

            var slotSessions = new[] { appointmentSlotSession };

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, null);

            // Assert
            actualResponse.Should().BeEmpty();
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSessionsInAppointmentsSlotsResponseIsEmpty()
        {
            // Arrange
            var slotSessions = Array.Empty<AppointmentSlotSession>();
            
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 1, "Timed");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session };

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            actualResponse.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_ReturnsNoLocationInResponse_WhenLocationsIsNull()
        {
            // Arrange
            var appointmentSlotSession =
                CreateAppointmentsSlotSession(101, 1, "2018-05-09T10:59:19", "2018-05-09T11:14:19", "Emergency", "Unknown");

            var slotSessions = new[] { appointmentSlotSession };

            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr McCoy");
            var session = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 1, "Timed");
            
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session };
            
            var start = _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-05-09T10:59:19");
            var end = _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-05-09T11:14:19");
            
            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, null, sessionHolders, sessions);

            // Assert
            var expectedSlot = new Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr McCoy" },
                EndTime = end,
                Location = "",
                StartTime = (DateTimeOffset) start,
                Type = "Emergency",
                SessionName = "TestSessionName"
            };
            var expectedResponse = new[] { expectedSlot };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("2018-05-09T9:59:19")]
        public void Map_ReturnsResponseWithoutEndTime_WhenEndTimeInAppointmentSlotsIsInInvalidFormat(string invalidEndTime)
        {
            // Arrange
            var appointmentSlotSession =
                CreateAppointmentsSlotSession(101, 1, "2018-05-09T10:59:19", invalidEndTime, "Emergency","Unknown");

            var slotSessions = new[] { appointmentSlotSession };
            
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 1, "Untimed");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session };
            
            var start = _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-05-09T10:59:19");
            
            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            var expectedSlot = new Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = null,
                Location = "Leeds",
                StartTime = (DateTimeOffset) start,
                Type = "Emergency",
                SessionName = "TestSessionName"
            };
            var expectedResponse = new[] { expectedSlot };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("2018-05-09T9:59:19")]
        public void Map_ReturnsResponseWithoutSlot_WhenStartTimeInAppointmentSlotsIsInInvalidFormat(string invalidStartTime)
        {
            // Arrange
            var appointmentSlotSessionWithInvalidStartTime =
                CreateAppointmentsSlotSession(101, 1, invalidStartTime, "2018-05-09T10:59:19", "Emergency","Unknown");
            
            var appointmentSlotSession=
                CreateAppointmentsSlotSession(901, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency","Unknown");
            
            var slotSessions = new[]{ appointmentSlotSessionWithInvalidStartTime, appointmentSlotSession};
            
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var sessionWithInvalidStartTime = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 1, "Unknown");
            var session = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 9, "Timed");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session, sessionWithInvalidStartTime };
            
            _mockLogger.SetupLogger(LogLevel.Warning, $"Unable to parse EMIS Appointment Slot Start Time of '{invalidStartTime}", null).Verifiable();
            
            var startEndSlot = _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-07-12T10:59:19");
            
            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            var slot = new Slot
            {
                Id = "901",
                Clinicians = new[] { "Dr House" },
                EndTime = startEndSlot,
                Location = "Leeds",
                StartTime = (DateTimeOffset) startEndSlot,
                Type = "Emergency",
                SessionName = "TestSessionName"
            };
            var expectedResponse = new[]{ slot };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockLogger.Verify();
        }
        
        [TestMethod]
        public void Map_HappyPath_ReturnsAnArrayOfSlots()
        {
            // Arrange
            var appointmentSlotSession1 =
                CreateAppointmentsSlotSession(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency","Unknown");
            
            var appointmentSlotSession2=
                CreateAppointmentsSlotSession(901, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency","Unknown");

            var appointmentSlotSession3 =
                CreateAppointmentsSlotSession(102, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", "Unknown","Unknown");

            var slotSessions = new[] { appointmentSlotSession1, appointmentSlotSession2, appointmentSlotSession3 };
            
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var sessionWithInvalidStartTime = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 1, "Untimed");
            var session = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 9, "Unknown");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session, sessionWithInvalidStartTime };
            
            var slot1Times = _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-05-09T10:59:19");
            var slot2Times = _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-07-12T10:59:19");

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            var slot1 = new Slot
            {
                Id = "901",
                Clinicians = new[]{ "Dr House" },
                EndTime = slot2Times,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slot2Times,
                Type = "Emergency",
                SessionName = "TestSessionName",
                Channel = Channel.Unknown
            };
            
            var slot2 = new Slot
            {
                Id = "101",
                Clinicians = new[]{ "Dr House" },
                EndTime = slot1Times,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slot1Times,
                Type = "Emergency",
                SessionName = "TestSessionName",
                Channel = Channel.Unknown
            };
            
            var slot3 = new Slot
            {
                Id = "102",
                Clinicians = new[]{ "Dr House" },
                EndTime = slot2Times,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slot2Times,
                Type = "Unknown",
                SessionName = "TestSessionName",
                Channel = Channel.Unknown
            };

            var expectedResponse = new[]{ slot1, slot2, slot3 };
            
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Map_ReturnsResponseWithoutSlot_WhenSlotTypeIsMissing()
        {
            // Arrange
            var appointmentSlotSession1 =
                CreateAppointmentsSlotSession(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency","Unknown");
            
            var appointmentSlotSession2=
                CreateAppointmentsSlotSession(901, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency","Unknown");

            var appointmentSlotSessionNullSlotType =
                CreateAppointmentsSlotSession(102, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", null,"Unknown");

            var slotSessions = new[] { appointmentSlotSession1, appointmentSlotSession2, appointmentSlotSessionNullSlotType };
            
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var sessionWithInvalidStartTime = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 1, "Untimed");
            var session = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 9, "Unknown");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session, sessionWithInvalidStartTime };
            
            var slot1Times = _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-05-09T10:59:19");
            var slot2Times = _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-07-12T10:59:19");

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            var slot1 = new Slot
            {
                Id = "901",
                Clinicians = new[]{ "Dr House" },
                EndTime = slot2Times,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slot2Times,
                Type = "Emergency",
                SessionName = "TestSessionName",
                Channel = Channel.Unknown
            };
            
            var slot2 = new Slot
            {
                Id = "101",
                Clinicians = new[]{ "Dr House" },
                EndTime = slot1Times,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slot1Times,
                Type = "Emergency",
                SessionName = "TestSessionName",
                Channel = Channel.Unknown
            };

            var expectedResponse = new[]{ slot1, slot2 };
            
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockLogger.VerifyLogger(LogLevel.Warning, "Unable to parse EMIS Appointment Slot - slot type name null or empty", Times.Once());
        }

        [TestMethod]
        public void Map_ReturnsAppropriateSlotDisplayName()
        {
            // Arrange
            var appointmentSlotSession1 =
                CreateAppointmentsSlotSession(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency","Unknown");

            var appointmentSlotSession2 =
                CreateAppointmentsSlotSession(901, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency","Unknown");

            var appointmentSlotSessionNullSlotType =
                CreateAppointmentsSlotSession(102, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", null,"Unknown");

            var slotSessions = new[] { appointmentSlotSession1, appointmentSlotSession2, appointmentSlotSessionNullSlotType };

            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var sessionWithInvalidStartTime = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 1, "Untimed");
            var session = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 9, "Unknown", "GP Session");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session, sessionWithInvalidStartTime };
            
            var slot1Times = _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-05-09T10:59:19");
            var slot2Times = _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-07-12T10:59:19");

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            var slot1 = new Slot
            {
                Id = "901",
                Clinicians = new[] { "Dr House" },
                EndTime = slot2Times,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slot2Times,
                Type = "Emergency",
                SessionName = "GP Session",
            };

            var slot2 = new Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = slot1Times,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slot1Times,
                Type = "Emergency",
                SessionName = "TestSessionName",
            };
            
            var expectedResponse = new[] { slot1, slot2 };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockLogger.VerifyLogger(LogLevel.Warning, "Unable to parse EMIS Appointment Slot - slot type name null or empty", Times.Once());
        }
        
        [TestMethod]
        public void Map_ReturnsChannelObtainedFromEmisEnumMapper()
        {
            // Arrange
            var inputSlotTypeStatus = _fixture.Create<string>();
            var slotSessions = new[] { CreateAppointmentsSlotSession(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency",inputSlotTypeStatus) };
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 9, "Unknown", "GP Session");
            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session };

            var outputChannel = _fixture.Create<Channel>();
            
            _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-05-09T10:59:19");

            _mockEmisEnumMapper.Setup(x => x.MapSlotTypeStatus(inputSlotTypeStatus, Channel.Unknown))
                .Returns(outputChannel);
            
            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            actualResponse.Single().Channel.Should().Be(outputChannel);
        }
      

        [TestMethod]
        public void Map_ReturnsNoClinicians_WhenNoneMatched()
        {
            // Arrange
            var appointmentSlotSession1 =
                CreateAppointmentsSlotSession(901, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency","Unknown");

            var slotSessions = new[] { appointmentSlotSession1 };

            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(new[] { 66 }, location.LocationId, 9, "Unknown");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session };

            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-07-12T10:59:19");

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            var slot = new Slot
            {
                Id = "901",
                Clinicians = new List<string>(),
                EndTime = slotTime,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTime,
                Type = "Emergency",
                SessionName = "TestSessionName",
            };

            var expectedResponse = new[] { slot };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ReturnsNoClinicians_WhenSessionHoldersIsNull()
        {
            // Arrange

            var appointmentSlotSession1 =
                CreateAppointmentsSlotSession(900, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency","Unknown");

            var appointmentSlotSession2 =
                CreateAppointmentsSlotSession(901, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency","Unknown");

            var slotSessions = new[] { appointmentSlotSession1, appointmentSlotSession2 };

            var location = CreateLocation(23, "Leeds");
            var session = CreateSession(new[] { 11 }, location.LocationId, 9, "Unknown", "GP Session");

            var locations = new[] { location };
            var sessions = new[] { session };
            
            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-07-12T10:59:19");

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, null, sessions);

            // Assert
            var slot1 = new Slot
            {
                Id = "900",
                Clinicians = new List<string>(),
                EndTime = slotTime,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTime,
                Type = "Emergency",
                SessionName = "GP Session"
            };
            
            var slot2 = new Slot
            {
                Id = "901",
                Clinicians = new List<string>(),
                EndTime = slotTime,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTime,
                Type = "Emergency",
                SessionName = "GP Session"
            };

            var expectedResponse = new[] { slot1, slot2 };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }      
        
        private static AppointmentSlotSession CreateAppointmentsSlotSession(int slotId, int sessionId, string startTime, string endTime, string slotTypeName, string slotTypeStatus)
        {
            var appointmentSlot = new AppointmentSlot()
            {
                SlotId = slotId,
                EndTime = endTime,
                StartTime = startTime,
                SlotTypeName = slotTypeName,
                SlotTypeStatus = slotTypeStatus
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

        private static Backend.GpSystems.Suppliers.Emis.Models.Session CreateSession(IEnumerable<int> clinicianIds, int locationId, int sessionId, string sessionType)
        {
            return CreateSession(clinicianIds, locationId, sessionId, sessionType, "TestSessionName");
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
