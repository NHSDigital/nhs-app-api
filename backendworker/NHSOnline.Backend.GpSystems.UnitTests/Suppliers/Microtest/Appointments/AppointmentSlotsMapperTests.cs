using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;
using Slot = NHSOnline.Backend.GpSystems.Appointments.Models.Slot;
using SlotSupplier = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments.Slot;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Appointments
{
    [TestClass]
    public class AppointmentSlotsMapperTests
    {
        private IFixture _fixture;
        private DateTime _testDate;
        private DateTime _tomorrow;
        private DateTime _today;
        private DateTime _nextMonth;
        private DateTime _twoDaysFromNow;
        private DateTime _lastMonth;
        private IAppointmentSlotsMapper _systemUnderTest;
        private Mock<IDateTimeOffsetProvider> _dateTimeOffsetProviderMock;
        private Mock<IMicrotestEnumMapper> _mockMicrotestEnumMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockMicrotestEnumMapper = _fixture.Freeze<Mock<IMicrotestEnumMapper>>();
            var logger = _fixture.Create<ILogger<AppointmentSlotsMapper>>();
            _dateTimeOffsetProviderMock = _fixture.Freeze<Mock<IDateTimeOffsetProvider>>();

            _systemUnderTest = new AppointmentSlotsMapper(_dateTimeOffsetProviderMock.Object,
                _mockMicrotestEnumMapper.Object, logger);

            _testDate = DateTime.Today;
            _tomorrow = _testDate.AddDays(1);
            _today = _testDate.AddHours(1);
            _nextMonth = _testDate.AddMonths(1);
            _twoDaysFromNow = _testDate.AddDays(2);
            _lastMonth = _testDate.AddMonths(-1);
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSlotsInResponseIsNull()
        {
            // Act
            var actualResponse = _systemUnderTest.Map(null);

            // Assert
            actualResponse.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_ReturnsNoLocation_WhenLocationsInResponseIsNull()
        {
            // Arrange
            var slot =
                CreateSlot("101", new[] { "Dr Venkman" }, DateTimeHelper.DateTimeToJson(_twoDaysFromNow),
                    DateTimeHelper.DateTimeToJson(_twoDaysFromNow), null, "Emergency", "Unknown");

            var slots = new[] { slot };

            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset(_twoDaysFromNow);

            // Act
            var actualResponse = _systemUnderTest.Map(slots);

            // Assert
            var expectedSlot = new Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr Venkman" },
                EndTime = slotTime,
                Location = "",
                StartTime = (DateTimeOffset) slotTime,
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown
            };
            var expectedResponse = new[] { expectedSlot };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSlotsInResponseIsEmpty()
        {
            // Arrange
            var slots = Array.Empty<SlotSupplier>();

            // Act
            var actualResponse = _systemUnderTest.Map(slots);

            // Assert
            actualResponse.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("2018-05-09T9:59:19")]
        public void Map_ReturnsResponseWithoutEndTime_WhenEndTimeInSlotIsInInvalidFormat(string invalidEndTime)
        {
            // Arrange
            var slot =
                CreateSlot("101", new[] { "Dr Venkman" }, DateTimeHelper.DateTimeToJson(_tomorrow),
                    invalidEndTime, "Leeds", "Emergency", "Unknown");

            var slots = new[] { slot };

            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset(_tomorrow);

            // Act
            var actualResponse = _systemUnderTest.Map(slots);

            // Assert
            var expectedSlot = new Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr Venkman" },
                EndTime = null,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTime,
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown
            };
            var expectedResponse = new[] { expectedSlot };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("2018-05-09T9:59:19")]
        public void Map_ReturnsResponseWithoutSlot_WhenStartTimeInSlotIsInInvalidFormat(string invalidStartTime)
        {
            // Arrange
            var slotWithInvalidStartTime =
                CreateSlot("101", new[] { "Dr Venkman" }, invalidStartTime,
                    DateTimeHelper.DateTimeToJson(_twoDaysFromNow), "Leeds", "Emergency", "Unknown");

            var slot2 =
                CreateSlot("101", new[] { "Dr Venkman" }, DateTimeHelper.DateTimeToJson(_twoDaysFromNow),
                    DateTimeHelper.DateTimeToJson(_twoDaysFromNow), "Leeds", "Emergency", "Unknown");

            var slots = new[] { slotWithInvalidStartTime, slot2 };

            var slotTimeTwoDays = _dateTimeOffsetProviderMock.MockDateTimeOffset(_twoDaysFromNow);

            // Act
            var actualResponse = _systemUnderTest.Map(slots);

            // Assert
            var expectedSlot = new Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr Venkman" },
                EndTime = slotTimeTwoDays,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeTwoDays,
                Type = "Emergency",
                SessionName = ""
            };

            var expectedResponse = new[] { expectedSlot };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_HappyPath_ReturnsAnEnumerableOfPastAndSlots()
        {
            // Arrange
            var slot1 =
                CreateSlot("101", new[] { "Dr Venkman" }, DateTimeHelper.DateTimeToJson(_tomorrow),
                    DateTimeHelper.DateTimeToJson(_tomorrow), "Leeds", "Emergency", "Unknown");

            var slot2 =
                CreateSlot("102", new[] { "Dr Venkman" }, DateTimeHelper.DateTimeToJson(_twoDaysFromNow),
                    DateTimeHelper.DateTimeToJson(_twoDaysFromNow), "Leeds", "Emergency", "Unknown");

            var slot3 =
                CreateSlot("103", new[] { "Dr Venkman" }, DateTimeHelper.DateTimeToJson(_nextMonth),
                    DateTimeHelper.DateTimeToJson(_nextMonth), "Leeds", null, "Unknown");

            var slot4 =
                CreateSlot("104", new[] { "Dr Venkman" }, DateTimeHelper.DateTimeToJson(_today),
                    DateTimeHelper.DateTimeToJson(_today), "Leeds", "Emergency", "Unknown");

            var slot5 =
                CreateSlot("105", new[] { "Dr Venkman" }, DateTimeHelper.DateTimeToJson(_lastMonth),
                    DateTimeHelper.DateTimeToJson(_lastMonth), "Leeds", null, "Unknown");

            var slots = new[] { slot1, slot2, slot3, slot4, slot5 };

            var slotTimeTwoDays = _dateTimeOffsetProviderMock.MockDateTimeOffset(_twoDaysFromNow);
            var slotTimeTomorrow = _dateTimeOffsetProviderMock.MockDateTimeOffset(_tomorrow);
            var slotTimeNextMonth = _dateTimeOffsetProviderMock.MockDateTimeOffset(_nextMonth);
            var slotTimeToday = _dateTimeOffsetProviderMock.MockDateTimeOffset(_today);
            var slotTimeLastMonth = _dateTimeOffsetProviderMock.MockDateTimeOffset(_lastMonth);

            // Act
            var actualResponse = _systemUnderTest.Map(slots);

            // Assert
            var expectedSlot1 = new Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr Venkman" },
                EndTime = slotTimeTomorrow,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeTomorrow,
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown
            };

            var expectedSlot2 = new Slot
            {
                Id = "102",
                Clinicians = new[] { "Dr Venkman" },
                EndTime = slotTimeTwoDays,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeTwoDays,
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown
            };

            var expectedSlot3 = new Slot
            {
                Id = "103",
                Clinicians = new[] { "Dr Venkman" },
                EndTime = slotTimeNextMonth,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeNextMonth,
                Type = string.Empty,
                SessionName = "",
                Channel = Channel.Unknown
            };

            var expectedSlot4 = new Slot
            {
                Id = "104",
                Clinicians = new[] { "Dr Venkman" },
                EndTime = slotTimeToday,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeToday,
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown
            };

            var expectedSlot5 = new Slot
            {
                Id = "105",
                Clinicians = new[] { "Dr Venkman" },
                EndTime = slotTimeLastMonth,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeLastMonth,
                Type = string.Empty,
                SessionName = "",
                Channel = Channel.Unknown
            };

            var expectedResponse = new[]
            {
                expectedSlot1, expectedSlot2, expectedSlot3, expectedSlot4,
                expectedSlot5
            };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ReturnsNoClinicians_WhenNoneMatched()
        {
            // Arrange
            var slot =
                CreateSlot("101", null, DateTimeHelper.DateTimeToJson(_tomorrow),
                    DateTimeHelper.DateTimeToJson(_tomorrow), "Leeds", "Emergency", "Unknown");

            var slotSessions = new[] { slot };

            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset(_tomorrow);

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions);

            // Assert
            var expectedSlot = new Slot
            {
                Id = "101",
                Clinicians = null,
                EndTime = slotTime,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTime,
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown
            };

            var expectedResponse = new[] { expectedSlot };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [DataTestMethod]
        [DataRow("telephone", Channel.Telephone)]
        [DataRow("unknown", Channel.Unknown)]
        public void Map_ReturnsChannelObtainedFromMicrotestEnumMapper(string inputSlotTypeStatus,
            Channel expectedOutputChannel)
        {
            // Arrange
            var slot =
                CreateSlot("101", null, DateTimeHelper.DateTimeToJson(_tomorrow),
                    DateTimeHelper.DateTimeToJson(_tomorrow), "Leeds", "Emergency", inputSlotTypeStatus);
            var slotSessions = new[] { slot };

            _dateTimeOffsetProviderMock.MockDateTimeOffset(_tomorrow);

            _mockMicrotestEnumMapper.Setup(x => x.MapChannel(inputSlotTypeStatus, Channel.Unknown))
                .Returns(expectedOutputChannel);

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions);

            // Assert
            actualResponse.Single().Channel.Should().Be(expectedOutputChannel);
        }

        private static SlotSupplier CreateSlot(string slotId, IEnumerable<string> clinicians, string startTime,
            string endTime, string location, string type, string channel)
        {
            var slot = new SlotSupplier
            {
                Id = slotId,
                EndTime = endTime,
                StartTime = startTime,
                Clinicians = clinicians,
                Location = location,
                Type = type,
                Channel = channel
            };

            return slot;
        }
    }
}