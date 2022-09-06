using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.UnitTests.Areas.Messages
{
    [TestClass]
    public class CanonicalSenderNameServiceTests
    {
        private const string NhsAppId = "NHS App Test";
        private const string NhsAppSupplierId = "NHS App Supplier Test";

        private Mock<IMessagesConfiguration> _mockConfiguration;
        private Mock<ISenderService> _mockSenderService;

        private CanonicalSenderNameService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockSenderService = new Mock<ISenderService>(MockBehavior.Strict);
            _mockConfiguration = new Mock<IMessagesConfiguration>();
            _mockConfiguration.SetupGet(c => c.SenderIdNhsApp).Returns(NhsAppId);
            _mockConfiguration.SetupGet(c => c.SupplierIdNhsApp).Returns(NhsAppSupplierId);

            _systemUnderTest = new CanonicalSenderNameService(_mockSenderService.Object, _mockConfiguration.Object);
        }

        [TestMethod]
        public async Task UpdateWithCanonicalSenderName_ShouldUseSenderServiceToUpdateMessagesWithCanonicalSenderName()
        {
            // Arrange
            const string expectedSenderName = "CanonicalSenderName";
            const string senderId = "Sender Id";

            var userMessage = new UserMessage
            {
                Sender = "Not CanonicalSenderName",
                SenderContext = new SenderContext
                {
                    SenderId = senderId,
                    SupplierId = NhsAppSupplierId
                }
            };

            var canonicalSender = new SendersResponse
            {
                Senders = new List<Sender>
                {
                    new Sender
                    {
                        Id = senderId,
                        Name = expectedSenderName
                    }
                }
            };

            _mockSenderService.Setup(s => s.GetSender(senderId))
                .ReturnsAsync(new SendersResult.Found(canonicalSender));

            // Act
            await _systemUnderTest.UpdateWithCanonicalSenderName(new []{userMessage});

            // Assert
            userMessage.Sender.Should().Be(expectedSenderName);
            userMessage.SenderContext.SenderId.Should().Be(senderId);
            userMessage.SenderContext.SupplierId.Should().Be(NhsAppSupplierId);

            VerifyAll();
        }

        [TestMethod]
        public async Task UpdateWithCanonicalSenderName_SenderServiceReturnsNotFound_ReturnMessageWithOriginalSender()
        {
            // Arrange
            const string expectedSenderName = "Not CanonicalSenderName";
            const string senderId = "Sender Id";

            var userMessage = new UserMessage
            {
                Sender = "Not CanonicalSenderName",
                SenderContext = new SenderContext
                {
                    SenderId = senderId,
                    SupplierId = NhsAppSupplierId
                }
            };

            _mockSenderService.Setup(s => s.GetSender(senderId))
                .ReturnsAsync(new SendersResult.None());

            // Act
            await _systemUnderTest.UpdateWithCanonicalSenderName(new []{userMessage});

            // Assert
            userMessage.Sender.Should().Be(expectedSenderName);
            userMessage.SenderContext.SenderId.Should().Be(senderId);
            userMessage.SenderContext.SupplierId.Should().Be(NhsAppSupplierId);

            VerifyAll();
        }

        [TestMethod]
        public async Task UpdateWithCanonicalSenderName_SenderServiceReturnsErrorResult_ReturnMessageWithOriginalSender()
        {
            // Arrange
            const string expectedSenderName = "Not CanonicalSenderName";
            const string senderId = "Sender Id";

            var userMessage = new UserMessage
            {
                Sender = "Not CanonicalSenderName",
                SenderContext = new SenderContext
                {
                    SenderId = senderId,
                    SupplierId = NhsAppSupplierId
                }
            };

            _mockSenderService.Setup(s => s.GetSender(senderId))
                .ReturnsAsync(new SendersResult.InternalServerError());

            // Act
            await _systemUnderTest.UpdateWithCanonicalSenderName(new []{userMessage});

            // Assert
            userMessage.Sender.Should().Be(expectedSenderName);
            userMessage.SenderContext.SenderId.Should().Be(senderId);
            userMessage.SenderContext.SupplierId.Should().Be(NhsAppSupplierId);

            VerifyAll();
        }

        [TestMethod]
        public async Task UpdateWithCanonicalSenderName_UserMessageHasNoSenderContext_CreateSenderContextWithNhsAppIdAndSupplierId()
        {
            // Arrange
            const string expectedSenderName = "Nhs App Sender Name";

            var userMessage = new UserMessage
            {
                Sender = "Not CanonicalSenderName",
            };

            var canonicalSender = new SendersResponse
            {
                Senders = new List<Sender>
                {
                    new Sender
                    {
                        Id = NhsAppId,
                        Name = expectedSenderName
                    }
                }
            };

            _mockSenderService.Setup(s => s.GetSender(NhsAppId))
                .ReturnsAsync(new SendersResult.Found(canonicalSender));

            // Act
            await _systemUnderTest.UpdateWithCanonicalSenderName(new []{userMessage});

            // Assert
            userMessage.Sender.Should().Be(expectedSenderName);
            userMessage.SenderContext.SenderId.Should().Be(NhsAppId);
            userMessage.SenderContext.SupplierId.Should().Be(NhsAppSupplierId);

            VerifyAll();
        }

        [TestMethod]
        public async Task UpdateWithCanonicalSenderName_UserMessageHasNoSenderIdOrSupplierId_UsesNhsAppSenderIdandSupplierId()
        {
            // Arrange
            const string expectedSenderName = "Nhs App Sender Name";

            var userMessage = new UserMessage
            {
                Sender = "Not CanonicalSenderName",
                SenderContext = new SenderContext
                {
                    SenderId = null,
                    SupplierId = null
                }
            };

            var canonicalSender = new SendersResponse
            {
                Senders = new List<Sender>
                {
                    new Sender
                    {
                        Id = NhsAppId,
                        Name = expectedSenderName
                    }
                }
            };

            _mockSenderService.Setup(s => s.GetSender(NhsAppId))
                .ReturnsAsync(new SendersResult.Found(canonicalSender));

            // Act
            await _systemUnderTest.UpdateWithCanonicalSenderName(new []{userMessage});

            // Assert
            userMessage.Sender.Should().Be(expectedSenderName);
            userMessage.SenderContext.SenderId.Should().Be(NhsAppId);
            userMessage.SenderContext.SupplierId.Should().Be(NhsAppSupplierId);

            VerifyAll();
        }

        [TestMethod]
        [DataRow("409a0887-a946-4884-9796-45296a053192", "Parliament Street Medical Centre", "Y02847", "PARLIAMENT STREET MEDICAL CENTRE")]
        [DataRow("409a0887-a946-4884-9796-45296a053192", "Grange Farm Medical Centre", "Y03124", "GRANGE FARM MEDICAL CENTRE")]
        [DataRow("409a0887-a946-4884-9796-45296a053192", "Deer Park Family Medical Practice", "C84044", "DEER PARK FAMILY MEDICAL PRACTICE")]
        [DataRow("409a0887-a946-4884-9796-45296a053192", "Bilborough Medical Centre", "Y06356", "BILBOROUGH MEDICAL CENTRE")]
        [DataRow("409a0887-a946-4884-9796-45296a053192", "Bilborough MC", "Y06356", "BILBOROUGH MEDICAL CENTRE")]
        [DataRow("409a0887-a946-4884-9796-45296a053192", "Assarts Farm", "NO083", "BILBOROUGH MEDICAL CENTRE")]
        [DataRow("409a0887-a946-4884-9796-45296a053192", "Aspley Medical Centre", "C84091", "ASPLEY MEDICAL CENTRE")]
        [DataRow("409a0887-a946-4884-9796-45296a053192", "Aldborough Surgery", "D82628", "ALDBOROUGH SURGERY")]
        [DataRow("409a0887-a946-4884-9796-45296a053aaa", "Aldborough Surgery", NhsAppId, "NHS App")]
        [DataRow(null, "AnythingElse", NhsAppId, "NHS App")]
        [DataRow(null, null, NhsAppId, "NHS App")]
        public async Task UpdateWithCanonicalSenderName_UserMessageHasNoSenderId_UsesSupplierIdAndSenderNameToGetSenderId(
            string supplierId, string senderName, string expectedSenderId, string expectedSenderName)
        {
            // Arrange
            var userMessage = new UserMessage
            {
                Sender = senderName,
                SenderContext = new SenderContext
                {
                    SenderId = null,
                    SupplierId = supplierId
                }
            };

            var canonicalSender = new SendersResponse
            {
                Senders = new List<Sender>
                {
                    new Sender
                    {
                        Id = expectedSenderId,
                        Name = expectedSenderName
                    }
                }
            };

            _mockSenderService.Setup(s => s.GetSender(expectedSenderId))
                .ReturnsAsync(new SendersResult.Found(canonicalSender));

            // Act
            await _systemUnderTest.UpdateWithCanonicalSenderName(new []{userMessage});

            // Assert
            userMessage.Sender.Should().Be(expectedSenderName);
            userMessage.SenderContext.SenderId.Should().Be(expectedSenderId);

            VerifyAll();
        }

        [TestMethod]
        public async Task UpdateWithCanonicalSenderName_SenderServiceThrowsException_ThrowsException()
        {
            // Arrange
            var userMessage = new UserMessage()
            {
                Sender = "Not CanonicalSenderName",
                SenderContext = new SenderContext
                {
                    SenderId = NhsAppId
                }
            };

            _mockSenderService.Setup(s => s.GetSender(NhsAppId))
                .ThrowsAsync(new ArgumentException("This is a test"));

            // Act
            await FluentActions.Awaiting(() => _systemUnderTest.UpdateWithCanonicalSenderName(
                    new []{userMessage}))
                .Should().ThrowAsync<ArgumentException>();

            // Assert
            VerifyAll();
        }

        private void VerifyAll()
        {
            _mockSenderService.VerifyAll();
        }
    }
}