using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientPracticeMessaging
{
    [TestClass]
    public class TppPatientPracticeMessagingServiceTests
    {
        private PatientPracticeMessagingService _systemUnderTest;

        private GpUserSession _gpUserSession;
        private IFixture _fixture;
        private Mock<ITppClientRequest<TppUserSession, MessageRecipientsReply>> _messageRecipients;
        private Mock<IGetPatientPracticeMessagingRecipientsTaskChecker> _messageRecipientsTaskChecker;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _gpUserSession = _fixture.Create<TppUserSession>();
            _messageRecipients = _fixture.Freeze<Mock<ITppClientRequest<TppUserSession, MessageRecipientsReply>>>();
            _messageRecipientsTaskChecker = _fixture.Freeze<Mock<IGetPatientPracticeMessagingRecipientsTaskChecker>>();
            _systemUnderTest = _fixture.Create<PatientPracticeMessagingService>();
        }

        [TestMethod]
        public async Task GetMessageRecipients_ReturnsSuccessResponseForHappyPath_WhenSuccessfulResponseFromTpp()
        {
            // Arrange
            var messageRecipientsResponse = new MessageRecipientsReply
            {
                Items = new List<Item>
                {
                    new Item
                    {
                        ItemText = "Mr Dr",
                        Id = "12345"
                    },
                    new Item
                    {
                        ItemText = "Mrs Dr",
                        Id = "123456"
                    }
                }
            };

            _messageRecipients.Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<MessageRecipientsReply>(HttpStatusCode.OK)
                    {
                        Body = messageRecipientsResponse,
                        ErrorResponse = null,
                    }));

            _messageRecipientsTaskChecker.Setup(x =>
                    x.Check(It.IsAny<TppApiObjectResponse<MessageRecipientsReply>>()))
                .Returns(new MessageRecipientsResponse
                {
                    MessageRecipients = new List<MessageRecipient>
                    {
                        new MessageRecipient
                        {
                            Name = "Mr Dr",
                            RecipientIdentifier = "12345"
                        },
                        new MessageRecipient
                        {
                            Name = "Mrs Dr",
                            RecipientIdentifier = "123456"
                        }
                    },
                    HasErrored = false
                })
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessageRecipients(_gpUserSession);

            // Assert
            _messageRecipients.Verify(x => x.Post(It.IsAny<TppUserSession>()));

            result.Should().BeAssignableTo<GetPatientMessageRecipientsResult.Success>()
                .Subject.Response.MessageRecipients.Count.Should().Be(2);
        }

        [TestMethod]
        public async Task GetMessageRecipients_ReturnsSuccessResponseForZeroRecipients_WhenSuccessfulResponseFromTpp()
        {
            // Arrange
            var messageRecipientsResponse = new MessageRecipientsReply
            {
                Items = new List<Item> ()
            };

            _messageRecipients.Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<MessageRecipientsReply>(HttpStatusCode.OK)
                    {
                        Body = messageRecipientsResponse,
                        ErrorResponse = null,
                    }));

            _messageRecipientsTaskChecker.Setup(x =>
                    x.Check(It.IsAny<TppApiObjectResponse<MessageRecipientsReply>>()))
                .Returns(new MessageRecipientsResponse
                {
                    MessageRecipients = new List<MessageRecipient>(),
                    HasErrored = false
                })
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessageRecipients(_gpUserSession);

            // Assert
            _messageRecipients.Verify(x => x.Post(It.IsAny<TppUserSession>()));

            result.Should().BeAssignableTo<GetPatientMessageRecipientsResult.Success>()
                .Subject.Response.MessageRecipients.Count.Should().Be(0);
        }
    }
}