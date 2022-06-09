using System;
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
        private SenderController _systemUnderTest;
        private Mock<ILogger<SenderController>> _mockLogger;
        private Mock<ISenderService> _mockSenderService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<SenderController>>();
            _mockSenderService = new Mock<ISenderService>(MockBehavior.Strict);

            _systemUnderTest = new SenderController(_mockLogger.Object, _mockSenderService.Object);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null Sender Id")]
        [DataRow("", DisplayName = "Empty Sender Id")]
        [DataRow(" ", DisplayName = "Whitespace Sender Id")]
        public async Task GetSender_WithInValidSenderId_ReturnsBadRequest(string senderId)
        {
            //Act
            var response = await _systemUnderTest.GetSender(senderId);

            //Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            VerifyMocks();
        }

        [TestMethod]
        public async Task Get_SenderServiceReturnsNotFound_EndPointReturnsNotFound()
        {
            //Arrange
            _mockSenderService.Setup(s => s.GetSender("senderId"))
                .ReturnsAsync(new SenderResult.NotFound());

            //Act
            var response = await _systemUnderTest.GetSender("senderId");

            //Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            VerifyMocks();
        }

        [TestMethod]
        public async Task Get_SenderServiceReturnsBadGateway_EndPointReturnsBadGateway()
        {
            //Arrange
            _mockSenderService.Setup(s => s.GetSender("senderId"))
                .ReturnsAsync(new SenderResult.BadGateway());

            //Act
            var response = await _systemUnderTest.GetSender("senderId");

            //Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            VerifyMocks();
        }

        [TestMethod]
        public async Task Get_SenderServiceReturnsInternalServerError_EndPointReturnsInternalServerError()
        {
            //Arrange
            _mockSenderService.Setup(s => s.GetSender("senderId"))
                .ReturnsAsync(new SenderResult.InternalServerError());

            //Act
            var response = await _systemUnderTest.GetSender("senderId");

            //Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            VerifyMocks();
        }

        [TestMethod]
        public async Task Get_SenderServiceThrowsException_EndPointReturnsInternalServerError()
        {
            //Arrange
            _mockSenderService.Setup(s => s.GetSender("senderId"))
                .ThrowsAsync(new AggregateException());

            //Act
            var response = await _systemUnderTest.GetSender("senderId");

            //Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            VerifyMocks();
        }

        [TestMethod]
        public async Task Get_SenderServiceReturnsRecord_EndPointReturnsSuccess()
        {
            //Arrange
            var sender = new Sender() { Id = "SENDER_ID", Name = "senderName" };

            _mockSenderService.Setup(s => s.GetSender("sender_Id"))
                .ReturnsAsync(new SenderResult.Found(sender));

            //Act
            var response = await _systemUnderTest.GetSender("sender_Id");

            //Assert
            response.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<Sender>()
                .Subject.Should().BeEquivalentTo(sender);
            VerifyMocks();
        }

        [TestMethod]
        public async Task Post_SenderServiceThrowsException_EndPointReturnsInternalServerError()
        {
            //Arrange
            var sender = new Sender() { Id = "senderId", Name = "senderName" };

            _mockSenderService.Setup(s => s.Create(sender))
                .ThrowsAsync(new AggregateException());

            //Act
            var response = await _systemUnderTest.Post(sender);

            //Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            VerifyMocks();
        }

        [TestMethod]
        public async Task Post_SenderServiceReturnsBadGateway_EndPointReturnsBadGateway()
        {
            //Arrange
            var sender = new Sender() { Id = "senderId", Name = "senderName" };

            _mockSenderService.Setup(s => s.Create(sender))
                .ReturnsAsync(new SenderPostResult.BadGateway());

            //Act
            var response = await _systemUnderTest.Post(sender);

            //Assert
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
            //Arrange
            var sender = new Sender { Id = senderId, Name = senderName };

            //Ac
            var response = await _systemUnderTest.Post(sender);

            //Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            VerifyMocks();
        }

        [TestMethod]
        public async Task Post_SenderServiceInternalServerError_EndPointReturnsInternalServerError()
        {
            //Arrange
            var sender = new Sender() { Id = "senderId", Name = "senderName" };

            _mockSenderService.Setup(s => s.Create(sender))
                .ReturnsAsync(new SenderPostResult.InternalServerError());

            //Act
            var response = await _systemUnderTest.Post(sender);

            //Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            VerifyMocks();
        }

        [TestMethod]
        public async Task Post_SenderServiceReturnsSuccess_EndPointReturnsNoContent()
        {
            //Arrange
            var sender = new Sender() { Id = "senderId", Name = "senderName" };

            _mockSenderService.Setup(s => s.Create(sender))
                .ReturnsAsync(new SenderPostResult.Created(new DbSender()));

            //Act
            var response = await _systemUnderTest.Post(sender);

            //Assert
            var statusCodeResult = response.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status204NoContent);
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