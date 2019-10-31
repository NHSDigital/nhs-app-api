using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Mappers;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages.Mappers
{
    [TestClass]
    public class JsonPatchDocumentMapperTests
    {
        private IFixture _fixture;
        private JsonPatchDocumentMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new MessagesApiCustomization());

            _systemUnderTest = _fixture.Create<JsonPatchDocumentMapper>();
        }
  
        [TestMethod]
        public void Map_WithReadOperationTrue_MapsToResponse()
        {
            // Arrange
            var op = "add";
            var path = "/read";
            var mappedPath = "/readTime";
            var value = true;
            
            var readOperation = _fixture.Build<Operation<Message>>()
                .With(x => x.op, op)
                .With(x => x.path, path)
                .With(x => x.value, value)
                .Create();

            var jsonPatch = _fixture.Create<JsonPatchDocument<Message>>();
            jsonPatch.Operations.Add(readOperation);

            // Act
            var result = _systemUnderTest.Map(jsonPatch);

            // Assert
            result.Should().BeAssignableTo<JsonPatchDocument<UserMessage>>();
            result.Operations.Should().HaveCount(1);
            
            result.Operations.First().op.Should().Be(op);
            result.Operations.First().path.Should().Be(mappedPath);
            result.Operations.First().value.Should().NotBeNull().And.BeOfType<DateTime>();
        }
        
        [TestMethod]
        public void Map_WithReadOperationFalse_MapsToResponse()
        {
            // Arrange
            var op = "add";
            var path = "/read";
            var mappedPath = "/readTime";
            var value = false;
            
            var readOperation = _fixture.Build<Operation<Message>>()
                .With(x => x.op, op)
                .With(x => x.path, path)
                .With(x => x.value, value)
                .Create();

            var jsonPatch = _fixture.Create<JsonPatchDocument<Message>>();
            jsonPatch.Operations.Add(readOperation);

            // Act
            var result = _systemUnderTest.Map(jsonPatch);

            // Assert
            result.Should().BeAssignableTo<JsonPatchDocument<UserMessage>>();
            result.Operations.Should().HaveCount(1);
            
            result.Operations.First().op.Should().Be(op);
            result.Operations.First().path.Should().Be(mappedPath);
            result.Operations.First().value.Should().BeNull();
        }
        
        [DataTestMethod]
        [DataRow("add", "/path", null, "value")]
        [DataRow("copy", "/path", "/from", null)]
        [DataRow("move", "/path", "/from", null)]
        [DataRow("remove", "/path", null, null)]
        [DataRow("replace", "/path", null, "value")]
        [DataRow("replace", "/readTime", null, "value")]
        public void Map_WithNormalOperation_MapsToResponse(string op, string path, string from, Object value)
        {
            // Arrange
            var messageOperation = _fixture.Build<Operation<Message>>()
                .With(x => x.op, op)
                .With(x => x.path, path)
                .With(x => x.from, from)
                .With(x => x.value, value)
                .Create();

            var jsonPatch = _fixture.Create<JsonPatchDocument<Message>>();
            jsonPatch.Operations.Add(messageOperation);

            // Act
            var result = _systemUnderTest.Map(jsonPatch);

            // Assert
            result.Operations.Should().NotBeEmpty();
            result.Operations.Should().HaveCount(1);
            result.Should().BeEquivalentTo(new JsonPatchDocument<UserMessage>()
            {
                Operations = { ConvertToUserMessageOperation(messageOperation) }
            });
        }
        
        [TestMethod]
        public void Map_WithMultipleOperations_MapsToResponse()
        {
            // Arrange
            var addMessageOperation = _fixture.Build<Operation<Message>>()
                .With(x => x.op, "add")
                .With(x => x.path, "/path")
                .With(x => x.value, "value")
                .Create();
            
            var copyMessageOperation = _fixture.Build<Operation<Message>>()
                .With(x => x.op, "copy")
                .With(x => x.path, "/path")
                .With(x => x.from, "/from")
                .Create();
            
            var removeMessageOperation = _fixture.Build<Operation<Message>>()
                .With(x => x.op, "remove")
                .With(x => x.path, "/path")
                .Create();

            var jsonPatch = _fixture.Create<JsonPatchDocument<Message>>();
            jsonPatch.Operations.AddRange(new List<Operation<Message>>()
            {
                addMessageOperation, copyMessageOperation, removeMessageOperation
            });

            // Act
            var result = _systemUnderTest.Map(jsonPatch);

            // Assert
            result.Operations.Should().NotBeEmpty();
            result.Operations.Should().HaveCount(3);
            result.Should().BeEquivalentTo(new JsonPatchDocument<UserMessage>()
            {
                Operations = { ConvertToUserMessageOperation(addMessageOperation), 
                    ConvertToUserMessageOperation(copyMessageOperation), 
                    ConvertToUserMessageOperation(removeMessageOperation) }
            });
        }
        
        [TestMethod]
        public void Map_WithNoOperations_MapsToEmptyResponse()
        {
            // Act
            var result = _systemUnderTest.Map(new JsonPatchDocument<Message>());

            // Assert
            result.Operations.Should().BeEmpty();
            result.Should().BeAssignableTo<JsonPatchDocument<UserMessage>>();
        }
        
        [TestMethod]
        public void Map_WhenOperationsAreNull_ThrowsArgumentNullException()
        {
            // Act
            Action act = () => _systemUnderTest.Map(null);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("source");
        }
        
        private Operation<UserMessage> ConvertToUserMessageOperation(Operation<Message> messageOperation)
        {
            return new Operation<UserMessage>(
                messageOperation.op,
                messageOperation.path,
                messageOperation.from, 
                messageOperation.value);
        }
        
    }
}