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
    public class AvailableAppointmentsMapperTests
    {
        private IFixture _fixture;
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        private AvailableAppointmentsMapper _systemUnderTest;
        private Mock<ILogger<AvailableAppointmentsMapper>> _mockLogger;
        private Mock<ICurrentDateTimeProvider> _mockCurrentDateTimeProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _mockCurrentDateTimeProvider = _fixture.Freeze<Mock<ICurrentDateTimeProvider>>();
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(DateTime.UtcNow);
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            _timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);
            _mockLogger = _fixture.Freeze<Mock<ILogger<AvailableAppointmentsMapper>>>();
            _systemUnderTest = new AvailableAppointmentsMapper(_dateTimeOffsetProvider, _mockLogger.Object);
        }

        [TestMethod]
        public void Map_HappyPath_ReturnsAnArrayOfAppointments()
        {
            // Arrange
            var slotTime1 = new SlotTime(Tomorrow("14:20"), Tomorrow("15:00"));
            var slotTime2 = new SlotTime(Tomorrow("14:40"), Tomorrow("14:55"));

            var leeds = new Location
            {
                Id = "LOCX01",
                Name = "Leeds"
            };
            
            var london = new Location
            {
                Id = "LOCX02",
                Name = "London"
            };

            var generalSession = new SlotSession
            {
                Id = "SESSX01",
                Description = "General Session",
                Location = leeds.Id
            };
            
            var emptySession = new SlotSession
            {
                Id = "SESSX02",
                Description = "",
                Location = leeds.Id
            };

            var generalType = new SlotType
            {
                Id = "TYPEX01",
                Description = "General Type"
            };

            var owner1 = new Owner
            {
                Id = "OWN01",
                Name = "Owner1"
            };

            var owner2 = new Owner
            {
                Id = "OWN02",
                Name = "Owner2"
            };

            var slot1 = new FreeSlot
            {
                DateTime = slotTime1.Start.ToVisionDateTimeString(),
                Id = "SLOTX01",
                Location = leeds.Id,
                Session = generalSession.Id,
                Type = generalType.Id, 
                Owner = owner1.Id,
                Duration = slotTime1.Duration
            };
            
            var slot2 = new FreeSlot
            {
                DateTime = slotTime2.Start.ToVisionDateTimeString(),
                Id = "SLOTX02",
                Location = london.Id,
                Session = emptySession.Id,
                Type = generalType.Id, 
                Owner = owner2.Id,
                Duration = slotTime2.Duration
            };

            var slots = new[] { slot1, slot2 }.ToList();
            var locations = new[] { leeds, london }.ToList();
            var sessions = new[] { generalSession, emptySession }.ToList();
            var slotTypes = new[] { generalType }.ToList();
            var owners = new[] { owner1, owner2 }.ToList();

            var availableAppointmentsResponse =
                CreateAvailableAppointmentsResponse(slots, locations, sessions, slotTypes, owners);
            
            // Act
            var actualResponse = _systemUnderTest.Map(availableAppointmentsResponse.Appointments);
            
            // Assert
            var expectedResponse = new[] 
            {
                new Slot
                {
                    Id = "SLOTX01",
                    Type =  "General Type",
                    Location = "Leeds",
                    StartTime = slotTime1.Start, 
                    EndTime = slotTime1.End,
                    Clinicians = new []{ "Owner1" },
                    Channel = Channel.Unknown,
                    SessionName = "General Session"
                },
                new Slot
                {
                    Id = "SLOTX02",
                    Type =  "General Type",
                    Location = "London",
                    StartTime = slotTime2.Start,
                    EndTime = slotTime2.End,
                    Clinicians = new []{ "Owner2" },
                    Channel = Channel.Unknown,
                    SessionName = ""
                }
            };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Map_ReturnsResponseWithoutSlot_WhenSlotTypeIsMissing()
        {
            // Arrange
            var slotTime1 = new SlotTime(Tomorrow("14:20"), Tomorrow("15:00"));
            var slotTime2 = new SlotTime(Tomorrow("14:40"), Tomorrow("14:55"));

            var leeds = new Location
            {
                Id = "LOCX01",
                Name = "Leeds"
            };
            
            var london = new Location
            {
                Id = "LOCX02",
                Name = "London"
            };

            var generalSession = new SlotSession
            {
                Id = "SESSX01",
                Description = "General Session",
                Location = leeds.Id
            };
            
            var emptySession = new SlotSession
            {
                Id = "SESSX02",
                Description = "",
                Location = leeds.Id
            };

            var generalType = new SlotType
            {
                Id = "TYPEX01",
                Description = "General Type"
            };
            
            var emptyType = new SlotType
            {
                Id = "TYPEX02",
                Description = ""
            };

            var owner1 = new Owner
            {
                Id = "OWN01",
                Name = "Owner1"
            };

            var owner2 = new Owner
            {
                Id = "OWN02",
                Name = "Owner2"
            };

            var slot1 = new FreeSlot
            {
                DateTime = slotTime1.Start.ToVisionDateTimeString(),
                Id = "SLOTX01",
                Location = leeds.Id,
                Session = generalSession.Id,
                Type = generalType.Id, 
                Owner = owner1.Id,
                Duration = slotTime1.Duration
            };

            var slot2 = new FreeSlot
            {
                DateTime = slotTime2.Start.ToVisionDateTimeString(),
                Id = "SLOTX02",
                Location = london.Id,
                Session = generalSession.Id,
                Type = emptyType.Id,
                Owner = owner2.Id,
                Duration = slotTime2.Duration
            };
            
            var slot3 = new FreeSlot
            {
                DateTime = slotTime2.Start.ToVisionDateTimeString(),
                Id = "SLOTX03",
                Location = london.Id,
                Session = emptySession.Id,
                Type = emptyType.Id,
                Owner = owner2.Id,
                Duration = slotTime2.Duration
            };
            
            var slot4 = new FreeSlot
            {
                DateTime = slotTime2.Start.ToVisionDateTimeString(),
                Id = "SLOTX04",
                Location = leeds.Id,
                Session = emptySession.Id,
                Type = generalType.Id, 
                Owner = owner2.Id,
                Duration = slotTime2.Duration
            };

            var slots = new[] { slot1, slot2, slot3, slot4 }.ToList();
            var locations = new[] { leeds, london }.ToList();
            var sessions = new[] { generalSession, emptySession }.ToList();
            var slotTypes = new[] { generalType, emptyType }.ToList();
            var owners = new[] { owner1, owner2 }.ToList();

            var availableAppointmentsResponse =
                CreateAvailableAppointmentsResponse(slots, locations, sessions, slotTypes, owners);
            
            // Act
            var actualResponse = _systemUnderTest.Map(availableAppointmentsResponse.Appointments);
            
            // Assert
            var expectedResponse = new[] 
            {
                new Slot
                {
                    Id = "SLOTX01",
                    Type =  "General Type",
                    Location = "Leeds",
                    StartTime = slotTime1.Start, 
                    EndTime = slotTime1.End,
                    Clinicians = new []{ "Owner1" },
                    Channel = Channel.Unknown,
                    SessionName = "General Session"
                },
                new Slot
                {
                    Id = "SLOTX04",
                    Type =  "General Type",
                    Location = "Leeds",
                    StartTime = slotTime2.Start,
                    EndTime = slotTime2.End,
                    Clinicians = new []{ "Owner2" },
                    Channel = Channel.Unknown,
                    SessionName = ""
                }
            };
            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockLogger.VerifyLogger(LogLevel.Warning, "Unable to parse Vision Appointment Slot - slot type name null or empty", Times.Exactly(2));
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenSlotsInResponseIsEmpty()
        {
            // Arrange
            var availableAppointmentsResponse = CreateAvailableAppointmentsResponse(
                    new List<FreeSlot>(),
                    new List<Location>(),
                    new List<SlotSession>(),
                    new List<SlotType>(),
                    new List<Owner>()
                );

            // Act
            var actualResponse = _systemUnderTest.Map(availableAppointmentsResponse.Appointments);

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
        public void Map_ReturnsResponseWithoutSlotsAppointment_WhenStartTimeInSlotsAppointmentIsInInvalidFormat(string invalidStartTime)
        {
            var slotTime2 = new SlotTime(Tomorrow("14:40"), Tomorrow("15:05"));

            var leeds = new Location
            {
                Id = "LOCX01",
                Name = "Leeds"
            };
            
            var london = new Location
            {
                Id = "LOCX02",
                Name = "London"
            };

            var generalSession = new SlotSession
            {
                Id = "SESSX01",
                Description = "General Session",
                Location = leeds.Id
            };

            var generalType = new SlotType
            {
                Id = "TYPEX01",
                Description = "General Type"
            };

            var owner1 = new Owner
            {
                Id = "OWN01",
                Name = "Owner1"
            };

            var owner2 = new Owner
            {
                Id = "OWN02",
                Name = "Owner2"
            };

            var slot1 = new FreeSlot
            {
                DateTime = invalidStartTime,
                Id = "SLOTX01",
                Location = leeds.Id,
                Session = generalSession.Id,
                Type = generalType.Id, 
                Owner = owner1.Id
            };
            
            var slot2 = new FreeSlot
            {
                DateTime = slotTime2.Start.ToVisionDateTimeString(),
                Id = "SLOTX02",
                Location = london.Id,
                Session = generalSession.Id,
                Type = generalType.Id,
                Owner = owner2.Id,
                Duration = slotTime2.Duration
            };

            var slots = new[] { slot1, slot2 }.ToList();
            var locations = new[] { leeds, london }.ToList();
            var sessions = new[] { generalSession }.ToList();
            var slotTypes = new[] { generalType }.ToList();
            var owners = new[] { owner1, owner2 }.ToList();

            var availableAppointmentsResponse =
                CreateAvailableAppointmentsResponse(slots, locations, sessions, slotTypes, owners);
            
            _mockLogger.SetupLogger(LogLevel.Warning, $"Unable to parse Vision appointment slot start time of '{invalidStartTime}", null).Verifiable();
            // Act
            var actualResponse = _systemUnderTest.Map(availableAppointmentsResponse.Appointments);

            // Assert
            var expectedResponse = new[] 
            {
                new Slot
                {
                    Id = "SLOTX02",
                    Type =  generalType.Description,
                    Location = "London",
                    StartTime = slotTime2.Start,
                    EndTime = slotTime2.End,
                    Clinicians = new[] { "Owner2" },
                    SessionName = generalSession.Description
                }
            };

            actualResponse.Should().BeEquivalentTo(expectedResponse);
            _mockLogger.Verify();
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
       

        private static AvailableAppointmentsResponse CreateAvailableAppointmentsResponse(
            List<FreeSlot> slots,
            List<Location> locations,
            List<SlotSession> sessions,
            List<SlotType> slotTypes, 
            List<Owner> owners
            )
        {
            var references = new References
            {
                Locations = locations,
                Sessions = sessions,
                SlotTypes = slotTypes, 
                Owners = owners
            };
            
            return new AvailableAppointmentsResponse
            {
                Appointments = new AvailableAppointments {
                    Slots = slots,
                    References = references
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