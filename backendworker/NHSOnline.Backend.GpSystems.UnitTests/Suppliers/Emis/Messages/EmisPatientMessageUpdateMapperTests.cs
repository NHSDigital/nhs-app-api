using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Messages
{
    [TestClass]
    public class EmisPatientMessageUpdateMapperTests
    {
        private IFixture _fixture;

        private EmisPatientMessageUpdateMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<EmisPatientMessageUpdateMapper>();
        }
        
        [TestMethod]
        public void Map_WhenCalledHappyPath_ReturnsMappedPutPatientMessageUpdateStatusResponse()
        {
            // Arrange
            var messagePutResponse = _fixture.Create<MessageUpdateResponse>();
            
            // Act
            var result = _systemUnderTest.Map(messagePutResponse);
            
            // Assert
            result.Should().BeEquivalentTo(new PutPatientMessageUpdateStatusResponse
            {
                MessageReadStateUpdateStatus = messagePutResponse.MessageReadStateUpdateStatus
            });
        }
    }
}