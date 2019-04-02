using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Appointments
{
    [TestClass]
    public class CancellationReasonServiceTests
    {
        private IFixture _fixture;
        private ICancellationReasonService _systemUnderTest;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _systemUnderTest = _fixture.Create<CancellationReasonService>();
        }

        [TestMethod]
        public void GetDefaultCancellationReasons_HappyPath_ReturnsExpectedCancellationReasons()
        {
            //Arrange
            var expectedCancellationReasons = new List<CancellationReason>
            {
                new CancellationReason { DisplayName = "No longer required", Id = "R1_NoLongerRequired" },
                new CancellationReason { DisplayName = "Unable to attend", Id = "R2_UnableToAttend" }
            };
            
            //Act
            var actualCancellationReasons = _systemUnderTest.GetDefaultCancellationReasons();
            
            //Assert
            actualCancellationReasons.Should().BeEquivalentTo(expectedCancellationReasons);
        }        
    }
}