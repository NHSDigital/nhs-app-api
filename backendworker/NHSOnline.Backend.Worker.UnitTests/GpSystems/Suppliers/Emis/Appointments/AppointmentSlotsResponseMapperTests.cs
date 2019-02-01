using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.SharedModels;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Temporal;
using Location = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Appointments
{
    [TestClass]
    public class AppointmentSlotsResponseMapperTests
    {
        private const string UserPatientLinkToken = "USER_PATIENT_LINK_TOKEN";
        private const string EndUserSessionId = "END_USER_SESSION_ID";
        private const string SessionId = "SESSION_ID";

        private IFixture _fixture;
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        private IAppointmentSlotsResponseMapper _sut;
        private EmisUserSession _userSession;
        DemographicsGetResponse _demographics;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform())
            });
            _timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);

            var slotsMapperLogger = _fixture.Create<ILoggerFactory>().CreateLogger<AppointmentSlotsMapper>();

            _userSession = new EmisUserSession()
            {
                UserPatientLinkToken = UserPatientLinkToken,
                EndUserSessionId = EndUserSessionId,
                SessionId = SessionId,
                OdsCode = "TestOds",
                AppointmentBookingReasonNecessity = Necessity.Optional
            };

            _demographics = new DemographicsGetResponse();

            _sut =
                new AppointmentSlotsResponseMapper(new AppointmentSlotsMapper(_dateTimeOffsetProvider, slotsMapperLogger,_fixture.Create<EmisEnumMapper>()));
        }

        [TestMethod]
        public void Map_WhenPatientHasTelephoneNumbers_ReturnsTelephoneNumbersArray()
        {
            // Arrange
            var slotsResponse = new AppointmentSlotsGetResponse();

            var location = CreateLocation(23, "Leeds");
            var session = CreateSession(location.LocationId, 1, "General Appointment Session", "Timed");

            var practiceSettigsResponse = new PracticeSettingsGetResponse
            {
                Messages = new PracticeSettingsMessages()
            };

            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Sessions = new[] { session }
            };

            var demographicsWithTelephone = new DemographicsGetResponse()
            {
                ContactDetails = new ContactDetails()
                {
                    TelephoneNumber = "01243254363",
                    MobileNumber = "07213432543"
                }
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = Array.Empty<Slot>(),
                BookingReasonNecessity = Necessity.Optional,
                TelephoneNumbers = new List<PatientTelephoneNumber> {
                    new PatientTelephoneNumber(){TelephoneNumber = "01243254363"},
                    new PatientTelephoneNumber(){TelephoneNumber = "07213432543"}
                } 
            };

            // Act
            var actualResponse =
                _sut.Map(slotsResponse, slotsMetadataResponse, practiceSettigsResponse, demographicsWithTelephone, _userSession);

            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_WhenSessionInAppointmentsSlotsResponseIsNull_ReturnsEmptySetOfSlots()
        {
            // Arrange
            var slotsResponse = new AppointmentSlotsGetResponse();
            
            var location = CreateLocation(23, "Leeds");
            var session = CreateSession(location.LocationId, 1, "General Appointment Session", "Timed");

            var practiceSettigsResponse = new PracticeSettingsGetResponse
            {
                Messages = new PracticeSettingsMessages()
            };

            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Sessions = new[] { session }
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = Array.Empty<Slot>(),
                BookingReasonNecessity = Necessity.Optional
            };

            // Act
            var actualResponse = 
                _sut.Map(slotsResponse, slotsMetadataResponse, practiceSettigsResponse, _demographics, _userSession);
            
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Map_WhenSessionInAppointmentsSlotsResponseIsEmpty_ReturnsEmptySetOfSlots()
        {
            // Arrange
            var slotsResponse = new AppointmentSlotsGetResponse
            {
                Sessions = new List<AppointmentSlotSession>()
            };
            
            var location = CreateLocation(23, "Leeds");
            var session = CreateSession(location.LocationId, 1, "General Appointment Session", "Timed");

            var practiceSettigsResponse = new PracticeSettingsGetResponse
            {
                Messages = new PracticeSettingsMessages()
            };

            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Sessions = new[] { session }
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = Array.Empty<Slot>(),
                BookingReasonNecessity = Necessity.Optional
            };

            // Act
            var actualResponse = 
                _sut.Map(slotsResponse, slotsMetadataResponse, practiceSettigsResponse, _demographics, _userSession);
            
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Map_WhenSessionInAppointmentsSlotsMetadataResponseIsNull_ReturnsEmptySetOfSlots()
        {
            // Arrange
            var location = CreateLocation(14, "Leeds");
            var sessionHolder = CreateSessionHolder(34, "Dr Who");

            var practiceSettigsResponse = new PracticeSettingsGetResponse
            {
                Messages = new PracticeSettingsMessages()
            };

            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Locations = new[] { location },
                SessionHolders = new[] { sessionHolder }
                
            };

            var appointmentSlotSession =
                CreateAppointmentsSlotSession(1, 2, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency","Unknown");

            var slotsResponse = new AppointmentSlotsGetResponse
            {
                Sessions = new[] { appointmentSlotSession }
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = Array.Empty<Slot>(),
                BookingReasonNecessity = Necessity.Optional
            };
            // Act
            var actualResponse = 
                _sut.Map(slotsResponse, slotsMetadataResponse, practiceSettigsResponse, _demographics, _userSession);
            
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Map_WhenSessionInAppointmentsSlotsMetadataResponseIsEmpty_ReturnsEmptySetOfSlots()
        {
            // Arrange
            var location = CreateLocation(14, "Leeds");
            var sessionHolder = CreateSessionHolder(34, "Dr Who");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Sessions = Array.Empty<Worker.GpSystems.Suppliers.Emis.Models.Session>(),
                Locations = new[] { location },
                SessionHolders = new[] { sessionHolder }
            };

            var practiceSettigsResponse = new PracticeSettingsGetResponse
            {
                Messages = new PracticeSettingsMessages
                {
                    AppointmentsMessage = "Please do not book appointments if you have a sore throat."
                }
            };

            var appointmentSlotSession =
                CreateAppointmentsSlotSession(1, 2, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency","Unknown");

            var slotsResponse = new AppointmentSlotsGetResponse
            {
                Sessions = new[] { appointmentSlotSession }
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                BookingGuidance = "Please do not book appointments if you have a sore throat.",
                Slots = Array.Empty<Slot>(),
                BookingReasonNecessity = Necessity.Optional
            };

            // Act
            var actualResponse = _sut.Map(slotsResponse, slotsMetadataResponse, practiceSettigsResponse, _demographics, _userSession);
            
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Map_WhenNoLocationsAreAvailable_ReturnsEmptySetOfLocations()
        {
            // Arrange
            var sessionHolder = CreateSessionHolder(34, "Dr Who");
            var session = new Worker.GpSystems.Suppliers.Emis.Models.Session
            {
                SessionId = 77,
                SessionName = "General Session Appointment",
                SessionType = "Timed"
            };
 
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Sessions = new[]{ session },
                Locations = Array.Empty<Location>(),
                SessionHolders = new[] { sessionHolder }
            };

            var appointmentSlotSession =
                CreateAppointmentsSlotSession(1, 77, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency","Unknown");

            var slotsResponse = new AppointmentSlotsGetResponse
            {
                Sessions = new[] { appointmentSlotSession }
            };

            var practiceSettingsResponse = new PracticeSettingsGetResponse
            {
                Messages = new PracticeSettingsMessages { AppointmentsMessage = "Please do not book appointments if you have a sore throat." }
            };

            var expectedSlot = new Slot
            {
                Id = "1",
                Clinicians = Array.Empty<string>(),
                Location = "",
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Type = "General Session Appointment - Emergency"
            };
            
            var expectedResponse = new AppointmentSlotsResponse
            {
                BookingGuidance = "Please do not book appointments if you have a sore throat.",
                Slots = new[] { expectedSlot },
                BookingReasonNecessity = Necessity.Optional
            };

            // Act
            var actualResponse = _sut.Map(slotsResponse, slotsMetadataResponse, practiceSettingsResponse, _demographics, _userSession);
            
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
        [TestMethod]
        public void Map_WhenNoCliniciansAreAvailable_ReturnsEmptySetOfClinicians()
        {
            // Arrange
            var session = new Worker.GpSystems.Suppliers.Emis.Models.Session
            {
                SessionId = 77,
                SessionType = "Timed",
                SessionName = "General Session Appointment"
            };
            var location = CreateLocation(365, "Leeds");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Sessions = new[]{ session },
                Locations = new[] { location },
                SessionHolders = Array.Empty<SessionHolder>(),
            };

            var appointmentSlotSession =
                CreateAppointmentsSlotSession(1, 77, "2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency","Unknown");

            var slotsResponse = new AppointmentSlotsGetResponse
            {
                Sessions = new[] { appointmentSlotSession }
            };

            var practiceSettigsResponse = new PracticeSettingsGetResponse
            {
                Messages = new PracticeSettingsMessages { AppointmentsMessage = "Please do not book appointments if you have a sore throat." }
            };

            var expectedSlot = new Slot
            {
                Id = "1",
                Clinicians = Array.Empty<string>(),
                Location = "",
                EndTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                StartTime = _dateTimeOffsetProvider.GetDateTimeOffsetForTest("2018-05-09T10:59:19"),
                Type = "General Session Appointment - Emergency"
            };
            
            var expectedResponse = new AppointmentSlotsResponse
            {
                BookingGuidance = "Please do not book appointments if you have a sore throat.",
                Slots = new[] { expectedSlot },
                BookingReasonNecessity = Necessity.Optional
            };

            // Act
            var actualResponse = _sut.Map(slotsResponse, slotsMetadataResponse, practiceSettigsResponse, _demographics, _userSession);
            
            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        private static AppointmentSlotSession CreateAppointmentsSlotSession(int slotId, int sessionId, string startTime, string endTime, string slotTypeName, string slotTypeStatus)
        {
            var appointmentSlot = new AppointmentSlot
            {
                SlotId = slotId,
                EndTime = endTime,
                StartTime = startTime,
                SlotTypeName = slotTypeName,
                SlotTypeStatus = slotTypeStatus
            };
            
            var appointmentSlotSession = new AppointmentSlotSession
            {
                SessionId = sessionId,
                Slots = new[]{ appointmentSlot }
            };

            return appointmentSlotSession;
        }

        private static Location CreateLocation(int id, string name)
        {
            return new Location
            {
                LocationId = id,
                LocationName = name
            };
        }

        private static SessionHolder CreateSessionHolder(int id, string name)
        {
            return new SessionHolder
            {
                ClinicianId = id,
                DisplayName = name
            };
        }

        private static Worker.GpSystems.Suppliers.Emis.Models.Session CreateSession(int locationId, int sessionId, string name, string sessionType)
        {
            return new Worker.GpSystems.Suppliers.Emis.Models.Session
            {
                LocationId = locationId,
                SessionId = sessionId,
                SessionName = name,
                SessionType = sessionType
            };
        }
    }
}
