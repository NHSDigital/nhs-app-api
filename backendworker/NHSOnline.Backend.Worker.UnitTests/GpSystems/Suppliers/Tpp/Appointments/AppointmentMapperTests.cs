using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class AppointmentMapperTests
    {
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        private AppointmentMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", "GMT Standard Time") });
            _timeZoneInfoProvider = new TimeZoneInfoProvider(configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
            _systemUnderTest = new AppointmentMapper(_dateTimeOffsetProvider);
        }

        [TestMethod]
        public void Map_HappyPath_ReturnsAnArrayOfAppointments()
        {
            var appointment1 = CreateAppointment("0547d0000", "2018-07-18T14:20:00.0Z", "2018-07-18T14:30:00.0Z", "Clinician: Dr House", "The Frankenstein Place");
            var appointment2 = CreateAppointment("0647d0000", "2018-07-18T14:40:00.0Z", "2018-07-18T14:50:00.0Z", "Clinician: Dr House", "The Frankenstein Place");
            var appointment3 = CreateAppointment("0747d0000", "2018-07-18T15:20:00.0Z", "2018-07-18T15:30:00.0Z", "Clinician: Dr House", "The Frankenstein Place");

            var appointments = new[] { appointment1, appointment2, appointment3 }.ToList();

            var actualResponse = _systemUnderTest.Map(appointments);

            var expectedResponse = new[] 
            {
                new Worker.Areas.Appointments.Models.Appointment
                    {
                        Id = "0547d0000",
                        Type =  "Clinician: Dr House",
                        EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-18T14:30:00.0"),
                        Location = "The Frankenstein Place",
                        StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-18T14:20:00.0")
                    },
                new Worker.Areas.Appointments.Models.Appointment
                    {
                        Id = "0647d0000",
                        Type =  "Clinician: Dr House",
                        EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-18T14:50:00.0"),
                        Location = "The Frankenstein Place",
                        StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-18T14:40:00.0")
                    },
                new Worker.Areas.Appointments.Models.Appointment
                    {
                        Id = "0747d0000",
                        Type =  "Clinician: Dr House",
                        EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-18T15:30:00.0"),
                        Location = "The Frankenstein Place",
                        StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-18T15:20:00.0")
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
            var appointment =
                CreateAppointment("0547d0000", "2018-07-18T14:20:00.0Z", invalidEndTime, "Clinician: Dr House", "The Frankenstein Place");

            var appointments = new[] { appointment }.ToList();

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedResponse = new[]
            {
                new NHSOnline.Backend.Worker.Areas.Appointments.Models.Appointment
                {
                    Id = "0547d0000",
                    Type =  "Clinician: Dr House",
                    EndTime = null,
                    Location = "The Frankenstein Place",
                    StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-18T14:20:00.0")
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
            // Arrange
            var appointment1 =
                CreateAppointment("0547d0000", invalidStartTime, "2018-07-18T14:20:00.0Z", "Clinician: Dr House", "The Frankenstein Place");

            var appointment2 =
                CreateAppointment("0647d0000", "2018-07-18T14:20:00.0Z", "2018-07-18T14:30:00.0Z", "Clinician: Dr House", "The Frankenstein Place");

            var appointments = new[] { appointment1, appointment2 }.ToList();

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedResponse = new[]
            {
                new NHSOnline.Backend.Worker.Areas.Appointments.Models.Appointment
                {
                    Id = "0647d0000",
                    Type =  "Clinician: Dr House",
                    Location = "The Frankenstein Place",
                    StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-18T14:20:00.0"),
                    EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-07-18T14:30:00.0")
                }
            };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
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
}
