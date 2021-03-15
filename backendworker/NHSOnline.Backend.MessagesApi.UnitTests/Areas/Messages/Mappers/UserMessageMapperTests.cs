using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Mappers;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages.Mappers
{
    [TestClass]
    public class UserMessageMapperTests
    {
        private const string NhsLoginId = "NHSLoginId";

        private UserMessageMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new UserMessageMapper(new Mock<ILogger<UserMessageMapper>>().Object);
        }

        [TestMethod]
        public void Map_BothRequestAndNhsLoginIdAreNull_ThrowsException()
        {
            // Act
            Action action = () => _systemUnderTest.Map(null, null);

            // Assert
            action.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(2)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("firstSource", StringComparison.Ordinal))
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("secondSource", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Map_RequestIsNull_ThrowsException()
        {
            // Act
            Action action = () => _systemUnderTest.Map(null, NhsLoginId);

            // Assert
            action.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("firstSource", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Map_NhsLoginIdIsNull_ThrowsException()
        {
            // Act
            Action action = () => _systemUnderTest.Map(new AddMessageRequest(), null);

            // Assert
            action.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("secondSource", StringComparison.Ordinal));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("   ")]
        public void Map_RequestAndContextCommunicationIdAreNullOrWhiteSpace_MappedCommunicationIdIsNull(
            string communicationId)
        {
            // Arrange
            var request = new AddMessageRequest
            {
                CommunicationId = communicationId,
                SenderContext = new AddMessageSenderContext
                {
                    CommunicationId = communicationId
                }
            };

            // Act
            var result = _systemUnderTest.Map(request, NhsLoginId);

            // Assert
            result.Should().NotBeNull();
            result.CommunicationId.Should().BeNull();
        }

        [DataTestMethod]
        public void Map_RequestSenderContextCommunicationIdHasValue_MapsCommunicationIdFromSenderContext()
        {
            // Arrange
            var request = new AddMessageRequest
            {
                CommunicationId = "RequestCommunicationId",
                SenderContext = new AddMessageSenderContext
                {
                    CommunicationId = "ContextCommunicationSenderId"
                }
            };

            // Act
            var result = _systemUnderTest.Map(request, NhsLoginId);

            // Assert
            result.Should().NotBeNull();
            result.CommunicationId.Should().Be("ContextCommunicationSenderId");
        }

        [DataTestMethod]
        public void Map_RequestCommunicationIdHasValueSenderContextIsNull_MapsCommunicationIdFromRequest()
        {
            // Arrange
            var request = new AddMessageRequest
            {
                CommunicationId = "RequestCommunicationId",
                SenderContext = new AddMessageSenderContext
                {
                    CommunicationId = null
                }
            };

            // Act
            var result = _systemUnderTest.Map(request, NhsLoginId);

            // Assert
            result.Should().NotBeNull();
            result.CommunicationId.Should().Be("RequestCommunicationId");
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("   ")]
        public void Map_RequestAndSenderContextTransmissionIdIsNullOrWhiteSpace_MappedTransmissionIdIsNull(
            string transmissionId)
        {
            // Arrange
            var request = new AddMessageRequest
            {
                TransmissionId = transmissionId,
                SenderContext = new AddMessageSenderContext
                {
                    TransmissionId = transmissionId
                }
            };

            // Act
            var result = _systemUnderTest.Map(request, NhsLoginId);

            // Assert
            result.Should().NotBeNull();
            result.TransmissionId.Should().BeNull();
        }

        [DataTestMethod]
        public void Map_RequestSenderContextTransmissionIdHasValue_MapsTransmissionIdFromSenderContext()
        {
            // Arrange
            var request = new AddMessageRequest
            {
                TransmissionId = "RequestTransmissionId",
                SenderContext = new AddMessageSenderContext
                {
                    TransmissionId = "ContextTransmissionSenderId"
                }
            };

            // Act
            var result = _systemUnderTest.Map(request, NhsLoginId);

            // Assert
            result.Should().NotBeNull();
            result.TransmissionId.Should().Be("ContextTransmissionSenderId");
        }

        [DataTestMethod]
        public void Map_RequestTransmissionIdHasValueSenderContextIsNull_MapsTransmissionIdFromRequest()
        {
            // Arrange
            var request = new AddMessageRequest
            {
                TransmissionId = "RequestTransmissionId",
                SenderContext = new AddMessageSenderContext
                {
                    TransmissionId = null
                }
            };

            // Act
            var result = _systemUnderTest.Map(request, NhsLoginId);

            // Assert
            result.Should().NotBeNull();
            result.TransmissionId.Should().Be("RequestTransmissionId");
        }

        [TestMethod]
        public void Map_RequestSenderContextIsNull_MappedSenderContextIsNull()
        {
            // Arrange
            var request = new AddMessageRequest
            {
                SenderContext = null
            };

            // Act
            var result = _systemUnderTest.Map(request, NhsLoginId);

            // Assert
            result.Should().NotBeNull();
            result.SenderContext.Should().BeNull();
        }

        [TestMethod]
        public void Map_AllFieldsAreSupplied_MapsAllFields()
        {
            // Arrange
            var nowTime = DateTime.UtcNow;
            var request = new AddMessageRequest
            {
                Body = "Body",
                CommunicationId = "CommunicationId",
                Sender = "Sender",
                TransmissionId = "TransmissionId",
                Version = 1,
                SenderContext = new AddMessageSenderContext
                {
                    CampaignId = "CampaignId",
                    CommunicationCreatedDateTime = nowTime,
                    CommunicationId = "CommunicationId",
                    NhsLoginId = "NhsLoginId",
                    NhsNumber = "NhsNumber",
                    OdsCode = "OdsCode",
                    RequestReference = "RequestReference",
                    SupplierId = "SupplierId",
                    TransmissionId = "TransmissionId"
                }
            };

            // Act
            var result = _systemUnderTest.Map(request, NhsLoginId);

            // Assert
            result.Should().NotBeNull();
            result.Body.Should().Be("Body");
            result.CommunicationId.Should().Be("CommunicationId");
            result.Sender.Should().Be("Sender");
            result.SentTime.Should().BeCloseTo(nowTime, TimeSpan.FromSeconds(1));
            result.TransmissionId.Should().Be("TransmissionId");
            result.Version.Should().Be(1);
            result.SenderContext.Should().BeEquivalentTo(new SenderContext
            {
                CampaignId = "CampaignId",
                CommunicationCreatedDateTime = nowTime,
                CommunicationId = "CommunicationId",
                NhsLoginId = "NhsLoginId",
                NhsNumber = "NhsNumber",
                OdsCode = "OdsCode",
                RequestReference = "RequestReference",
                SupplierId = "SupplierId",
                TransmissionId = "TransmissionId"
            });
        }
    }
}