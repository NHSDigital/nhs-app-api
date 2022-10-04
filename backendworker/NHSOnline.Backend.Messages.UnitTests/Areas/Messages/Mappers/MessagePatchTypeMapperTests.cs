using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Messages.Areas.Messages.Mappers;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.UnitTests.Areas.Messages.Mappers
{
    [TestClass]
    public class MessagePatchTypeMapperTests
    {
        private MessagePatchTypeMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest =
                new MessagePatchTypeMapper();
        }

        [TestMethod]
        public void Map_TypeRead_WhenOperationIsAddAndPathIsRead()
        {
            // Act
            var result = _systemUnderTest.Map(new Operation<Message> { op = "add", path = "/read" });

            //Assert
            result.Should().Be(MessagePatchType.Read);
        }

        [TestMethod]
        public void Map_TypeReply_WhenOperationIsAddAndPathIsResponse()
        {
            // Act
            var result = _systemUnderTest.Map(new Operation<Message> { op = "add", path = "/reply/response" });

            //Assert
            result.Should().Be(MessagePatchType.Reply);
        }

        [TestMethod]
        public void Map_TypeReplyStatus_WhenOperationIsAddAndPathIsResponse()
        {
            // Act
            var result = _systemUnderTest.Map(new Operation<Message> { op = "add", path = "/reply/status" });

            //Assert
            result.Should().Be(MessagePatchType.ReplyStatus);
        }

        [TestMethod]
        public void Map_TypeUnknown_WhenOperationAndPathNotMapped()
        {
            // Act
            var result = _systemUnderTest.Map(new Operation<Message> { op = "newop", path = "/another/path" });

            //Assert
            result.Should().Be(MessagePatchType.Unknown);
        }
    }
}