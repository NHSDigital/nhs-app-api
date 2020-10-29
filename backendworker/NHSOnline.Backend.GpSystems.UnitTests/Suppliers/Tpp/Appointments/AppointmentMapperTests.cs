using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Support.Temporal;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using Appointment = NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Appointment;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class AppointmentMapperTests
    {
        private Mock<IDateTimeOffsetProvider> _dateTimeOffsetProvider;
        private AppointmentMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _dateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            _systemUnderTest = new AppointmentMapper(_dateTimeOffsetProvider.Object);
        }

        [TestMethod]
        public void Map_HappyPath_ReturnsAnArrayOfAppointments()
        {
            SetupCurrentTime(new DateTimeOffset(2020, 10, 24, 15, 0, 0, TimeSpan.FromHours(1)));
            SetupTryCreateDateTimeOffset("2020-10-25T14:20:00.0", new DateTimeOffset(2020, 10, 25, 14, 20, 00, 0, TimeSpan.Zero));
            SetupTryCreateDateTimeOffset("2020-10-25T14:30:00.0", new DateTimeOffset(2020, 10, 25, 14, 30, 00, 0, TimeSpan.Zero));
            SetupTryCreateDateTimeOffset("2020-10-25T14:40:00.0", new DateTimeOffset(2020, 10, 25, 14, 40, 00, 0, TimeSpan.Zero));
            SetupTryCreateDateTimeOffset("2020-10-25T14:50:00.0", new DateTimeOffset(2020, 10, 25, 14, 50, 00, 0, TimeSpan.Zero));
            SetupTryCreateDateTimeOffset("2020-10-25T15:20:00.0", new DateTimeOffset(2020, 10, 25, 15, 20, 00, 0, TimeSpan.Zero));
            SetupTryCreateDateTimeOffset("2020-10-25T15:30:00.0", new DateTimeOffset(2020, 10, 25, 15, 30, 00, 0, TimeSpan.Zero));

            var appointment1 = CreateAppointment("0547d0000", "2020-10-25T14:20:00.0Z", "2020-10-25T14:30:00.0Z", "Clinician: Dr House", "The Frankenstein Place");
            var appointment2 = CreateAppointment("0647d0000", "2020-10-25T14:40:00.0Z", "2020-10-25T14:50:00.0Z", "Clinician: Dr House", "The Frankenstein Place");
            var appointment3 = CreateAppointment("0747d0000", "2020-10-25T15:20:00.0Z", "2020-10-25T15:30:00.0Z", "Clinician: Dr House", "The Frankenstein Place");

            var appointments = new[] { appointment1, appointment2, appointment3 }.ToList();

            var actualResponse = _systemUnderTest.Map(appointments);

            var expectedResponse = new[] 
            {
                new UpcomingAppointment
                    {
                        Id = "0547d0000",
                        Type =  "Clinician: Dr House",
                        EndTime = new DateTimeOffset(2020, 10, 25, 14, 30, 00, TimeSpan.Zero),
                        Location = "The Frankenstein Place",
                        StartTime = new DateTimeOffset(2020, 10, 25, 14, 20, 00, TimeSpan.Zero),
                        SessionName = string.Empty,
                        Clinicians = Array.Empty<string>(),
                        TelephoneNumber = string.Empty
                    },
                new UpcomingAppointment
                    {
                        Id = "0647d0000",
                        Type =  "Clinician: Dr House",
                        EndTime = new DateTimeOffset(2020, 10, 25, 14, 50, 00, TimeSpan.Zero),
                        Location = "The Frankenstein Place",
                        StartTime = new DateTimeOffset(2020, 10, 25, 14, 40, 00, TimeSpan.Zero),
                        SessionName = string.Empty,
                        Clinicians = Array.Empty<string>(),
                        TelephoneNumber = string.Empty
                    },
                new UpcomingAppointment
                    {
                        Id = "0747d0000",
                        Type =  "Clinician: Dr House",
                        EndTime = new DateTimeOffset(2020, 10, 25, 15, 30, 00, TimeSpan.Zero),
                        Location = "The Frankenstein Place",
                        StartTime = new DateTimeOffset(2020, 10, 25, 15, 20, 00, TimeSpan.Zero),
                        SessionName = string.Empty,
                        Clinicians = Array.Empty<string>()
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
            SetupCurrentTime(new DateTimeOffset(2020, 10, 24, 15, 0, 0, TimeSpan.FromHours(1)));
            SetupTryCreateDateTimeOffset("2020-10-25T14:20:00.0", new DateTimeOffset(2020, 10, 25, 14, 20, 00, 0, TimeSpan.Zero));
            SetupTryCreateDateTimeOffset(invalidEndTime, null);

            var appointment = CreateAppointment("0547d0000", "2020-10-25T14:20:00.0", invalidEndTime, "Clinician: Dr House", "The Frankenstein Place");

            var appointments = new[] { appointment }.ToList();

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedResponse = new[]
            {
                new UpcomingAppointment
                {
                    Id = "0547d0000",
                    Type =  "Clinician: Dr House",
                    EndTime = null,
                    Location = "The Frankenstein Place",
                    StartTime = new DateTimeOffset(2020, 10, 25, 14, 20, 00, TimeSpan.Zero),
                    SessionName = string.Empty,
                    Clinicians = Array.Empty<string>()
                }
            };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_AppointmentsAreAppropriateConcreteTypeBasedOnStartTime()
        {
            SetupCurrentTime(new DateTimeOffset(2020, 10, 24, 15, 0, 0, TimeSpan.FromHours(1)));
            SetupTryCreateDateTimeOffset("2020-10-23T14:20:00.0", new DateTimeOffset(2020, 10, 23, 14, 20, 00, 0, TimeSpan.FromHours(1)));
            SetupTryCreateDateTimeOffset("2020-10-23T14:30:00.0", new DateTimeOffset(2020, 10, 23, 14, 30, 00, 0, TimeSpan.FromHours(1)));
            SetupTryCreateDateTimeOffset("2020-10-25T14:20:00.0", new DateTimeOffset(2020, 10, 25, 14, 20, 00, 0, TimeSpan.Zero));
            SetupTryCreateDateTimeOffset("2020-10-25T14:30:00.0", new DateTimeOffset(2020, 10, 25, 14, 30, 00, 0, TimeSpan.Zero));

            var appointment1 = CreateAppointment("0547d0000", "2020-10-23T14:20:00.0", "2020-10-23T14:30:00.0", "Clinician: Dr House", "The Frankenstein Place");
            var appointment2 = CreateAppointment("0647d0000", "2020-10-25T14:20:00.0", "2020-10-25T14:30:00.0", "Clinician: Dr House", "The Frankenstein Place");

            var appointments = new[] { appointment1, appointment2 }.ToList();

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedResponse = new GpSystems.Appointments.Models.Appointment[]
            {
                new PastAppointment
                {
                    Id = "0547d0000",
                    Type =  "Clinician: Dr House",
                    Location = "The Frankenstein Place",
                    StartTime = new DateTimeOffset(2020, 10, 23, 14, 20, 00, TimeSpan.FromHours(1)),
                    EndTime = new DateTimeOffset(2020, 10, 23, 14, 30, 00, TimeSpan.FromHours(1)),
                    SessionName = string.Empty,
                    Clinicians = Array.Empty<string>(),
                    TelephoneNumber = string.Empty
                },
                new UpcomingAppointment
                {
                    Id = "0647d0000",
                    Type =  "Clinician: Dr House",
                    Location = "The Frankenstein Place",
                    StartTime = new DateTimeOffset(2020, 10, 25, 14, 20, 00, TimeSpan.Zero),
                    EndTime = new DateTimeOffset(2020, 10, 25, 14, 30, 00, TimeSpan.Zero),
                    SessionName = string.Empty,
                    Clinicians = Array.Empty<string>()
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
            SetupCurrentTime(new DateTimeOffset(2020, 10, 24, 15, 0, 0, TimeSpan.FromHours(1)));
            SetupTryCreateDateTimeOffset(invalidStartTime, null);
            SetupTryCreateDateTimeOffset("2020-10-25T14:30:00.0", new DateTimeOffset(2020, 10, 25, 14, 30, 00, 0, TimeSpan.Zero));
            SetupTryCreateDateTimeOffset("2020-10-25T14:40:00.0", new DateTimeOffset(2020, 10, 25, 14, 40, 00, 0, TimeSpan.Zero));
            SetupTryCreateDateTimeOffset("2020-10-25T14:50:00.0", new DateTimeOffset(2020, 10, 25, 14, 50, 00, 0, TimeSpan.Zero));

            var appointment1 = CreateAppointment("0547d0000", invalidStartTime, "2020-10-25T14:30:00.0Z", "Clinician: Dr House", "The Frankenstein Place");
            var appointment2 = CreateAppointment("0647d0000", "2020-10-25T14:40:00.0Z", "2020-10-25T14:50:00.0Z", "Clinician: Dr House", "The Frankenstein Place");

            var appointments = new[] { appointment1, appointment2 }.ToList();

            // Act
            var actualResponse = _systemUnderTest.Map(appointments);

            // Assert
            var expectedResponse = new[]
            {
                new UpcomingAppointment
                {
                    Id = "0647d0000",
                    Type =  "Clinician: Dr House",
                    Location = "The Frankenstein Place",
                    StartTime = new DateTimeOffset(2020, 10, 25, 14, 40, 00, TimeSpan.Zero),
                    EndTime = new DateTimeOffset(2020, 10, 25, 14, 50, 00, TimeSpan.Zero),
                    SessionName = string.Empty,
                    Clinicians = Array.Empty<string>()
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

        private void SetupCurrentTime(DateTimeOffset currentTime)
        {
            _dateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset())
                .Returns(currentTime);
        }

        private delegate void TryCreateDateTimeOffsetCallback(string dateTime, out DateTimeOffset? dateTimeOffset);

        private void SetupTryCreateDateTimeOffset(string dateTime, DateTimeOffset? dateTimeOffset)
        {
            DateTimeOffset? temp;
            var callback = new TryCreateDateTimeOffsetCallback((string time, out DateTimeOffset? offset) => offset = dateTimeOffset);
            _dateTimeOffsetProvider
                .Setup(x => x.TryCreateDateTimeOffset(dateTime, out temp))
                .Callback(callback)
                .Returns(dateTimeOffset != null);
        }
    }
}
