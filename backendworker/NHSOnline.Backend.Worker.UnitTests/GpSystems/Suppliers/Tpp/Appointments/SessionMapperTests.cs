using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class SessionMapperTests
    {
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        private SessionMapper _systemUnderTest;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture() 
                .Customize(new AutoMoqCustomization()); 
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            _timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
            _fixture.Inject(_dateTimeOffsetProvider); 
            
            _systemUnderTest = _fixture.Create<SessionMapper>(); 
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
                CreateSlot("2018-07-12T10:59:19", "2018-07-12T10:59:19", null);

            var session1 =
                CreateSession("101", "Dr House", "Leeds", "");

            var session2 =
                CreateSession("102", "Dr House", "Leeds", "");

            session1.Slots = new[] { appointmentSlot1, appointmentSlot2 }.ToList();
            session2.Slots = new[] { appointmentSlot3 }.ToList();

            var sessions = new[] { session1, session2 };

            // Act
            var actualResponse = _systemUnderTest.Map(sessions);

            // Assert
            var slot1 = new Worker.Areas.Appointments.Models.Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "Emergency"
            };

            var slot2 = new Worker.Areas.Appointments.Models.Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Type = "Emergency"
            };

            var slot3 = new Worker.Areas.Appointments.Models.Slot
            {
                Id = "102",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = string.Empty
            };

            var expectedResponse = new[] { slot1, slot2, slot3 };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ReturnsAppropriateSlotDisplayName()
        {
            var appointmentSlot1 =
                CreateSlot("2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");

            var appointmentSlot2 =
                CreateSlot("2018-07-12T10:59:19", "2018-07-12T10:59:19", "");

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

            var slot1 = new Worker.Areas.Appointments.Models.Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Type = "GP Session - Emergency"
            };

            var slot2 = new Worker.Areas.Appointments.Models.Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "GP Session"
            };

            var slot3 = new Worker.Areas.Appointments.Models.Slot
            {
                Id = "102",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "Emergency"
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
            var actualResponse = _systemUnderTest.Map(Array.Empty<Worker.GpSystems.Suppliers.Tpp.Models.Appointments.Session>());

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
            var expectedSlot = new Worker.Areas.Appointments.Models.Slot
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
            var slotWithInvalidStartTime = CreateSlot(invalidStartTime, "2018-05-09T10:59:19", "Emergency");
            var slot = CreateSlot("2018-07-12T10:59:19", "2018-07-12T10:59:19", "Emergency");

            var session = CreateSession("901", "Dr House", "Leeds", string.Empty);

            session.Slots = new List<Slot> { slot, slotWithInvalidStartTime };

            // Act
            var actualResponse = _systemUnderTest.Map(new[] { session });

            // Assert
            var expectedSlot = new Worker.Areas.Appointments.Models.Slot
            {
                Id = "901",
                Clinicians = new[] { "Dr House" },
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Location = "Leeds",
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-07-12T10:59:19"),
                Type = "Emergency"
            };
            var expectedResponse = new[] { expectedSlot };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        private static Worker.GpSystems.Suppliers.Tpp.Models.Appointments.Session CreateSession(
            string sessionId, 
            string staffDetails, 
            string location, 
            string type) => new Worker.GpSystems.Suppliers.Tpp.Models.Appointments.Session
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
