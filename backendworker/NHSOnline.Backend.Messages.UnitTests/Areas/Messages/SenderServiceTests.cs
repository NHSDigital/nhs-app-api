using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Mappers;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Messages.UnitTests.Areas.Messages
{
    [TestClass]
    public class SenderServiceTests
    {
        private ISenderService _systemUnderTest;
        private Mock<ILogger<SenderService>> _mockLogger;
        private Mock<ISenderRepository> _mockSenderRepository;
        private IMapper<Sender, DbSender> _requestMapper;
        private IMapper<DbSender, Sender> _responseMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<SenderService>>();
            _mockSenderRepository = new Mock<ISenderRepository>(MockBehavior.Strict);
            _requestMapper = new SenderRequestMapper();
            _responseMapper = new SenderResponseMapper();
            _systemUnderTest = new SenderService(_mockSenderRepository.Object, _mockLogger.Object, _requestMapper,
                _responseMapper);
        }

        [TestMethod]
        public async Task Create_RepositoryThrowsException_ReturnsInternalServerError()
        {
            //Arrange
            _mockSenderRepository.Setup(s => s.CreateOrUpdate(It.IsAny<DbSender>()))
                .ThrowsAsync(new AggregateException());

            //Act
            var response = await _systemUnderTest.Create(new Sender { Id = "id", Name = "name" });

            //Assert
            response.Should().BeAssignableTo<SenderPostResult.InternalServerError>();
            VerifyMocks();
        }

        [TestMethod]
        public async Task Create_RepositoryReturnsRepositoryError_ReturnsBadGateway()
        {
            //Arrange
            _mockSenderRepository.Setup(s => s.CreateOrUpdate(It.IsAny<DbSender>()))
                .ReturnsAsync(new RepositoryCreateResult<DbSender>.RepositoryError());

            //Act
            var response = await _systemUnderTest.Create(new Sender { Id = "id", Name = "name" });

            //Assert
            response.Should().BeAssignableTo<SenderPostResult.BadGateway>();
            VerifyMocks();
        }

        [TestMethod]
        public async Task Create_RepositoryReturnsSuccess_ReturnsCreated()
        {
            var sender = new Sender { Id = "id", Name = "name" };
            var dbSender = _requestMapper.Map(sender);

            //Arrange
            _mockSenderRepository.Setup(s => s.CreateOrUpdate(It.IsAny<DbSender>()))
                .ReturnsAsync(new RepositoryCreateResult<DbSender>.Created(dbSender));

            //Act
            var response = await _systemUnderTest.Create(sender);

            //Assert
            var senderObjectFromService = response.Should().BeAssignableTo<SenderPostResult.Success>();
            senderObjectFromService.Subject.DbSender.Should().BeEquivalentTo(dbSender);
            VerifyMocks();
        }

        [TestMethod]
        public async Task Find_RepositoryThrowsException_ReturnsInternalServerError()
        {
            //Arrange
            _mockSenderRepository.Setup(s => s.Find(It.IsAny<string>()))
                .ThrowsAsync(new AggregateException());

            //Act
            var response = await _systemUnderTest.GetSender("sender");

            //Assert
            response.Should().BeAssignableTo<SenderResult.InternalServerError>();
            VerifyMocks();
        }

        [TestMethod]
        public async Task Find_RepositoryReturnsRepositoryError_ReturnsBadGateway()
        {
            //Arrange
            _mockSenderRepository.Setup(s => s.Find(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryFindResult<DbSender>.RepositoryError());

            //Act
            var response = await _systemUnderTest.GetSender("sender");

            //Assert
            response.Should().BeAssignableTo<SenderResult.BadGateway>();
            VerifyMocks();
        }

        [TestMethod]
        public async Task Find_RepositoryReturnsNotFound_ReturnsNotFound()
        {
            //Arrange
            _mockSenderRepository.Setup(s => s.Find(It.IsAny<string>()))
                .ReturnsAsync(new RepositoryFindResult<DbSender>.NotFound());

            //Act
            var response = await _systemUnderTest.GetSender("sender");

            //Assert
            response.Should().BeAssignableTo<SenderResult.NotFound>();
            VerifyMocks();
        }

        [TestMethod]
        public async Task Find_RepositoryReturnsSuccess_ReturnsSender()
        {
            var sender = new Sender { Id = "senderId", Name = "name" };
            var dbSender = _requestMapper.Map(sender);

            //Arrange
            _mockSenderRepository.Setup(s => s.Find("senderId".ToUpperInvariant()))
                .ReturnsAsync(new RepositoryFindResult<DbSender>.Found(new List<DbSender> { dbSender }));

            //Act
            var response = await _systemUnderTest.GetSender("senderId");

            //Assert
            var senderObjectFromService = response.Should().BeAssignableTo<SenderResult.Found>();
            senderObjectFromService.Subject.Response.Id.Should().Be(sender.Id.ToUpperInvariant());
            senderObjectFromService.Subject.Response.Name.Should().Be(sender.Name);
            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _mockSenderRepository.VerifyAll();
        }
    }
}