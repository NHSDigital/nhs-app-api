using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;
using Slot = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments.Slot;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Appointments
{
    [TestClass]
    public class AppointmentSlotsMapperTests
    {
        private IFixture _fixture;
        private IAppointmentSlotsResponseMapper _systemUnderTest;
        private IEnumerable<Slot> _microtestSlots;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            var mapper = _fixture.Create<IMicrotestEnumMapper>();

            var logger = _fixture.Create<ILogger<AppointmentSlotsResponseMapper>>();

            _systemUnderTest = new AppointmentSlotsResponseMapper(mapper, logger);

            _microtestSlots = _fixture.CreateMany<Slot>();
        }

        [TestMethod]
        public void Map_ReturnsEmptySlotsArray_WhenEmptyCollectionIsPassed()
        {
            // Arrange
            var appointmentSlotsGetResponse = new AppointmentSlotsGetResponse
            {
                Slots = Enumerable.Empty<Slot>()
            };

            // Act
            var actualResponse = _systemUnderTest.Map(appointmentSlotsGetResponse, null);

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.Slots.Should().BeEmpty();
        }

        [TestMethod]
        public void Map_ProvidedValidAppointmentSlots_Maps()
        {
            // Arrange
            var appointmentSlotsGetResponse = new AppointmentSlotsGetResponse
            {
                Slots = _microtestSlots
            };

            // Act
            var actualResponse = _systemUnderTest.Map(appointmentSlotsGetResponse, null);

            // Assert
            actualResponse.Should().NotBeNull();
            foreach (var slot in actualResponse.Slots)
            {
                var microtestSlot = _microtestSlots.First(s => s.Id.Equals(slot.Id, StringComparison.Ordinal));
                slot.Should().BeEquivalentTo(microtestSlot, options => options.Excluding( s => s.Channel));
                slot.Channel.Should().Be(Channel.Unknown);
            }
        }

        [TestMethod]
        public void Map_DoesNotMapAppointmentSlot_WhenStartTimeIsNull()
        {
            // Arrange
            var invalidSlot = _microtestSlots.First();

            invalidSlot.StartTime = null;

            var appointmentSlotsGetResponse = new AppointmentSlotsGetResponse
            {
                Slots = _microtestSlots
            };

            // Act
            var actualResponse = _systemUnderTest.Map(appointmentSlotsGetResponse, null);

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.Slots.Should().NotContain(slot => slot.Id.Equals(invalidSlot.Id, StringComparison.Ordinal));
            actualResponse.Slots.Count().Should().Be(_microtestSlots.Count() - 1);
        }        
        
        [TestMethod]
        public void Map_DemographicsResponseContainsTwoTelephoneNumbers_MapsTwoTelephoneNumbers()
        {
            var appointmentSlotsGetResponse = new AppointmentSlotsGetResponse
            {
                Slots = _microtestSlots
            };

            var demographicsResponse = new DemographicsGetResponse
            {
                Demographics = new DemographicsData
                {
                    Telephone1 = "1234567890",
                    Telephone2 = "2345678901"
                }
            };
            
            // Act
            var actualResponse = _systemUnderTest.Map(appointmentSlotsGetResponse, demographicsResponse);

            // Assert
            var expectedTelephones = new[]
            {
                new PatientTelephoneNumber { TelephoneNumber = "1234567890" },
                new PatientTelephoneNumber { TelephoneNumber = "2345678901" }
            };

            actualResponse.TelephoneNumbers.Should().BeEquivalentTo(expectedTelephones);
        }
        
        [TestMethod]
        public void Map_DemographicsResponseContainsTelephone1Only_MapsOneTelephoneNumber()
        {
            var appointmentSlotsGetResponse = new AppointmentSlotsGetResponse
            {
                Slots = _microtestSlots
            };

            var demographicsResponse = new DemographicsGetResponse
            {
                Demographics = new DemographicsData
                {
                    Telephone1 = "1234567890"
                }
            };
            
            // Act
            var actualResponse = _systemUnderTest.Map(appointmentSlotsGetResponse, demographicsResponse);

            // Assert
            var expectedTelephones = new[]
            {
                new PatientTelephoneNumber { TelephoneNumber = "1234567890" }
            };

            actualResponse.TelephoneNumbers.Should().BeEquivalentTo(expectedTelephones);
        }
        
        [TestMethod]
        public void Map_DemographicsResponseContainsTelephone2Only_MapsOneTelephoneNumber()
        {
            var appointmentSlotsGetResponse = new AppointmentSlotsGetResponse
            {
                Slots = _microtestSlots
            };

            var demographicsResponse = new DemographicsGetResponse
            {
                Demographics = new DemographicsData
                {
                    Telephone2 = "07901828483"
                }
            };
            
            // Act
            var actualResponse = _systemUnderTest.Map(appointmentSlotsGetResponse, demographicsResponse);

            // Assert
            var expectedTelephones = new[]
            {
                new PatientTelephoneNumber { TelephoneNumber = "07901828483" }
            };

            actualResponse.TelephoneNumbers.Should().BeEquivalentTo(expectedTelephones);
        }
        
        [TestMethod]
        public void Map_DemographicsResponseIsNull_ResponseHasEmptyTelephoneNumbers()
        {
            var appointmentSlotsGetResponse = new AppointmentSlotsGetResponse
            {
                Slots = _microtestSlots
            };

            // Act
            var actualResponse = _systemUnderTest.Map(appointmentSlotsGetResponse, null);

            // Assert
            actualResponse.TelephoneNumbers.Should().BeEmpty();
        }
        
        [TestMethod]
        public void Map_DemographicsResponseHasNullDemographicsData_ResponseHasEmptyTelephoneNumbers()
        {
            var demographicsResponse = new DemographicsGetResponse
            {
                Demographics = null
            };
            
            var appointmentSlotsGetResponse = new AppointmentSlotsGetResponse
            {
                Slots = _microtestSlots
            };

            // Act
            var actualResponse = _systemUnderTest.Map(appointmentSlotsGetResponse, demographicsResponse);

            // Assert
            actualResponse.TelephoneNumbers.Should().BeEmpty();
        }
    }
}
