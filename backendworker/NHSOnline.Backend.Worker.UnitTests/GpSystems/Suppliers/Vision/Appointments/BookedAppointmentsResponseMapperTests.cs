using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Appointments
{
    [TestClass]
    public class BookedAppointmentsResponseMapperTests
    {
        private IFixture _fixture;
        private BookedAppointmentsResponseMapper _systemUnderTest;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<BookedAppointmentsResponseMapper>();
        }
        
        [TestMethod]
        public void Map_ResponseIncludesNoCancellationReasons()
        {
            // Arrange
            var emisResponse = _fixture.Create<BookedAppointmentsResponse>();

            // Act
            var response = _systemUnderTest.Map(emisResponse);

            // Assert
            var expectedCancellationReasons = new List<CancellationReason>();
            var expectedAppointments = new List<Appointment>();

            response.CancellationReasons.Should().BeEquivalentTo(expectedCancellationReasons);
            response.Appointments.Should().BeEquivalentTo(expectedAppointments);
        }
    }
    
}