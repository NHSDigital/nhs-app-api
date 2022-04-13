using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Messages.Areas.Messages.Mappers;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.UnitTests.Areas.Messages.Mappers
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
                Sender = "Sender",
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
            result.Sender.Should().Be("Sender");
            result.SentTime.Should().BeCloseTo(nowTime, TimeSpan.FromSeconds(1));
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