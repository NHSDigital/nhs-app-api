using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class AppointmentSlotsMapperTests
    {
        private AppointmentSlotsMapper _systemUnderTest;
        private IFixture _fixture;
        private Mock<IDateTimeOffsetProvider> _dateTimeOffsetProviderMock;
        private RequestSystmOnlineMessagesReply _messagesReply;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            _dateTimeOffsetProviderMock = _fixture.Freeze<Mock<IDateTimeOffsetProvider>>();
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            _fixture.Inject(_dateTimeOffsetProviderMock);
            
            _messagesReply = _fixture.Create<RequestSystmOnlineMessagesReply>();
            
            _systemUnderTest = new AppointmentSlotsMapper(_fixture.Create<SessionMapper>());
        }
        
        [TestMethod]
        public void Map_WhenSessionSlotsIsEmpty_ReturnsEmptySetOfSlots()
        {
            // Arrange
            var session = CreateSession("Leeds", "101", "Dr House", "Emergency");
            session.Slots = new List<Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Slot>();
            var listSlotsReply = new ListSlotsReply { Sessions = new [] { session }.ToList() };
            var expectedResponse = new AppointmentSlotsResponse { Slots = Array.Empty<Backend.GpSystems.Appointments.Models.Slot>() };

            // Act
            var actualResponse = _systemUnderTest.Map(listSlotsReply, _messagesReply);

            // Assert
            actualResponse.Slots.Should().BeEquivalentTo(expectedResponse.Slots);
        }

        [TestMethod]
        public void Map_WhenSessionSlotsIsNull_ReturnsEmptySetOfSlots()
        {
            var session = CreateSession("Leeds", "101", "Dr House", "Emergency");

            var listSlotsReply = new ListSlotsReply { Sessions = new [] { session }.ToList() };

            var expectedResponse = new AppointmentSlotsResponse { Slots = Array.Empty<Backend.GpSystems.Appointments.Models.Slot>() };

            var actualResponse = _systemUnderTest.Map(listSlotsReply, _messagesReply);

            actualResponse.Slots.Should().BeEquivalentTo(expectedResponse.Slots);
        }

        [TestMethod]
        public void Map_WhenNoStaffDetailsAreAvailable_ReturnsEmptySetOfClinicians()
        {
            // Arrange
            var session = CreateSession("Leeds", "101", "", "General Session Appointment");
            var slot = CreateSlot("2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");
            session.Slots = new[] { slot }.ToList();
            
            
            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-05-09T10:59:19");

            var listSlotsReply = new ListSlotsReply
            {
                Sessions = new [] { session }.ToList()
            };

            var expectedSlot = new Backend.GpSystems.Appointments.Models.Slot
            {
                Id = "101",
                Clinicians =  Array.Empty<string>(),
                Location = "Leeds",
                EndTime = slotTime,
                StartTime = (DateTimeOffset) slotTime,
                Type = "Emergency",
                SessionName = "General Session Appointment"
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = new[] { expectedSlot }
            };

            // Act
            var actualResponse = _systemUnderTest.Map(listSlotsReply, _messagesReply);

            // Assert
            actualResponse.Slots.Should().BeEquivalentTo(expectedResponse.Slots);
        }

        [TestMethod]
        public void Map_WhenNoLocationIsAvailable_ReturnsEmptyLocation()
        {
            // Arrange
            var session = CreateSession("", "101", "Dr House", "General Session Appointment");
            var slot = CreateSlot("2018-05-09T10:59:19", "2018-05-09T10:59:19", "Emergency");
            session.Slots = new[] { slot }.ToList();

            var slotTime = _dateTimeOffsetProviderMock.MockDateTimeOffset("2018-05-09T10:59:19");
            
            var listSlotsReply = new ListSlotsReply
            {
                Sessions = new List<Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Session> { session }
            };

            var expectedSlot = new Backend.GpSystems.Appointments.Models.Slot
            {
                Id = "101",
                Clinicians = new[] { "Dr House" },
                Location = "",
                EndTime =  slotTime,
                StartTime = (DateTimeOffset) slotTime,
                Type = "Emergency", 
                SessionName = "General Session Appointment"
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = new[] { expectedSlot }
            };

            // Act
            var actualResponse = _systemUnderTest.Map(listSlotsReply, _messagesReply);

            // Assert
            actualResponse.Slots.Should().BeEquivalentTo(expectedResponse.Slots);
        }

        [TestMethod]
        public void Map_WhenSystemMessagesNull_ReturnsEmptyBookingGuidance()
        {
            // Arrange
            var listSlotsReply = new ListSlotsReply();
            
            // Act
            var actualResponse = _systemUnderTest.Map(listSlotsReply, null);

            // Assert
            actualResponse.BookingGuidance.Should().Be("");
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("  ")]
        [DataTestMethod]
        public void Map_WhenSystemMessagesBookAppointmentsNullOrWhitespace_ReturnsEmptyBookingGuidance(string bookAppointments)
        {
            // Arrange
            var listSlotsReply = new ListSlotsReply();
            _messagesReply.BookAppointments = bookAppointments;
            
            // Act
            var actualResponse = _systemUnderTest.Map(listSlotsReply, _messagesReply);

            // Assert
            actualResponse.BookingGuidance.Should().Be("");   
        }
        
        [DataRow("test", "test")]
        [DataRow(" abc ", "abc")]
        [DataRow(" appointment booking guidance  ", "appointment booking guidance")]
        [DataTestMethod]
        public void Map_WhenSystemMessagesBookAppointmentsPopulated_ReturnsTrimmedBookingGuidance(string bookAppointments, string expectedGuidance)
        {
            // Arrange
            var listSlotsReply = new ListSlotsReply();
            _messagesReply.BookAppointments = bookAppointments;
            
            // Act
            var actualResponse = _systemUnderTest.Map(listSlotsReply, _messagesReply);

            // Assert
            actualResponse.BookingGuidance.Should().Be(expectedGuidance);   
        }

        private static Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Session CreateSession(
            string location, 
            string id, 
            string staff, 
            string type) => new Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Session
        {
            Location = location,
            SessionId = id,
            StaffDetails = staff,
            Type = type
        };

        private static Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Slot CreateSlot(
            string startDate, 
            string endDate, 
            string type) => new Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Slot
        {
            StartDate = startDate,
            EndDate = endDate,
            Type = type
        };
    }
}
