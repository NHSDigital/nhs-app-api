using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Appointments
{
    [TestClass]
    public class AppointmentsResponseMapperTests
    {
        private IFixture _fixture;
        private AppointmentsResponseMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<AppointmentsResponseMapper>();
        }

        [TestMethod]
        public void Map_ResponseIncludesExpectedCancellationReasons()
        {
            // Arrange
            var emisResponse = _fixture.Create<AppointmentsGetResponse>();

            // Act
            var response = _systemUnderTest.Map(emisResponse);

            // Assert
            var expectedCancellationReasons = new  List<CancellationReason>
            {
                new CancellationReason { DisplayName = "No longer required", Id = "R1_NoLongerRequired" },
                new CancellationReason { DisplayName = "Unable to attend", Id = "R2_UnableToAttend" }
            };

            response.CancellationReasons.Should().BeEquivalentTo(expectedCancellationReasons);
            response.PastAppointmentsEnabled.Should().BeTrue();
        }
    }
}
