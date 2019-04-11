using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class AppointmentsReplyMapperTests
    {
        private IFixture _fixture;
        private AppointmentsReplyMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<AppointmentsReplyMapper>();
        }

        [TestMethod]
        public void Map_ResponseIncludesNoCancellationReasons()
        {
            // Arrange
            var tppPastAppointmentsResponse = _fixture.Create<ViewAppointmentsReply>();
            var tppUpcomingAppointmentsResponse = _fixture.Create<ViewAppointmentsReply>();

            // Act
            var response = _systemUnderTest.Map(tppPastAppointmentsResponse, tppUpcomingAppointmentsResponse);

            // Assert
            var expectedCancellationReasons = Array.Empty<CancellationReason>();

            response.CancellationReasons.Should().BeEquivalentTo(expectedCancellationReasons);
        }
    }
}
