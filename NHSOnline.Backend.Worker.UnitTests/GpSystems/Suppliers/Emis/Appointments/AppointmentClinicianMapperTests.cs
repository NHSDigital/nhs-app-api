using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Appointments
{
    [TestClass]
    public class AppointmentClinicianMapperTests
    {
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenCliniciansAreNull()
        {
            var systemUnderTest = new AppointmentClinicianMapper();

            var clinicians = systemUnderTest.Map((IEnumerable<SessionHolder>)null);

            clinicians.Should().BeEmpty();
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenGivenEmptySetOfClinicians()
        {
            var systemUnderTest = new AppointmentClinicianMapper();

            var clinicians = systemUnderTest.Map(new List<SessionHolder>());

            clinicians.Should().BeEmpty();
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

            var systemUnderTest = new AppointmentClinicianMapper();
            var clinicians = systemUnderTest.Map(sessionHolderList);

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


            var expectedClinicianResults = new[]{ expectedClinician1, expectedClinician2 };

            clinicians.Should().BeEquivalentTo(expectedClinicianResults);
        }
    }
}