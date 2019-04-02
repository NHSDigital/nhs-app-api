using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Appointments
{
    [TestClass]
    public class AppointmentsResponseMapperTests
    {
        private IFixture _fixture;
        private AppointmentsResponseMapper _systemUnderTest;
        private Mock<ICancellationReasonService> _defaultCancellationReasons;
        private IEnumerable<CancellationReason> _expectedCancellationReasons;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _expectedCancellationReasons = _fixture.CreateMany<CancellationReason>();
            _defaultCancellationReasons = _fixture.Freeze<Mock<ICancellationReasonService>>();
            _defaultCancellationReasons.Setup(x => x.GetDefaultCancellationReasons())
                .Returns(_expectedCancellationReasons)
                .Verifiable();

            _systemUnderTest = _fixture.Create<AppointmentsResponseMapper>();
        }

        [TestMethod]
        public void Map_HappyPath_ReturnsExpectedCancellationReasons()
        {
            // Arrange
            var emisResponse = _fixture.Create<AppointmentsGetResponse>();
            
            // Act
            var response = _systemUnderTest.Map(emisResponse);

            // Assert
            response.CancellationReasons.Should().BeEquivalentTo(_expectedCancellationReasons);
            _defaultCancellationReasons.Verify();
        }
        
        [TestMethod]
        public void Map_HappyPath_PastAppointmentsShouldBeEnabled()
        {
            // Arrange
            var emisResponse = _fixture.Create<AppointmentsGetResponse>();
            
            // Act
            var response = _systemUnderTest.Map(emisResponse);
            
            // Assert
            response.PastAppointmentsEnabled.Should().BeTrue();
        }

    }
}
