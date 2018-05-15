using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Bridges.Emis.Appointments;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis.Appointments
{
    [TestClass]
    public class AppointmentSessionMapperTests
    {
        [TestMethod]
        public void Mapt_ReturnsEmptyArray_WhenLocationsAreNull()
        {
            var response = new AppointmentSlotsMetadataGetResponse();

            var converter = new AppointmentSessionMapper();

            var appointmentSessions = converter.Map(response);

            appointmentSessions.Should().BeEquivalentTo(new Worker.Areas.Appointments.Models.AppointmentSession[0]);
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenResponseHasEmptySetOfSessions()
        {
            var response = new AppointmentSlotsMetadataGetResponse { Sessions = new List<Worker.Bridges.Emis.Models.Session>() };

            var converter = new AppointmentSessionMapper();

            var appointmentSessions = converter.Map(response);

            appointmentSessions.Should().BeEquivalentTo(new Worker.Areas.Appointments.Models.AppointmentSession[0]);
        }
        
        [TestMethod]
        public void Map_ReturnsArray_WhenResponseHasSetOfClinicians()
        {
            var session1 = new Worker.Bridges.Emis.Models.Session
            {
                SessionId = 1,
                SessionName = "Appointment Session"
            };

            var session2 = new Worker.Bridges.Emis.Models.Session
            {
                SessionId = 2,
                SessionName = "General Appointment Session"
            };

            var sessionList = new List<Worker.Bridges.Emis.Models.Session> { session1, session2 };

            var response = new AppointmentSlotsMetadataGetResponse { Sessions = sessionList };

            var converter = new AppointmentSessionMapper();
            var appointmentSessions = converter.Map(response);

            var expectedAppointmentSession1 = new Worker.Areas.Appointments.Models.AppointmentSession()
            {
                Id = "1",
                DisplayName = "Appointment Session"
            };
            
            var expectedAppointmentSession2 = new Worker.Areas.Appointments.Models.AppointmentSession
            {
                Id = "2",
                DisplayName = "General Appointment Session"
            };


            var expectedAppointmentSessionArray = new[]{ expectedAppointmentSession1, expectedAppointmentSession2 };

            appointmentSessions.Should().BeEquivalentTo(expectedAppointmentSessionArray);
        }
    }
}