using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.PfsApi.Areas.Messages;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Messages
{
    [TestClass]
    public class SendersControllerTests : IDisposable
    {
        private SendersController _systemUnderTest;
        private Mock<ILogger<SendersController>> _mockLogger;
        private Mock<ISenderService> _mockSenderService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<SendersController>>();
            _mockSenderService = new Mock<ISenderService>(MockBehavior.Strict);
            _systemUnderTest = new SendersController(_mockLogger.Object, _mockSenderService.Object);
        }

        [TestMethod]
        public async Task GetSender_ByLastUpdatedBefore_WithInvalidLastUpdatedBeforeAndLimit_ReturnsBadRequest()
        {
            // Act
            var response = await _systemUnderTest.GetSenders(DateTime.Now, 0);

            // Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSender_ByLastUpdatedBefore_SenderServiceReturnsNoContentFound_EndPointReturnsNotFound()
        {
            // Arrange
            _mockSenderService.Setup(s => s.GetSenders(new DateTime(2022,1,2), 1))
                .ReturnsAsync(new SendersResult.None());

            // Act
            var response = await _systemUnderTest.GetSenders(new DateTime(2022,1,2), 1);

            // Assert
            response.Should()
                .BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<List<string>>()
                .Subject.Should().BeEquivalentTo(new List<string>());

            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSender_ByLastUpdatedBefore_SenderServiceReturnsBadGateway_EndPointReturnsBadGateway()
        {
            // Arrange
            _mockSenderService.Setup(s => s.GetSenders(new DateTime(2022,1,2), 1))
                .ReturnsAsync(new SendersResult.BadGateway());

            // Act
            var response = await _systemUnderTest.GetSenders(new DateTime(2022, 1, 2), 1);

            // Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSender_ByLastUpdatedBefore_SenderServiceReturnsInternalServerError_EndPointReturnsInternalServerError()
        {
            // Arrange
            _mockSenderService.Setup(s => s.GetSenders(new DateTime(2022,1,2), 1))
                .ReturnsAsync(new SendersResult.InternalServerError());

            // Act
            var response = await _systemUnderTest.GetSenders(new DateTime(2022,1,2), 1);

            // Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSender_ByLastUpdatedBefore_SenderServiceThrowsException_EndPointReturnsInternalServerError()
        {
            // Arrange
            _mockSenderService.Setup(s => s.GetSenders(new DateTime(2022,1,2), 1))
                .ThrowsAsync(new AggregateException());

            // Act
            var response = await _systemUnderTest.GetSenders(new DateTime(2022,1,2), 1);

            // Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSender_ByLastUpdatedBefore_SenderServiceReturnsRecord_EndPointReturnsSuccess()
        {
            // Arrange
            var senders = new SendersResponse
            {
                Senders = new List<Sender>
                {
                    new Sender { Id = "SENDER_ID_ONE", Name = "senderName" },
                    new Sender { Id = "SENDER_ID_TWO", Name = "senderName" }
                }
            };

            _mockSenderService.Setup(s => s.GetSenders(new DateTime(2022,1,2), 1))
                .ReturnsAsync(new SendersResult.Found(senders));

            // Act
            var response = await _systemUnderTest.GetSenders(new DateTime(2022,1,2), 1);

            // Assert
            response.Should()
                .BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<List<string>>()
                .Subject.Should().BeEquivalentTo(senders.Senders.Select(x => x.Id).ToList());
            VerifyMocks();
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null Sender Id")]
        [DataRow("", DisplayName = "Empty Sender Id")]
        [DataRow(" ", DisplayName = "Whitespace Sender Id")]
        public async Task GetSender_WithInvalidSenderId_ReturnsBadRequest(string senderId)
        {
            // Act
            var response = await _systemUnderTest.GetSender(senderId);

            // Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSender_SenderServiceReturnsNotFound_EndPointReturnsNotFound()
        {
            // Arrange
            _mockSenderService.Setup(s => s.GetSender("senderId"))
                .ReturnsAsync(new SendersResult.None());

            // Act
            var response = await _systemUnderTest.GetSender("senderId");

            // Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSender_SenderServiceReturnsBadGateway_EndPointReturnsBadGateway()
        {
            // Arrange
            _mockSenderService.Setup(s => s.GetSender("senderId"))
                .ReturnsAsync(new SendersResult.BadGateway());

            // Act
            var response = await _systemUnderTest.GetSender("senderId");

            // Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSender_SenderServiceReturnsInternalServerError_EndPointReturnsInternalServerError()
        {
            // Arrange
            _mockSenderService.Setup(s => s.GetSender("senderId"))
                .ReturnsAsync(new SendersResult.InternalServerError());

            // Act
            var response = await _systemUnderTest.GetSender("senderId");

            // Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSender_SenderServiceThrowsException_EndPointReturnsInternalServerError()
        {
            // Arrange
            _mockSenderService.Setup(s => s.GetSender("senderId"))
                .ThrowsAsync(new AggregateException());

            // Act
            var response = await _systemUnderTest.GetSender("senderId");

            // Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSender_SenderServiceReturnsRecord_EndPointReturnsSuccess()
        {
            // Arrange
            var sendersResponse = new SendersResponse
            {
                Senders = new List<Sender>
                {
                    new Sender { Id = "SENDER_ID", Name = "sender_name" }
                }
            };

            _mockSenderService.Setup(s => s.GetSender("sender_Id"))
                .ReturnsAsync(new SendersResult.Found(sendersResponse));

            // Act
            var response = await _systemUnderTest.GetSender("sender_Id");

            // Assert
            var objectResult = response.Should().BeAssignableTo<OkObjectResult>().Subject;
            var resultValue = objectResult.Value.Should().BeAssignableTo<Sender>().Subject;
            resultValue.Id.Should().Be("SENDER_ID");
            resultValue.Name.Should().Be("sender_name");

            VerifyMocks();
        }

        [TestMethod]
        public async Task Post_SenderServiceThrowsException_EndPointReturnsInternalServerError()
        {
            // Arrange
            var sender = new Sender() { Id = "senderId", Name = "senderName" };

            _mockSenderService.Setup(s => s.Create(sender))
                .ThrowsAsync(new AggregateException());

            // Act
            var response = await _systemUnderTest.Post(sender);

            // Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            VerifyMocks();
        }

        [TestMethod]
        public async Task Post_SenderServiceReturnsBadGateway_EndPointReturnsBadGateway()
        {
            // Arrange
            var sender = new Sender() { Id = "senderId", Name = "senderName" };

            _mockSenderService.Setup(s => s.Create(sender))
                .ReturnsAsync(new SenderPostResult.BadGateway());

            // Act
            var response = await _systemUnderTest.Post(sender);

            // Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            VerifyMocks();
        }

        [DataTestMethod]
        [DataRow(null, "Sender Name", DisplayName = "Null Sender Id")]
        [DataRow("", "Sender Name", DisplayName = "Empty Sender Id")]
        [DataRow("  ", "Sender Name", DisplayName = "Whitespace Sender Id")]
        [DataRow("Sender Id", null, DisplayName = "Null Sender Name")]
        [DataRow("Sender Id", "", DisplayName = "Empty Sender Name")]
        [DataRow("Sender Id", "  ", DisplayName = "Whitespace Sender Name")]
        public async Task Post_WithInValidSender_EndPointReturnsBadRequest(string senderId, string senderName)
        {
            // Arrange
            var sender = new Sender { Id = senderId, Name = senderName };

            //Ac
            var response = await _systemUnderTest.Post(sender);

            // Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            VerifyMocks();
        }

        [TestMethod]
        public async Task Post_SenderServiceInternalServerError_EndPointReturnsInternalServerError()
        {
            // Arrange
            var sender = new Sender() { Id = "senderId", Name = "senderName" };

            _mockSenderService.Setup(s => s.Create(sender))
                .ReturnsAsync(new SenderPostResult.InternalServerError());

            // Act
            var response = await _systemUnderTest.Post(sender);

            // Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            VerifyMocks();
        }

        [TestMethod]
        public async Task Post_SenderServiceReturnsSuccess_EndPointReturnsCreated()
        {
            // Arrange
            var sender = new Sender { Id = "sender_Id", Name = "sender_name" };

            _mockSenderService.Setup(s => s.Create(sender))
                .ReturnsAsync(new SenderPostResult.Created(new DbSender{Id = "SENDER_ID", Name = "sender_name"}));

            // Act
            var response = await _systemUnderTest.Post(sender);

            // Assert
            var createdResult = response.Should().BeAssignableTo<CreatedResult>().Subject;
            var resultValue = createdResult.Value.Should().BeAssignableTo<Sender>().Subject;
            resultValue.Id.Should().Be("SENDER_ID");
            resultValue.Name.Should().Be("sender_name");

            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _mockSenderService.VerifyAll();
        }

        public void Dispose()
        {
            _systemUnderTest.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}