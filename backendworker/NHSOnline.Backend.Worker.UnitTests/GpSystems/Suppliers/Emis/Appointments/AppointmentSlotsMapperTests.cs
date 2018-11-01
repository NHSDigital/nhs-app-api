using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Appointments
{
    [TestClass]
    public class AppointmentSlotsMapperTests
    {
        private IFixture _fixture;
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        private AppointmentSlotsMapper _systemUnderTest;
        private Mock<IEmisEnumMapper> _mockEmisEnumMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            _timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
            
            _mockEmisEnumMapper = _fixture.Freeze<Mock<IEmisEnumMapper>>();
            
            var logger = _fixture.Create<ILoggerFactory>().CreateLogger<AppointmentSlotsMapper>();            

            _systemUnderTest = new AppointmentSlotsMapper(_dateTimeOffsetProvider, logger,_mockEmisEnumMapper.Object);            
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
            var slotSessions = new List<AppointmentSlotSession>();
            
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
            
            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            var expectedSlot = new Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = null,
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Type = "Emergency"
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
            
            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            var slot = new Slot
            {
                Id = "901",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "Emergency"
            };
            var expectedResponse = new[]{ slot };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
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
                CreateAppointmentsSlotSession(102, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", null,"Unknown");

            var slotSessions = new[] { appointmentSlotSession1, appointmentSlotSession2, appointmentSlotSession3 };
            
            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var sessionWithInvalidStartTime = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 1, "Untimed");
            var session = CreateSession(new[]{ sessionHolder.ClinicianId }, location.LocationId, 9, "Unknown");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session, sessionWithInvalidStartTime };

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            var slot1 = new Slot
            {
                Id = "901",
                Clinicians = new[]{ "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "Emergency",
                Channel = Channel.Unknown
            };
            
            var slot2 = new Slot
            {
                Id = "101",
                Clinicians = new[]{ "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Type = "Emergency",
                Channel = Channel.Unknown
            };

            var slot3 = new Slot
            {
                Id = "102",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = string.Empty,
                Channel = Channel.Unknown
            };

            var expectedResponse = new[]{ slot1, slot2, slot3 };
            
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ReturnsAppropriateSlotDisplayName()
        {
            // Arrange
            var appointmentSlotSession1 =
                CreateAppointmentsSlotSession(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency","Unknown");

            var appointmentSlotSession2 =
                CreateAppointmentsSlotSession(901, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency","Unknown");

            var appointmentSlotSession3 =
                CreateAppointmentsSlotSession(102, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", null,"Unknown");

            var slotSessions = new[] { appointmentSlotSession1, appointmentSlotSession2, appointmentSlotSession3 };

            var location = CreateLocation(23, "Leeds");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var sessionWithInvalidStartTime = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 1, "Untimed");
            var session = CreateSession(new[] { sessionHolder.ClinicianId }, location.LocationId, 9, "Unknown", "GP Session");

            var locations = new[] { location };
            var sessionHolders = new[] { sessionHolder };
            var sessions = new[] { session, sessionWithInvalidStartTime };

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            var slot1 = new Slot
            {
                Id = "901",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "GP Session - Emergency"
            };

            var slot2 = new Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Type = "Emergency"
            };

            var slot3 = new Slot
            {
                Id = "102",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "GP Session"
            };

            var expectedResponse = new[] { slot1, slot2, slot3 };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
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

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, sessionHolders, sessions);

            // Assert
            var slot = new Slot
            {
                Id = "901",
                Clinicians = new List<string>(),
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "Emergency"
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
                CreateAppointmentsSlotSession(901, 9, "2018-07-12T10:59:19", "2018-07-12T10:59:19", null,"Unknown");

            var slotSessions = new[] { appointmentSlotSession1, appointmentSlotSession2 };

            var location = CreateLocation(23, "Leeds");
            var session = CreateSession(new[] { 11 }, location.LocationId, 9, "Unknown", "GP Session");

            var locations = new[] { location };
            var sessions = new[] { session };

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions, locations, null, sessions);

            // Assert
            var slot1 = new Slot
            {
                Id = "900",
                Clinicians = new List<string>(),
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "GP Session - Emergency"
            };

            var slot2 = new Slot
            {
                Id = "901",
                Clinicians = new List<string>(),
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "GP Session"
            };

            var expectedResponse = new[] { slot1, slot2};

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

        private static Worker.GpSystems.Suppliers.Emis.Models.Session CreateSession(IEnumerable<int> clinicianIds, int locationId, int sessionId, string sessionType)
        {
            return CreateSession(clinicianIds, locationId, sessionId, sessionType, string.Empty);
        }

        private static Worker.GpSystems.Suppliers.Emis.Models.Session CreateSession(IEnumerable<int> clinicianIds, int locationId, int sessionId, string sessionType, string sessionName)
        {
            return new Worker.GpSystems.Suppliers.Emis.Models.Session
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
