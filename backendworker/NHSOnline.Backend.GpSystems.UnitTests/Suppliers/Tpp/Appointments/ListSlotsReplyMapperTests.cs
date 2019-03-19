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
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class ListSlotsReplyMapperTests
    {
        private ListSlotsReplyMapper _systemUnderTest;
        private IFixture _fixture;
        private Mock<ICurrentDateTimeProvider> _mockCurrentDateTimeProvider;
        private Mock<IDateTimeOffsetProvider> _dateTimeOffsetProviderMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            _dateTimeOffsetProviderMock = _fixture.Freeze<Mock<IDateTimeOffsetProvider>>();
            
            _mockCurrentDateTimeProvider = _fixture.Freeze<Mock<ICurrentDateTimeProvider>>();
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(DateTime.UtcNow);
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            _fixture.Inject(_dateTimeOffsetProviderMock);
            
            _systemUnderTest = new ListSlotsReplyMapper(_fixture.Create<SessionMapper>());
        }
        
        [TestMethod]
        public void Map_WhenSessionSlotsIsEmpty_ReturnsEmptySetOfSlots()
        {
            var session = CreateSession("Leeds", "101", "Dr House", "Emergency");
            session.Slots = new List<Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Slot>();

            var listSlotsReply = new ListSlotsReply { Sessions = new [] { session }.ToList() };

            var expectedResponse = new AppointmentSlotsResponse { Slots = Array.Empty<Backend.GpSystems.Appointments.Models.Slot>() };

            var actualResponse = _systemUnderTest.Map(listSlotsReply);

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public void Map_WhenSessionSlotsIsNull_ReturnsEmptySetOfSlots()
        {
            var session = CreateSession("Leeds", "101", "Dr House", "Emergency");

            var listSlotsReply = new ListSlotsReply { Sessions = new [] { session }.ToList() };

            var expectedResponse = new AppointmentSlotsResponse { Slots = Array.Empty<Backend.GpSystems.Appointments.Models.Slot>() };

            var actualResponse = _systemUnderTest.Map(listSlotsReply);

            actualResponse.Should().BeEquivalentTo(expectedResponse);
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
                StartTime = slotTime,
                Type = "Emergency",
                SessionName = "General Session Appointment"
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = new[] { expectedSlot }
            };

            // Act
            var actualResponse = _systemUnderTest.Map(listSlotsReply);

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
                StartTime = slotTime,
                Type = "Emergency", 
                SessionName = "General Session Appointment"
            };

            var expectedResponse = new AppointmentSlotsResponse
            {
                Slots = new[] { expectedSlot }
            };

            // Act
            var actualResponse = _systemUnderTest.Map(listSlotsReply);

            // Assert
            actualResponse.Should().BeEquivalentTo(expectedResponse);
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
