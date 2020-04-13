using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Messages
{
    public class EmisPatientMessageSendMapperTests
    {
        private IFixture _fixture;

        private EmisPatientMessageSendMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<EmisPatientMessageSendMapper>();
        }

        [TestMethod]
        public void Map_WhenCalledHappyPath_ReturnsMappedPostPatientMessageResponse()
        {
            // Arrange
            var messagePostResponse = _fixture.Create<MessagePostResponse>();

            // Act
            var result = _systemUnderTest.Map(messagePostResponse);

            // Assert
            result.Should().BeEquivalentTo(new PostPatientMessageResponse
            {
                MessageSent = messagePostResponse.MessageSent
            });
        }

        [TestMethod]
        public void Map_MessageNotSent_ReturnsMappedPostPatientMessageResponse()
        {
            // Arrange
            var messagePostResponse = _fixture.Create<MessagePostResponse>();
            messagePostResponse.MessageSent = null;

            // Act
            var result = _systemUnderTest.Map(messagePostResponse);

            // Assert
            result.Should().BeNull();
        }
    }
}