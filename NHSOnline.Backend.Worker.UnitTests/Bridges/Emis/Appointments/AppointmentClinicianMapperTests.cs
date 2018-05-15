using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Appointments;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis.Appointments
{
    [TestClass]
    public class AppointmentClinicianMapperTests
    {
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenCliniciansAreNull()
        {
            var response = new AppointmentSlotsMetadataGetResponse();

            var converter = new AppointmentClinicianMapper();

            var clinicians = converter.Map(response);

            clinicians.Should().BeEquivalentTo(new Clinician[0]);
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenResponseHasEmptySetOfClinicians()
        {
            var response = new AppointmentSlotsMetadataGetResponse { SessionHolders = new List<SessionHolder>() };

            var converter = new AppointmentClinicianMapper();

            var clinicians = converter.Map(response);

            clinicians.Should().BeEquivalentTo(new Clinician[0]);
        }
        
        [TestMethod]
        public void Map_ReturnsArray_WhenResponseHasSetOfClinicians()
        {
            var sessionHolder1 = new SessionHolder
            {
                ClinicianId = 1,
                DisplayName = "Dr House"
            };

            var sessionHolder2 = new SessionHolder
            {
                ClinicianId = 2,
                DisplayName = "Dr Who"
            };

            var sessionHolderList = new List<SessionHolder> { sessionHolder1, sessionHolder2 };

            var response = new AppointmentSlotsMetadataGetResponse { SessionHolders = sessionHolderList };

            var converter = new AppointmentClinicianMapper();
            var clinicians = converter.Map(response);

            var expectedClinician1 = new Clinician
            {
                Id = "1",
                DisplayName = "Dr House"
            };
            
            var expectedClinician2 = new Clinician
            {
                Id = "2",
                DisplayName = "Dr Who"
            };


            var expectedClinicianArray = new[]{ expectedClinician1, expectedClinician2 };

            clinicians.Should().BeEquivalentTo(expectedClinicianArray);
        }
    }
}