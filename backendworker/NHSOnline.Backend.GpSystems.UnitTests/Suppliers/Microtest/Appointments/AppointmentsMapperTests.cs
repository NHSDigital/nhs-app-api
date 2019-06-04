using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
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
                    DateTimeHelper.DateTimeToJson(_twoDaysFromNow), null, "Emergency");

            var appointments = new[] { appointment };

            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset(_twoDaysFromNow);

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedAppointment = new UpcomingAppointment
            {
                Id = "101",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = slotTime,
                Location = "",
                StartTime = (DateTimeOffset) slotTime,
                Type = "Emergency",
                SessionName = ""
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
                    invalidEndTime, "Leeds", "Emergency");

            var appointments = new[] { appointment };

            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset(_tomorrow);

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedAppointment = new UpcomingAppointment
            {
                Id = "101",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = null,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTime,
                Type = "Emergency",
                SessionName = ""
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
                    DateTimeHelper.DateTimeToJson(_twoDaysFromNow), "Leeds", "Emergency");

            var appointment2 =
                CreateAppointment("101", new[] { "Dr Zoidberg" }, DateTimeHelper.DateTimeToJson(_twoDaysFromNow),
                    DateTimeHelper.DateTimeToJson(_twoDaysFromNow), "Leeds", "Emergency");

            var appointments = new[] { appointmentWithInvalidStartTime, appointment2 };

            var slotTimeTwoDays = _dateTimeOffsetProviderMock.MockDateTimeOffset(_twoDaysFromNow);

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedAppointment = new UpcomingAppointment
            {
                Id = "101",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = slotTimeTwoDays,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeTwoDays,
                Type = "Emergency",
                SessionName = ""
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
                    DateTimeHelper.DateTimeToJson(_tomorrow), "Leeds", "Emergency");

            var appointment2 =
                CreateAppointment("102", new[] { "Dr Zoidberg" }, DateTimeHelper.DateTimeToJson(_twoDaysFromNow),
                    DateTimeHelper.DateTimeToJson(_twoDaysFromNow), "Leeds", "Emergency");

            var appointment3 =
                CreateAppointment("103", new[] { "Dr Zoidberg" }, DateTimeHelper.DateTimeToJson(_nextMonth),
                    DateTimeHelper.DateTimeToJson(_nextMonth), "Leeds", null);

            var appointment4 =
                CreateAppointment("104", new[] { "Dr Zoidberg" }, DateTimeHelper.DateTimeToJson(_today),
                    DateTimeHelper.DateTimeToJson(_today), "Leeds", "Emergency");

            var appointment5 =
                CreateAppointment("105", new[] { "Dr Zoidberg" }, DateTimeHelper.DateTimeToJson(_lastMonth),
                    DateTimeHelper.DateTimeToJson(_lastMonth), "Leeds", null);
            
            var appointments = new[] { appointment1, appointment2, appointment3, appointment4, appointment5 };

            var slotTimeTwoDays = _dateTimeOffsetProviderMock.MockDateTimeOffset(_twoDaysFromNow);
            var slotTimeTomorrow = _dateTimeOffsetProviderMock.MockDateTimeOffset(_tomorrow);
            var slotTimeNextMonth = _dateTimeOffsetProviderMock.MockDateTimeOffset(_nextMonth);
            var slotTimeToday = _dateTimeOffsetProviderMock.MockDateTimeOffset(_today);
            var slotTimeLastMonth = _dateTimeOffsetProviderMock.MockDateTimeOffset(_lastMonth);

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedAppointment1 = new UpcomingAppointment
            {
                Id = "101",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = slotTimeTomorrow,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeTomorrow,
                Type = "Emergency",
                SessionName = ""
            };

            var expectedAppointment2 = new UpcomingAppointment
            {
                Id = "102",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = slotTimeTwoDays,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeTwoDays,
                Type = "Emergency",
                SessionName = ""
            };

            var expectedAppointment3 = new UpcomingAppointment
            {
                Id = "103",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = slotTimeNextMonth,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeNextMonth,
                Type = string.Empty,
                SessionName = ""
            };

            var expectedAppointment4 = new PastAppointment
            {
                Id = "104",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = slotTimeToday,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeToday,
                Type = "Emergency",
                SessionName = ""
            };

            var expectedAppointment5 = new PastAppointment
            {
                Id = "105",
                Clinicians = new[] { "Dr Zoidberg" },
                EndTime = slotTimeLastMonth,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTimeLastMonth,
                Type = string.Empty,
                SessionName = ""
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
                    DateTimeHelper.DateTimeToJson(_tomorrow), "Leeds", "Emergency");

            var slotSessions = new[] { appointment };

            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset(_tomorrow);

            // Act
            var actualResponse = _systemUnderTest.Map(slotSessions);

            // Assert
            var expectedAppointment = new UpcomingAppointment
            {
                Id = "101",
                Clinicians = null,
                EndTime = slotTime,
                Location = "Leeds",
                StartTime = (DateTimeOffset) slotTime,
                Type = "Emergency",
                SessionName = ""
            };

            var expectedResponse = new[] { expectedAppointment };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        private static Appointment CreateAppointment(string slotId, IEnumerable<string> clinicians, string startTime,
            string endTime, string location, string type)
        {
            var appointment = new Appointment
            {
                Id = slotId,
                EndTime = endTime,
                StartTime = startTime,
                Clinicians = clinicians,
                Location = location,
                Type = type
            };

            return appointment;
        }
    }
}