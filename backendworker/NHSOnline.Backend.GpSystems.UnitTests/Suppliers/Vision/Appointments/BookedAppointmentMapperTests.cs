using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Appointments
{
    [TestClass]
    public class AppointmentMapperTests
    {
        private IFixture _fixture;
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        private BookedAppointmentMapper _systemUnderTest;
        private Location _location1;
        private Location _location2;
        private List<Location> _locations;
        private SlotSession _generalSession;
        private List<SlotSession> _sessions;
        private SlotType _slotTypeWithDescription;
        private SlotType _slotTypeWithoutDescription;
        private List<SlotType> _slotTypes;
        private Owner _owner1;
        private Owner _owner2;
        private List<Owner> _owners;
        private Mock<ICurrentDateTimeProvider> _mockCurrentDateTimeProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _mockCurrentDateTimeProvider = _fixture.Freeze<Mock<ICurrentDateTimeProvider>>();
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(DateTime.UtcNow);

            IConfigurationBuilder configBuilder = new ConfigurationBuilder();

            configBuilder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("TIMEZONE",
                    TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform())
            });
            _timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object,
                configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);

            _location1 = _fixture.Create<Location>();
            _location2 = _fixture.Create<Location>();
            _locations = new[] { _location1, _location2 }.ToList();

            _generalSession = _fixture.Build<SlotSession>().With(x => x.Location, _location1.Id).Create();
            _sessions = new[] { _generalSession }.ToList();

            _slotTypeWithDescription = _fixture.Create<SlotType>();
            _slotTypeWithoutDescription = _fixture.Build<SlotType>().With(x => x.Description, "").Create();
            _slotTypes = new[] { _slotTypeWithDescription, _slotTypeWithoutDescription }.ToList();

            _owner1 = _fixture.Create<Owner>();
            _owner2 = _fixture.Create<Owner>();
            _owners = new[] { _owner1, _owner2 }.ToList();

            _systemUnderTest = new BookedAppointmentMapper(_dateTimeOffsetProvider);
        }

        [TestMethod]
        public void Map_HappyPath_ReturnsAnArrayOfAppointments()
        {
            var slotTime1 = new SlotTime(Tomorrow("14:20"), Tomorrow("15:00"));
            var slotTime2 = new SlotTime(Tomorrow("14:40"), Tomorrow("14:55"));

            var slot1 = new BookedSlot
            {
                DateTime = slotTime1.Start.ToVisionDateTimeString(),
                Id = "SLOTX01",
                Location = _location1.Id,
                Session = _generalSession.Id,
                Type = _slotTypeWithDescription.Id, 
                Owner = _owner1.Id,
                Duration = slotTime1.Duration
            };

            var slot2 = new BookedSlot
            {
                DateTime = slotTime2.Start.ToVisionDateTimeString(),
                Id = "SLOTX02",
                Location = _location2.Id,
                Session = _generalSession.Id,
                Type = _slotTypeWithoutDescription.Id,
                Owner = _owner2.Id,
                Duration = slotTime2.Duration
            };

            var slots = new[] { slot1, slot2 }.ToList();

            var bookedAppointmentsResponse =
                CreateBookedAppointmentsResponse(slots, _locations, _sessions, _slotTypes, _owners);
            
            var actualResponse = _systemUnderTest.Map(bookedAppointmentsResponse.Appointments);
            
            var expectedResponse = new[] 
            {
                new UpcomingAppointment
                {
                    Id = "SLOTX01",
                    Type =  $"{_generalSession.Description} - {_slotTypeWithDescription.Description}",
                    Location = _location1.Name,
                    StartTime = slotTime1.Start, 
                    EndTime = slotTime1.End,
                    Clinicians = new []{ _owner1.Name }
                },
                new UpcomingAppointment
                {
                    Id = "SLOTX02",
                    Type =  $"{_generalSession.Description}",
                    Location = _location2.Name,
                    StartTime = slotTime2.Start,
                    EndTime = slotTime2.End,
                    Clinicians = new []{ _owner2.Name }
                }
            };

            actualResponse.Should().BeEquivalentTo(expectedResponse.ToList());
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSlotsInResponseIsEmpty()
        {
            // Arrange
            var bookedAppointmentsResponse = CreateBookedAppointmentsResponse(
                    new List<BookedSlot>(),
                    new List<Location>(),
                    new List<SlotSession>(),
                    new List<SlotType>(),
                    new List<Owner>()
                );

            // Act
            var actualResponse = _systemUnderTest.Map(bookedAppointmentsResponse.Appointments);

            // Assert
            actualResponse.Should().BeEmpty();
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSlotsInResponseIsNull()
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
        public void Map_ReturnsResponseWithoutSlotsAppointment_WhenStartTimeInSlotsAppointmentIsInInvalidFormat(
            string invalidStartTime)
        {
            var slotTime2 = new SlotTime(Tomorrow("14:40"), Tomorrow("15:05"));

            var slot1 = new BookedSlot
            {
                DateTime = invalidStartTime,
                Id = "SLOTX01",
                Location = _location1.Id,
                Session = _generalSession.Id,
                Type = _slotTypeWithDescription.Id, 
                Owner = _owner1.Id
            };
            
            var slot2 = new BookedSlot
            {
                DateTime = slotTime2.Start.ToVisionDateTimeString(),
                Id = "SLOTX02",
                Location = _location2.Id,
                Session = _generalSession.Id,
                Type = _slotTypeWithoutDescription.Id,
                Owner = _owner2.Id,
                Duration = slotTime2.Duration
            };

            var slots = new[] { slot1, slot2 }.ToList();

            var bookedAppointmentsResponse =
                CreateBookedAppointmentsResponse(slots, _locations, _sessions, _slotTypes, _owners);
            
            // Act
            var actualResponse = _systemUnderTest.Map(bookedAppointmentsResponse.Appointments);

            // Assert
            var expectedResponse = new[] 
            {
                new UpcomingAppointment
                {
                    Id = "SLOTX02",
                    Type =  _generalSession.Description,
                    Location = _location2.Name,
                    StartTime = slotTime2.Start,
                    EndTime = slotTime2.End,
                    Clinicians = new[] { _owner2.Name }
                }
            };

            actualResponse.Should().BeEquivalentTo(expectedResponse.ToList());
        }

        [TestMethod]
        public void Map_AppointmentWithinCutOffTime_DisableCancellationSet()
        {
            const string now = "2018-12-25T15:15:01";
            const int cutOffMinutes = 15;
            const string cancellable = "2018-12-25T15:40:00";
            const string nonCancellable = "2018-12-25T15:30:00";

            var currentTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(now);
            DateTimeOffset? noncancellableDateTimeOffset = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(nonCancellable);
            DateTimeOffset? cancellableDateTimeOffset = _dateTimeOffsetProvider.GetDateTimeOffsetForTest(cancellable);


            var slotTime1 = new SlotTime(noncancellableDateTimeOffset.Value, noncancellableDateTimeOffset.Value.AddMinutes(10));
            var slotTime2 = new SlotTime(cancellableDateTimeOffset.Value, cancellableDateTimeOffset.Value.AddMinutes(10));

            var mockDateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();
            mockDateTimeOffsetProvider.Setup(x => x.CreateDateTimeOffset()).Returns(currentTime);
            mockDateTimeOffsetProvider.Setup(x => x.TryCreateDateTimeOffset(nonCancellable, out noncancellableDateTimeOffset))
                .Returns(true);
            mockDateTimeOffsetProvider.Setup(x => x.TryCreateDateTimeOffset(cancellable, out cancellableDateTimeOffset))
                .Returns(true);

            _systemUnderTest = new BookedAppointmentMapper(mockDateTimeOffsetProvider.Object);

            var slot1 = new BookedSlot
            {
                DateTime = slotTime1.Start.ToVisionDateTimeString(),
                Id = "SLOTX01",
                Location = _location1.Id,
                Session = _generalSession.Id,
                Type = _slotTypeWithDescription.Id, 
                Owner = _owner1.Id,
                Duration = slotTime1.Duration
            };

            var slot2 = new BookedSlot
            {
                DateTime = slotTime2.Start.ToVisionDateTimeString(),
                Id = "SLOTX02",
                Location = _location2.Id,
                Session = _generalSession.Id,
                Type = _slotTypeWithoutDescription.Id,
                Owner = _owner2.Id,
                Duration = slotTime2.Duration
            };

            var slots = new[] { slot1, slot2 }.ToList();

            var bookedAppointmentsResponse =
                CreateBookedAppointmentsResponse(slots, _locations, _sessions, _slotTypes, _owners, cutOffMinutes);

            var actualResponse = _systemUnderTest.Map(bookedAppointmentsResponse.Appointments);

            var expectedResponse = new[] 
            {
                new UpcomingAppointment
                {
                    Id = "SLOTX01",
                    Type = $"{_generalSession.Description} - {_slotTypeWithDescription.Description}",
                    Location = _location1.Name,
                    StartTime = slotTime1.Start, 
                    EndTime = slotTime1.End,
                    Clinicians = new []{ _owner1.Name },
                    DisableCancellation = true
                },
                new UpcomingAppointment
                {
                    Id = "SLOTX02",
                    Type = $"{_generalSession.Description}",
                    Location = _location2.Name,
                    StartTime = slotTime2.Start,
                    EndTime = slotTime2.End,
                    Clinicians = new []{ _owner2.Name },
                    DisableCancellation = false
                }
            };

            actualResponse.Should().BeEquivalentTo(expectedResponse.ToList());
        }

        
        private DateTimeOffset Tomorrow()
        {
            return _dateTimeOffsetProvider.CreateDateTimeOffset().SetTimeToMidnight().AddDays(1);
        }

        private DateTimeOffset Tomorrow(string time)
        {
            var dateTime = Tomorrow();
            var parts = time.Split(':');

            return dateTime.AddHours(int.Parse(parts[0], Thread.CurrentThread.CurrentCulture))
                .AddMinutes(int.Parse(parts[1], Thread.CurrentThread.CurrentCulture));
        }

        private static BookedAppointmentsResponse CreateBookedAppointmentsResponse(
            List<BookedSlot> slots,
            List<Location> locations,
            List<SlotSession> sessions,
            List<SlotType> slotTypes, 
            List<Owner> owners, 
            int cutOffMinutes = 0
            )
        {
            var references = new References
            {
                Locations = locations,
                Sessions = sessions,
                SlotTypes = slotTypes, 
                Owners = owners
            };

            var settings = new SlotSettings { CancellationCutOffMinutes = cutOffMinutes };

            return new BookedAppointmentsResponse
            {
                Appointments = new BookedAppointments{
                    Slots = slots,
                    References = references,
                    Settings = settings
                }
            };
        }

        private class SlotTime
        {
            public SlotTime(DateTimeOffset start, DateTimeOffset end)
            {
                Start = start;
                End = end;
            }

            public DateTimeOffset Start { get; }
            public DateTimeOffset End { get; }
            
            public string Duration => (End - Start).Minutes.ToString(CultureInfo.InvariantCulture);
        }
    }
}