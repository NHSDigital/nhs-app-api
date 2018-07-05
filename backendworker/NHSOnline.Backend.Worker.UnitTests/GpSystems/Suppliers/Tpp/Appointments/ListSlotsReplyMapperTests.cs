using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Worker.Support.Date;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class ListSlotsReplyMapperTests
    {
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        private IListSlotsReplyMapper _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", "GMT Standard Time") });
            _timeZoneInfoProvider = new TimeZoneInfoProvider(configBuilder.Build());
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
            _sut = new ListSlotsReplyMapper(
                new SessionMapper(_dateTimeOffsetProvider));
        }
        [TestMethod]
        public void Map_WhenSessionSlotsIsEmpty_ReturnsEmptySetOfSlots()
        {
            var session = CreateSession("Leeds", "101", "Dr House", "Emergency");
            session.Slots = new List<Worker.GpSystems.Suppliers.Tpp.Models.Appointments.Slot>();

            var listSlotsReply = new ListSlotsReply { Sessions = new [] { session }.ToList() };

            var expectedResponse = new AppointmentSlotsResponse { Slots = new Worker.Areas.Appointments.Models.Slot[0] };

            var actualResponse = _sut.Map(listSlotsReply);

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }



        [TestMethod]
        public void Map_WhenSessionSlotsIsNull_ReturnsEmptySetOfSlots()
        {
            var session = CreateSession("Leeds", "101", "Dr House", "Emergency");

            var listSlotsReply = new ListSlotsReply { Sessions = new [] { session }.ToList() };

            var expectedResponse = new AppointmentSlotsResponse { Slots = new Worker.Areas.Appointments.Models.Slot[0] };

            var actualResponse = _sut.Map(listSlotsReply);

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_WhenNoStaffDetailsAreAvailable_ReturnsEmptySetOfClinicians()
        {
            // Arrange
            var session = CreateSession("Leeds", "101", "", "General Session Appointment");
            var slot = CreateSlot("2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");
            session.Slots = new[] { slot }.ToList();


            var listSlotsReply = new ListSlotsReply
            {
                Sessions = new [] { session }.ToList()
            };

            var expectedSlot = new Worker.Areas.Appointments.Models.Slot
            {
                Id = "101",
                Clinicians = new string[0],
                Location = "Leeds",
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19"),
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19"),
                Type = "General Session Appointment - Emergency"
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = new[] { expectedSlot }
            };

            // Act
            var actualResponse = _sut.Map(listSlotsReply);

            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_WhenNoLocationIsAvailable_ReturnsEmptyLocation()
        {
            // Arrange
            var session = CreateSession("", "101", "Dr House", "General Session Appointment");
            var slot = CreateSlot("2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");
            session.Slots = new[] { slot }.ToList();


            var listSlotsReply = new ListSlotsReply
            {
                Sessions = new List<Worker.GpSystems.Suppliers.Tpp.Models.Appointments.Session> { session }
            };

            var expectedSlot = new Worker.Areas.Appointments.Models.Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                Location = "",
                EndTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19"),
                StartTime = _dateTimeOffsetProvider.CreateDateTimeOffset("2018-05-09T10:59:19"),
                Type = "General Session Appointment - Emergency"
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = new[] { expectedSlot }
            };

            // Act
            var actualResponse = _sut.Map(listSlotsReply);

            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        private static Worker.GpSystems.Suppliers.Tpp.Models.Appointments.Session CreateSession(
            string location, 
            string id, 
            string staff, 
            string type) => new Worker.GpSystems.Suppliers.Tpp.Models.Appointments.Session
        {
            Location = location,
            SessionId = id,
            StaffDetails = staff,
            Type = type
        };

        private static Worker.GpSystems.Suppliers.Tpp.Models.Appointments.Slot CreateSlot(
            string startDate, 
            string endDate, 
            string type) => new Worker.GpSystems.Suppliers.Tpp.Models.Appointments.Slot
        {
            StartDate = startDate,
            EndDate = endDate,
            Type = type
        };

    }
}
