using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Cache.Messages;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Messages.UnitTests.Areas.Messages
{
    [TestClass]
    public class SenderServiceGetSendersTests
    {
        private ISenderService _systemUnderTest;
        private Mock<ILogger<SenderService>> _mockLogger;
        private Mock<ISenderRepository> _mockSenderRepository;
        private Mock<ISenderCacheProvider> _mockCacheProvider;
        private Mock<IMapper<Sender, DbSender>> _mockRequestMapper;
        private Mock<IMapper<List<DbSender>, SendersResponse>> _mockSendersMapper;
        private Mock<IMapper<DbSender, SendersResponse>> _mockSenderMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<SenderService>>();
            _mockSenderRepository = new Mock<ISenderRepository>(MockBehavior.Strict);
            _mockCacheProvider = new Mock<ISenderCacheProvider>(MockBehavior.Strict);
            _mockRequestMapper = new Mock<IMapper<Sender, DbSender>>(MockBehavior.Strict);
            _mockSenderMapper = new Mock<IMapper<DbSender, SendersResponse>>(MockBehavior.Strict);
            _mockSendersMapper = new Mock<IMapper<List<DbSender>, SendersResponse>>(MockBehavior.Strict);

            _systemUnderTest = new SenderService(
                _mockSenderRepository.Object,
                _mockCacheProvider.Object,
                _mockRequestMapper.Object,
                _mockSendersMapper.Object,
                _mockSenderMapper.Object,
                _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetSenders_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var lastUpdatedBefore = DateTime.UtcNow.AddHours(-1);
            const int limit = 100;

            _mockSenderRepository.Setup(s => s.Find(lastUpdatedBefore, limit))
                .ThrowsAsync(new AggregateException());

            // Act
            var response = await _systemUnderTest.GetSenders(lastUpdatedBefore, limit);

            // Assert
            response.Should().BeAssignableTo<SendersResult.InternalServerError>();
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSenders_RepositoryReturnsRepositoryError_ReturnsBadGateway()
        {
            // Arrange
            var lastUpdatedBefore = DateTime.UtcNow.AddHours(-1);
            const int limit = 100;

            _mockSenderRepository.Setup(s => s.Find(lastUpdatedBefore, limit))
                .ReturnsAsync(new RepositoryFindResult<DbSender>.RepositoryError());

            // Act
            var response = await _systemUnderTest.GetSenders(lastUpdatedBefore, limit);

            // Assert
            response.Should().BeAssignableTo<SendersResult.BadGateway>();
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSenders_RepositoryReturnsNotFound_ReturnsNone()
        {
            // Arrange
            var lastUpdatedBefore = DateTime.UtcNow.AddHours(-1);
            const int limit = 100;

            _mockSenderRepository.Setup(s => s.Find(lastUpdatedBefore, limit))
                .ReturnsAsync(new RepositoryFindResult<DbSender>.NotFound());

            // Act
            var response = await _systemUnderTest.GetSenders(lastUpdatedBefore, limit);

            // Assert
            response.Should().BeAssignableTo<SendersResult.None>();
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSenders_RepositoryReturnsFound_ReturnsSender()
        {
            // Arrange
            var lastUpdatedBefore = DateTime.UtcNow.AddHours(-1);
            const int limit = 100;

            var dbSenders = new List<DbSender>();
            var sendersResponse = new SendersResponse
            {
                Senders = new List<Sender> { new Sender { Id = "ID", Name = "Name" } }
            };

            _mockSenderRepository.Setup(s => s.Find(lastUpdatedBefore, limit))
                .ReturnsAsync(new RepositoryFindResult<DbSender>.Found(dbSenders));

            _mockSendersMapper
                .Setup(x => x.Map(dbSenders))
                .Returns(sendersResponse);

            // Act
            var response = await _systemUnderTest.GetSenders(lastUpdatedBefore, limit);

            // Assert
            var senderObjectFromService = response.Should().BeAssignableTo<SendersResult.Found>();
            senderObjectFromService.Subject.Response.Should().Be(sendersResponse);
            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _mockSenderRepository.VerifyAll();
            _mockSenderMapper.VerifyAll();
            _mockSendersMapper.VerifyAll();
        }
    }
}