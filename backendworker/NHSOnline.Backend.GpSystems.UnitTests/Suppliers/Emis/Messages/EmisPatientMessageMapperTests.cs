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
    public class EmisPatientMessageMapperTests
    {
        private IFixture _fixture;

        private EmisPatientMessageMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<EmisPatientMessageMapper>();
        }
        
        [TestMethod]
        public void Map_WhenCalledHappyPath_ReturnsMappedGetPatientMessageResponse()
        {
            // Arrange
            var messageGetResponse = _fixture.Create<MessageGetResponse>();
            
            // Act
            var result = _systemUnderTest.Map(messageGetResponse);
            
            // Assert
            result.Should().BeEquivalentTo(new GetPatientMessageResponse
            {
                MessageDetails = 
                {
                    MessageId = messageGetResponse.Message.MessageId,
                    Subject = messageGetResponse.Message.Subject,
                    Recipient = messageGetResponse.Message.Recipients[0].Name,
                    MessageReplies = messageGetResponse.Message.MessageReplies,
                    Content = messageGetResponse.Message.Content,
                    SentDateTime = messageGetResponse.Message.SentDateTime
                }
            });
        }
    }
}