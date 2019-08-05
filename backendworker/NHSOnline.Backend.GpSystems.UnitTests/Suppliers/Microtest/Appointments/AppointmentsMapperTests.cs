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
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;
using Appointment = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments.Appointment;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Appointments
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
        private Mock<IDateTimeOffsetProvider> _dateTimeOffsetProviderMock;
        private Mock<IMicrotestEnumMapper> _mockMicrotestEnumMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("TIMEZONE",
                    TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform())
            });

            _dateTimeOffsetProviderMock = _fixture.Freeze<Mock<IDateTimeOffsetProvider>>();
            _mockMicrotestEnumMapper = _fixture.Freeze<Mock<IMicrotestEnumMapper>>();

            var logger = _fixture.Create<ILoggerFactory>().CreateLogger<AppointmentsMapper>();

            _fixture.Inject(_dateTimeOffsetProviderMock);

            _systemUnderTest = new AppointmentsMapper(_dateTimeOffsetProviderMock.Object,
                _mockMicrotestEnumMapper.Object, logger);

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
            // Act
            var actualResponse = _systemUnderTest.Map(null);

            // Assert
            actualResponse.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_ReturnsNoLocation_WhenLocationsInResponseIsNull()
        {
            // Arrange

            var appointment =
                CreateAppointment("101", new[] { "Dr Zoidberg" }, DateTimeHelper.DateTimeToJson(_twoDaysFromNow),
                    DateTimeHelper.DateTimeToJson(_twoDaysFromNow), null, "Emergency", "Unknown", "012345");

            var appointments = new[] { appointment };

            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset(_twoDaysFromNow).Value;

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedAppointment = new UpcomingAppointment
            {
                Id = "101",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = slotTime,
                Location = "",
                StartTime = slotTime,
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown,
                TelephoneNumber = "012345"
            };
            var expectedResponse = new[] { expectedAppointment };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenAppointmentsInResponseIsEmpty()
        {
            // Arrange
            var appointments = Array.Empty<Appointment>();

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

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
                CreateAppointment("101", new[] { "Dr Zoidberg" }, DateTimeHelper.DateTimeToJson(_tomorrow),
                    invalidEndTime, "Leeds", "Emergency", "Unknown", "012345");

            var appointments = new[] { appointment };

            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset(_tomorrow).Value;

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedAppointment = new UpcomingAppointment
            {
                Id = "101",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = null,
                Location = "Leeds",
                StartTime = slotTime,
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown,
                TelephoneNumber = "012345"
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
                CreateAppointment("101", new[] { "Dr Zoidberg" }, invalidStartTime,
                    DateTimeHelper.DateTimeToJson(_twoDaysFromNow), "Leeds", "Emergency", "Unknown", "012345");

            var appointment2 =
                CreateAppointment("101", new[] { "Dr Zoidberg" }, DateTimeHelper.DateTimeToJson(_twoDaysFromNow),
                    DateTimeHelper.DateTimeToJson(_twoDaysFromNow), "Leeds", "Emergency", "Unknown", "012345");

            var appointments = new[] { appointmentWithInvalidStartTime, appointment2 };

            var slotTimeTwoDays = _dateTimeOffsetProviderMock.MockDateTimeOffset(_twoDaysFromNow).Value;

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedAppointment = new UpcomingAppointment
            {
                Id = "101",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = slotTimeTwoDays,
                Location = "Leeds",
                StartTime = slotTimeTwoDays,
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown,
                TelephoneNumber = "012345"
            };

            var expectedResponse = new[] { expectedAppointment };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_HappyPath_ReturnsAnEnumerableOfPastAndUpcomingAppointments()
        {
            // Arrange
            var appointment1 =
                CreateAppointment("101", new[] { "Dr Zoidberg" }, DateTimeHelper.DateTimeToJson(_tomorrow),
                    DateTimeHelper.DateTimeToJson(_tomorrow), "Leeds", "Emergency", "Unknown", "012345");

            var appointment2 =
                CreateAppointment("102", new[] { "Dr Zoidberg" }, DateTimeHelper.DateTimeToJson(_twoDaysFromNow),
                    DateTimeHelper.DateTimeToJson(_twoDaysFromNow), "Leeds", "Emergency", "Unknown", "012345");

            var appointment3 =
                CreateAppointment("103", new[] { "Dr Zoidberg" }, DateTimeHelper.DateTimeToJson(_nextMonth),
                    DateTimeHelper.DateTimeToJson(_nextMonth), "Leeds", null, "Unknown", null);

            var appointment4 =
                CreateAppointment("104", new[] { "Dr Zoidberg" }, DateTimeHelper.DateTimeToJson(_today),
                    DateTimeHelper.DateTimeToJson(_today), "Leeds", "Emergency", "Unknown", "");

            var appointment5 =
                CreateAppointment("105", new[] { "Dr Zoidberg" }, DateTimeHelper.DateTimeToJson(_lastMonth),
                    DateTimeHelper.DateTimeToJson(_lastMonth), "Leeds", null, "Unknown", "012345");

            var appointments = new[] { appointment1, appointment2, appointment3, appointment4, appointment5 };

            var slotTimeTwoDays = _dateTimeOffsetProviderMock.MockDateTimeOffset(_twoDaysFromNow).Value;
            var slotTimeTomorrow = _dateTimeOffsetProviderMock.MockDateTimeOffset(_tomorrow).Value;
            var slotTimeNextMonth = _dateTimeOffsetProviderMock.MockDateTimeOffset(_nextMonth).Value;
            var slotTimeToday = _dateTimeOffsetProviderMock.MockDateTimeOffset(_today).Value;
            var slotTimeLastMonth = _dateTimeOffsetProviderMock.MockDateTimeOffset(_lastMonth).Value;

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedAppointment1 = new UpcomingAppointment
            {
                Id = "101",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = slotTimeTomorrow,
                Location = "Leeds",
                StartTime = slotTimeTomorrow,
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown,
                TelephoneNumber = "012345"
            };

            var expectedAppointment2 = new UpcomingAppointment
            {
                Id = "102",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = slotTimeTwoDays,
                Location = "Leeds",
                StartTime = slotTimeTwoDays,
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown,
                TelephoneNumber = "012345"
            };

            var expectedAppointment3 = new UpcomingAppointment
            {
                Id = "103",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = slotTimeNextMonth,
                Location = "Leeds",
                StartTime = slotTimeNextMonth,
                Type = string.Empty,
                SessionName = "",
                Channel = Channel.Unknown,
                TelephoneNumber = ""
            };

            var expectedAppointment4 = new PastAppointment
            {
                Id = "104",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = slotTimeToday,
                Location = "Leeds",
                StartTime = slotTimeToday,
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown,
                TelephoneNumber = ""
            };

            var expectedAppointment5 = new PastAppointment
            {
                Id = "105",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = slotTimeLastMonth,
                Location = "Leeds",
                StartTime = slotTimeLastMonth,
                Type = string.Empty,
                SessionName = "",
                Channel = Channel.Unknown,
                TelephoneNumber = "012345"
            };

            var expectedResponse = new NHSOnline.Backend.GpSystems.Appointments.Models.Appointment[]
            {
                expectedAppointment1, expectedAppointment2, expectedAppointment3, expectedAppointment4,
                expectedAppointment5
            };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ReturnsNoClinicians_WhenNoneMatched()
        {
            // Arrange
            var appointment =
                CreateAppointment("101", null, DateTimeHelper.DateTimeToJson(_tomorrow),
                    DateTimeHelper.DateTimeToJson(_tomorrow), "Leeds", "Emergency", "Unknown", "012345");

            var slotSessions = new[] { appointment };

            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset(_tomorrow).Value;

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions);

            // Assert
            var expectedAppointment = new UpcomingAppointment
            {
                Id = "101",
                Clinicians = null,
                EndTime = slotTime,
                Location = "Leeds",
                StartTime = slotTime,
                Type = "Emergency",
                SessionName = "",
                Channel = Channel.Unknown,
                TelephoneNumber = "012345"
            };

            var expectedResponse = new[] { expectedAppointment };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [DataTestMethod]
        [DataRow("telephone", Channel.Telephone)]
        [DataRow("unknown", Channel.Unknown)]
        public void Map_ReturnsChannelObtainedFromMicrotestEnumMapper(string inputSlotTypeStatus,
            Channel expectedOutputChannel)
        {
            // Arrange
            var appointment =
                CreateAppointment("101", null, DateTimeHelper.DateTimeToJson(_tomorrow),
                    DateTimeHelper.DateTimeToJson(_tomorrow), "Leeds", "Emergency", inputSlotTypeStatus, "012345");

            var slotSessions = new[] { appointment };

            _dateTimeOffsetProviderMock.MockDateTimeOffset(_tomorrow);

            _mockMicrotestEnumMapper.Setup(x => x.MapChannel(inputSlotTypeStatus, Channel.Unknown))
                .Returns(expectedOutputChannel);

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions);

            // Assert
            actualResponse.Single().Channel.Should().Be(expectedOutputChannel);
        }

        private static Appointment CreateAppointment(string slotId, IEnumerable<string> clinicians, string startTime,
            string endTime, string location, string type, string channel, string telephoneNumber)
        {
            var appointment = new Appointment
            {
                Id = slotId,
                EndTime = endTime,
                StartTime = startTime,
                Clinicians = clinicians,
                Location = location,
                Type = type,
                Channel = channel,
                TelephoneNumber = telephoneNumber
            };

            return appointment;
        }
    }
}