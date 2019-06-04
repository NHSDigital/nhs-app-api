using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;
using Slot = NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments.Slot;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Appointments
{
    [TestClass]
    public class AppointmentSlotsResponseMapperTests
    {
        private IFixture _fixture;
        private AppointmentSlotsGetResponse _appointmentsGetResponse;
        private Mock<IAppointmentSlotsMapper> _mockAppointmentSlotsMapper;
        private IAppointmentSlotsResponseMapper _systemUnderTest;
        private IEnumerable<NHSOnline.Backend.GpSystems.Appointments.Models.Slot> _microtestSlots;
        private IEnumerable<Slot> _returnedSlots;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _microtestSlots = _fixture.CreateMany<NHSOnline.Backend.GpSystems.Appointments.Models.Slot>();
            
            _mockAppointmentSlotsMapper = _fixture.Freeze<Mock<IAppointmentSlotsMapper>>();
            _mockAppointmentSlotsMapper.Setup(x => x.Map(It.IsAny<IEnumerable<Slot>>()))
                .Returns(_microtestSlots);

            _returnedSlots = _fixture.Create<IEnumerable<Slot>>();
            _appointmentsGetResponse = _fixture.Build<AppointmentSlotsGetResponse>()
                .With(x => x.Slots, _returnedSlots)
                .Create();

            _systemUnderTest = _fixture.Create<AppointmentSlotsResponseMapper>();
        }

        [TestMethod]
        public void Map_ProvidedValidAppointSlots_ReturnsCorrectSlots()
        {
            // Act
            var actualResponse = _systemUnderTest.Map(_appointmentsGetResponse, null);

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.Slots.Should().Equal(_microtestSlots);
        }

       [TestMethod]
        public void Map_ProvidedValidAppointSlots_ReturnsCorrectBookingReasonNecessity()
        {
            // Act
            var actualResponse = _systemUnderTest.Map(_appointmentsGetResponse, null);

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.BookingReasonNecessity.Should().Be(Necessity.Mandatory);
        }  
        
        [TestMethod]
        public void Map_ProvidedValidAppointSlots_ReturnsCorrectBookingGuidance()
        {
            // Act
            var actualResponse = _systemUnderTest.Map(_appointmentsGetResponse, null);

            // Assert
            actualResponse.Should().NotBeNull();
            actualResponse.BookingGuidance.Should().Be(string.Empty);
        }

        [TestMethod]
        public void Map_DemographicsResponseContainsTwoTelephoneNumbers_MapsTwoTelephoneNumbers()
        {
            var demographicsResponse = new DemographicsGetResponse
            {
                Demographics = new DemographicsData
                {
                    Telephone1 = "1234567890",
                    Telephone2 = "2345678901"
                }
            };
            
            // Act
            var actualResponse = _systemUnderTest.Map(_appointmentsGetResponse, demographicsResponse);

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
            var demographicsResponse = new DemographicsGetResponse
            {
                Demographics = new DemographicsData
                {
                    Telephone1 = "1234567890"
                }
            };
            
            // Act
            var actualResponse = _systemUnderTest.Map(_appointmentsGetResponse, demographicsResponse);

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
            var demographicsResponse = new DemographicsGetResponse
            {
                Demographics = new DemographicsData
                {
                    Telephone2 = "07901828483"
                }
            };
            
            // Act
            var actualResponse = _systemUnderTest.Map(_appointmentsGetResponse, demographicsResponse);

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
            // Act
            var actualResponse = _systemUnderTest.Map(_appointmentsGetResponse, null);

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

            // Act
            var actualResponse = _systemUnderTest.Map(_appointmentsGetResponse, demographicsResponse);

            // Assert
            actualResponse.TelephoneNumbers.Should().BeEmpty();
        }
    }
}
