using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Messages.Areas.Messages.Mappers;
using NHSOnline.Backend.Messages.Repository;

namespace NHSOnline.Backend.Messages.UnitTests.Areas.Messages.Mappers
{
    [TestClass]
    public class SendersResponseMapperTests
    {
        private SendersResponseMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new SendersResponseMapper(new Mock<ILogger<SendersResponseMapper>>().Object);
        }

        [TestMethod]
        public void Map_SingleSenderIsNull_Throws()
        {
            // Act
            Action act = () => _systemUnderTest.Map((DbSender) null);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("source");
        }

        [TestMethod]
        public void Map_SingleSender_MapsToResponse()
        {
            // Arrange
            var source = new DbSender
            {
                Id = "SENDER_ID",
                Name = "Sender Name",
                Timestamp = DateTime.Now.AddHours(-3)
            };

            // Act
            var response = _systemUnderTest.Map(source);

            // Assert
            var responseSender = response.Senders.Should().ContainSingle().Subject;
            responseSender.Id.Should().Be("SENDER_ID");
            responseSender.Name.Should().Be("Sender Name");
        }

        [TestMethod]
        public void Map_MultipleSendersIsNull_Throws()
        {
            // Act
            Action act = () => _systemUnderTest.Map((List<DbSender>) null);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("source");
        }

        [TestMethod]
        public void Map_MultipleSenders_MapsToResponse()
        {
            // Arrange
            var source = new List<DbSender>()
            {
                new DbSender
                {
                    Id = "SENDER_ID_1",
                    Name = "Sender Name 1",
                    Timestamp = DateTime.Now.AddHours(-4)
                },
                new DbSender
                {
                    Id = "SENDER_ID_2",
                    Name = "Sender Name 2",
                    Timestamp = DateTime.Now.AddHours(-3)
                }
            };

            // Act
            var response = _systemUnderTest.Map(source);

            // Assert
            response.Senders.Should().HaveCount(2);

            response.Senders.First().Id.Should().Be("SENDER_ID_1");
            response.Senders.First().Name.Should().Be("Sender Name 1");
            response.Senders.Last().Id.Should().Be("SENDER_ID_2");
            response.Senders.Last().Name.Should().Be("Sender Name 2");
        }
    }
}