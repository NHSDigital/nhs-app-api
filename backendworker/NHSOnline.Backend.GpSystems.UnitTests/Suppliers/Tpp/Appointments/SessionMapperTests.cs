using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support.Temporal;
using Slot = NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Slot;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class SessionMapperTests
    {
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        private SessionMapper _systemUnderTest;
        private IFixture _fixture;
        private Mock<ILogger<SessionMapper>> _mockLogger;
        private Mock<ICurrentDateTimeProvider> _mockCurrentDateTimeProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture() 
                .Customize(new AutoMoqCustomization());
            
            _mockCurrentDateTimeProvider = _fixture.Freeze<Mock<ICurrentDateTimeProvider>>();
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(DateTime.UtcNow);
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            _timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);
            _fixture.Inject(_dateTimeOffsetProvider); 
            _mockLogger = _fixture.Freeze<Mock<ILogger<SessionMapper>>>();
            _systemUnderTest = new SessionMapper(_dateTimeOffsetProvider, _mockLogger.Object);
        }
        
        [TestMethod]
        public void Map_TPPDateFormatShouldBeValid_ReturnsExpectedSlot()
        {
            var session = CreateSession("101", "Dr House", "Leeds", "Emergency");
            session.Slots = new[]
            {
                CreateSlot("2018-07-18T14:20:00.0Z","2018-07-18T14:30:00.0Z","Vaccination")
            }.ToList();

            var actualResponse = _systemUnderTest.Map(new[] { session });

            actualResponse.Should().HaveCount(1);
        }
        
        [TestMethod]
        public void Map_HappyPath_ReturnsAnArrayOfSlots()
        {
            // Arrange
            var appointmentSlot1 =
                CreateSlot("2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");

            var appointmentSlot2 =
                CreateSlot("2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency");

            var appointmentSlot3 =
                CreateSlot("2018-07-12T10:59:19", "2018-07-12T10:59:19", "Unknown");

            var session1 =
                CreateSession("101", "Dr House", "Leeds", "");

            var session2 =
                CreateSession("102", "Dr House", "Leeds", "");

            session1.Slots = new[] { appointmentSlot1, appointmentSlot3 }.ToList();
            session2.Slots = new[] { appointmentSlot2 }.ToList();

            var sessions = new[] { session1, session2 };

            // Act
            var actualResponse = _systemUnderTest.Map(sessions);

            // Assert
            var slot1 = new Backend.GpSystems.Appointments.Models.Slot
            {
                Id = "102",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown
            };

            var slot2 = new Backend.GpSystems.Appointments.Models.Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown
            };
            
            var slot3 = new Backend.GpSystems.Appointments.Models.Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "Unknown",
                SessionName = "",
                Channel = Channel.Unknown
            };

            var expectedResponse = new[] { slot1, slot2, slot3 };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Map_ReturnsResponseWithoutSlot_WhenSlotTypeIsMissing()
        {
            // Arrange
            var appointmentSlot1 =
                CreateSlot("2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");

            var appointmentSlot2 =
                CreateSlot("2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency");

            var appointmentSlot3 =
                CreateSlot("2018-07-12T10:59:19", "2018-07-12T10:59:19", null);

            var session1 =
                CreateSession("101", "Dr House", "Leeds", "");

            var session2 =
                CreateSession("102", "Dr House", "Leeds", "");

            session1.Slots = new[] { appointmentSlot1, appointmentSlot3 }.ToList();
            session2.Slots = new[] { appointmentSlot2 }.ToList();

            var sessions = new[] { session1, session2 };

            // Act
            var actualResponse = _systemUnderTest.Map(sessions);

            // Assert
            var slot1 = new Backend.GpSystems.Appointments.Models.Slot
            {
                Id = "102",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown
            };

            var slot2 = new Backend.GpSystems.Appointments.Models.Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown
            };

            var expectedResponse = new[] { slot1, slot2 };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockLogger.VerifyLogger(LogLevel.Warning, "Unable to parse TPP Appointment Slot - slot type name null or empty", null, Times.Exactly(1));
        }

        [TestMethod]
        public void Map_ReturnsAppropriateSlotDisplayName()
        {
            var appointmentSlot1 =
                CreateSlot("2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");

            var appointmentSlot2 =
                CreateSlot("2018-07-12T10:59:19", "2018-07-12T10:59:19", "Unknown");

            var appointmentSlot3 =
                CreateSlot("2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency");

            var session1 =
                CreateSession("101", "Dr House", "Leeds", "GP Session");

            var session2 =
                CreateSession("102", "Dr House", "Leeds", "");

            session1.Slots = new[] { appointmentSlot1, appointmentSlot2 }.ToList();
            session2.Slots = new[] { appointmentSlot3 }.ToList();

            var sessions = new[] { session1, session2 };

            var actualResponse = _systemUnderTest.Map(sessions);

            var slot1 = new Backend.GpSystems.Appointments.Models.Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Type = "Emergency",
                SessionName = "GP Session"
            };
            
            var slot2 = new Backend.GpSystems.Appointments.Models.Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "Unknown",
                SessionName = "GP Session"
            };

            var slot3 = new Backend.GpSystems.Appointments.Models.Slot
            {
                Id = "102",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "Emergency",
                SessionName = ""
            };

            var expectedResponse = new[] { slot1, slot2, slot3 };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSessionsInListSlotsReplyIsNull()
        {
            // Act
            var actualResponse = _systemUnderTest.Map(null);

            // Assert
            actualResponse.Should().BeEmpty();
        }
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSessionsInListSlotsReplyIsEmpty()
        {
            // Act
            var actualResponse = _systemUnderTest.Map(Array.Empty<Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Session>());

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
            var slotWithInvalidEndTime = CreateSlot("2018-05-09T10:59:19", invalidEndTime, "Emergency");

            var session = CreateSession("101", "Dr House", "Leeds", string.Empty);

            session.Slots = new List<Slot> { slotWithInvalidEndTime };

            // Act
            var actualResponse = _systemUnderTest.Map(new[] { session });

            // Assert
            var expectedSlot = new Backend.GpSystems.Appointments.Models.Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = null,
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Type = "Emergency",
                SessionName = ""
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
            var slotWithInvalidStartTime = CreateSlot(invalidStartTime, "2018-05-09T10:59:19", "Emergency");
            var slot = CreateSlot("2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency");

            var session = CreateSession("901", "Dr House", "Leeds", string.Empty);

            session.Slots = new List<Slot> { slot, slotWithInvalidStartTime };
            _mockLogger.SetupLogger(LogLevel.Warning, $"Unable to parse TPP Appointment Slot - wrong start date: {invalidStartTime}", null).Verifiable();
            // Act
            var actualResponse = _systemUnderTest.Map(new[] { session });

            // Assert
            var expectedSlot = new Backend.GpSystems.Appointments.Models.Slot
            {
                Id = "901",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "Emergency",
                SessionName = ""
            };
            var expectedResponse = new[] { expectedSlot };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockLogger.Verify();
        }

        private static Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Session CreateSession(
            string sessionId, 
            string staffDetails, 
            string location, 
            string type) => new Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Session
        {
            SessionId = sessionId,
            StaffDetails = staffDetails,
            Location = location,
            Type = type
        };

        private static Slot CreateSlot(string startDate, string endDate, string type) => new Slot
        {
            StartDate = startDate,
            EndDate = endDate,
            Type = type
        };
    }
}
