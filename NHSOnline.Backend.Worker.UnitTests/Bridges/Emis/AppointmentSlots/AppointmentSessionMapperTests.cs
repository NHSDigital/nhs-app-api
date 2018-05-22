using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.AppointmentSlots;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis.AppointmentSlots
{
    [TestClass]
    public class AppointmentSessionMapperTests
    {
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenLocationsAreNull()
        {
            var response = new AppointmentSlotsMetadataGetResponse();

            var mapper = new AppointmentSessionMapper();

            var appointmentSessions = mapper.Map(response);

            appointmentSessions.Should().BeEquivalentTo(new AppointmentSession[0]);
        }
        
        [TestMethod]
        public void Map_ReturnsEmptyArray_WhenResponseHasEmptySetOfSessions()
        {
            var response = new AppointmentSlotsMetadataGetResponse { Sessions = new List<Session>() };

            var mapper = new AppointmentSessionMapper();

            var appointmentSessions = mapper.Map(response);

            appointmentSessions.Should().BeEquivalentTo(new AppointmentSession[0]);
        }
        
        [TestMethod]
        public void Map_ReturnsArray_WhenResponseHasSetOfClinicians()
        {
            var session1 = new Session
            {
                SessionId = 1,
                SessionType = SessionType.Timed.ToString(),
                SessionName = "Public Session"
            };

            var session2 = new Session
            {
                SessionId = 2,
                SessionType = SessionType.Untimed.ToString(),
                SessionName = "Private Session"
            };

            var sessionList = new List<Session> { session1, session2 };

            var response = new AppointmentSlotsMetadataGetResponse { Sessions = sessionList };

            var mapper = new AppointmentSessionMapper();
            var appointmentSessions = mapper.Map(response);

            var expectedAppointmentSession1 = new AppointmentSession()
            {
                Id = "1",
                DisplayName = $"{SessionType.Timed.ToString()}" 
            };
            
            var expectedAppointmentSession2 = new AppointmentSession
            {
                Id = "2",
                DisplayName = $"{SessionType.Untimed.ToString()}"
            };


            var expectedAppointmentSessionArray = new[]{ expectedAppointmentSession1, expectedAppointmentSession2 };

            appointmentSessions.Should().BeEquivalentTo(expectedAppointmentSessionArray);
        }
    }
}