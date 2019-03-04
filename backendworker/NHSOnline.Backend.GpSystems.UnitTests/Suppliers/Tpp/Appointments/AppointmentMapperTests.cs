using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Support.Temporal;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class AppointmentMapperTests
    {
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        private ICurrentDateTimeProvider _currentTimeProvider;
        private AppointmentMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform() )});
            _timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            _currentTimeProvider = new CurrentDateTimeProvider(_timeZoneInfoProvider);
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _currentTimeProvider);
            _systemUnderTest = new AppointmentMapper(_dateTimeOffsetProvider);
        }

        [TestMethod]
        public void Map_HappyPath_ReturnsAnArrayOfAppointments()
        {
            var appointmentTime1 = new AppointmentTime(Tomorrow().At("14:20"));
            var appointmentTime2 = new AppointmentTime(Tomorrow().At("14:40"));
            var appointmentTime3 = new AppointmentTime(Tomorrow().At("15:20"));

            var appointment1 = CreateAppointment("0547d0000",appointmentTime1.Start.AsTppDateTimeString(), appointmentTime1.End.AsTppDateTimeString(), "Clinician: Dr House", "The Frankenstein Place");
            var appointment2 = CreateAppointment("0647d0000", appointmentTime2.Start.AsTppDateTimeString(), appointmentTime2.End.AsTppDateTimeString(), "Clinician: Dr House", "The Frankenstein Place");
            var appointment3 = CreateAppointment("0747d0000", appointmentTime3.Start.AsTppDateTimeString(), appointmentTime3.End.AsTppDateTimeString(), "Clinician: Dr House", "The Frankenstein Place");

            var appointments = new[] { appointment1, appointment2, appointment3 }.ToList();

            var actualResponse = _systemUnderTest.Map(appointments);

            var expectedResponse = new[] 
            {
                new Backend.GpSystems.Appointments.Models.UpcomingAppointment
                    {
                        Id = "0547d0000",
                        Type =  "Clinician: Dr House",
                        EndTime = appointmentTime1.End,
                        Location = "The Frankenstein Place",
                        StartTime = appointmentTime1.Start,
                        SessionName = string.Empty
                    },
                new Backend.GpSystems.Appointments.Models.UpcomingAppointment
                    {
                        Id = "0647d0000",
                        Type =  "Clinician: Dr House",
                        EndTime = appointmentTime2.End,
                        Location = "The Frankenstein Place",
                        StartTime = appointmentTime2.Start,
                        SessionName = string.Empty
                    },
                new Backend.GpSystems.Appointments.Models.UpcomingAppointment
                    {
                        Id = "0747d0000",
                        Type =  "Clinician: Dr House",
                        EndTime = appointmentTime3.End,
                        Location = "The Frankenstein Place",
                        StartTime = appointmentTime3.Start,
                        SessionName = string.Empty
                    }
            };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenAppointmentsInResponseIsEmpty()
        {
            // Arrange
            var appointments = new List<Appointment>();

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            actualResponse.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenAppointmentsInResponseIsNull()
        {
            // Act
            var actualResponse = _systemUnderTest.Map(null);

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
            var appointmentStart = Tomorrow().At("14:40");
            var appointment =
                CreateAppointment("0547d0000", appointmentStart.AsTppDateTimeString(), invalidEndTime, "Clinician: Dr House", "The Frankenstein Place");

            var appointments = new[] { appointment }.ToList();

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedResponse = new[]
            {
                new NHSOnline.Backend.GpSystems.Appointments.Models.UpcomingAppointment
                {
                    Id = "0547d0000",
                    Type =  "Clinician: Dr House",
                    EndTime = null,
                    Location = "The Frankenstein Place",
                    StartTime = appointmentStart,
                    SessionName = string.Empty
                }
            };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_AppointmentsAreAppropriateConcreteTypeBasedOnStartTime()
        {
            var appointmentTime1 = new AppointmentTime(_dateTimeOffsetProvider.CreateDateTimeOffset().SetTimeToMidnight().AddDays(-1));
            var appointmentTime2 = new AppointmentTime(Tomorrow().At("14:20"));

            var appointment1 =
                CreateAppointment("0547d0000", appointmentTime1.Start.AsTppDateTimeString(), appointmentTime1.End.AsTppDateTimeString(), "Clinician: Dr House", "The Frankenstein Place");

            var appointment2 =
                CreateAppointment("0647d0000", appointmentTime2.Start.AsTppDateTimeString(), appointmentTime2.End.AsTppDateTimeString(), "Clinician: Dr House", "The Frankenstein Place");

            var appointments = new[] { appointment1, appointment2 }.ToList();

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedResponse = new NHSOnline.Backend.GpSystems.Appointments.Models.Appointment[]
            {
                new NHSOnline.Backend.GpSystems.Appointments.Models.PastAppointment
                {
                    Id = "0547d0000",
                    Type =  "Clinician: Dr House",
                    Location = "The Frankenstein Place",
                    StartTime = appointmentTime1.Start,
                    EndTime = appointmentTime1.End,
                    SessionName = string.Empty
                },
                new NHSOnline.Backend.GpSystems.Appointments.Models.UpcomingAppointment
                {
                    Id = "0647d0000",
                    Type =  "Clinician: Dr House",
                    Location = "The Frankenstein Place",
                    StartTime = appointmentTime2.Start,
                    EndTime = appointmentTime2.End,
                    SessionName = string.Empty
                }
            };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("2018-05-09T9:59:19")]
        public void Map_ReturnsResponseWithoutAppointment_WhenStartTimeInAppointmentIsInInvalidFormat(string invalidStartTime)
        {
            var appointmentTime1 = new AppointmentTime(Tomorrow().At("14:20"));
            var appointmentTime2 = new AppointmentTime(Tomorrow().At("14:40"));

            // Arrange
            var appointment1 =
                CreateAppointment("0547d0000", invalidStartTime, appointmentTime1.End.AsTppDateTimeString(), "Clinician: Dr House", "The Frankenstein Place");

            var appointment2 =
                CreateAppointment("0647d0000", appointmentTime2.Start.AsTppDateTimeString(), appointmentTime2.End.AsTppDateTimeString(), "Clinician: Dr House", "The Frankenstein Place");

            var appointments = new[] { appointment1, appointment2 }.ToList();

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedResponse = new[]
            {
                new NHSOnline.Backend.GpSystems.Appointments.Models.UpcomingAppointment
                {
                    Id = "0647d0000",
                    Type =  "Clinician: Dr House",
                    Location = "The Frankenstein Place",
                    StartTime = appointmentTime2.Start,
                    EndTime = appointmentTime2.End,
                    SessionName = string.Empty
                }
            };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        private DateTimeOffset Tomorrow()
        {
            return _dateTimeOffsetProvider.CreateDateTimeOffset().SetTimeToMidnight().AddDays(1);
        }
        
        private static Appointment CreateAppointment(string apptId, string startDate, string endDate, string details, string siteName) =>
            new Appointment
            {
                SiteName = siteName,
                ApptId = apptId,
                StartDate = startDate,
                EndDate = endDate,
                Details = details
            };
    }

    public class AppointmentTime
    {
        public AppointmentTime(DateTimeOffset start)
        {
            Start = start;
            End = start.AddMinutes(10);
        }
        
        public DateTimeOffset Start { get; }
        public DateTimeOffset End { get; }
    }

    public static class TppAppointmentExtensions
    {
        public static DateTimeOffset At(this DateTimeOffset dateTime, string time)
        {
            var parts = time.Split(':');

            return dateTime.AddHours(int.Parse(parts[0], Thread.CurrentThread.CurrentCulture)).AddMinutes(int.Parse(parts[1], Thread.CurrentThread.CurrentCulture));
        }
        public static string AsTppDateTimeString(this DateTimeOffset dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:00.0Z", Thread.CurrentThread.CurrentCulture);
        }
    }
}
