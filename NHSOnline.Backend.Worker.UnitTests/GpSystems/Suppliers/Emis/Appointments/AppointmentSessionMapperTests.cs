using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Appointments
{
    [TestClass]
    public class AppointmentSessionMapperTests
    {
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenLocationsAreNull()
        {
            var mapper = new AppointmentSessionMapper();

            var appointmentSessions = mapper.Map((IEnumerable<Session>)null);

            appointmentSessions.Should().BeEmpty();
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenResponseHasEmptySetOfSessions()
        {
            var mapper = new AppointmentSessionMapper();

            var appointmentSessions = mapper.Map(new List<Session>());

            appointmentSessions.Should().BeEmpty();
        }
        
        [TestMethod]
        public void Map_ReturnsArray_WhenGivenSetOfClinicians()
        {
            var session1 = new Session
            {
                SessionId = 1,
                SessionType = "Timed",
                SessionName = "Public Session"
            };

            var session2 = new Session
            {
                SessionId = 2,
                SessionType = "Untimed",
                SessionName = "Private Session"
            };

            var sessionList = new List<Session> { session1, session2 };

            var mapper = new AppointmentSessionMapper();
            var appointmentSessions = mapper.Map(sessionList);

            var expectedAppointmentSession1 = new AppointmentSession()
            {
                Id = "1",
                DisplayName = "Timed"
            };
            
            var expectedAppointmentSession2 = new AppointmentSession
            {
                Id = "2",
                DisplayName = "Untimed"
            };

            var expectedAppointmentSessionArray = new[]{ expectedAppointmentSession1, expectedAppointmentSession2 };

            appointmentSessions.Should().BeEquivalentTo(expectedAppointmentSessionArray);
        }
    }
}