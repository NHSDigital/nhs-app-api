using System;
using System.Collections.Generic;
using System.Linq;
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
    public class SenderServiceGetSenderTests
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
        public async Task GetSender_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockCacheProvider
                .Setup(c => c.GetSender(It.IsAny<string>()))
                .Returns((Sender) null);

            _mockSenderRepository.Setup(s => s.Find("SENDER_ID"))
                .ThrowsAsync(new AggregateException());

            // Act
            var response = await _systemUnderTest.GetSender("sender_id");

            // Assert
            response.Should().BeAssignableTo<SendersResult.InternalServerError>();
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSender_RepositoryReturnsRepositoryError_ReturnsBadGateway()
        {
            // Arrange
            _mockCacheProvider
                .Setup(c => c.GetSender(It.IsAny<string>()))
                .Returns((Sender) null);

            _mockSenderRepository.Setup(s => s.Find("SENDER_ID"))
                .ReturnsAsync(new RepositoryFindResult<DbSender>.RepositoryError());

            // Act
            var response = await _systemUnderTest.GetSender("sender_id");

            // Assert
            response.Should().BeAssignableTo<SendersResult.BadGateway>();
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSender_RepositoryReturnsNotFound_ReturnsNone()
        {
            // Arrange
            _mockCacheProvider
                .Setup(c => c.GetSender(It.IsAny<string>()))
                .Returns((Sender) null);

            _mockSenderRepository.Setup(s => s.Find("SENDER_ID"))
                .ReturnsAsync(new RepositoryFindResult<DbSender>.NotFound());

            // Act
            var response = await _systemUnderTest.GetSender("sender_id");

            // Assert
            response.Should().BeAssignableTo<SendersResult.None>();
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSender_WhenFoundInCache_ReturnsSender()
        {
            var sender = new Sender { Id = "SENDER_ID", Name = "name" };

            // Arrange
            _mockCacheProvider
                .Setup(c => c.GetSender(sender.Id))
                .Returns(sender);

            // Act
            var response = await _systemUnderTest.GetSender("sender_Id");

            // Assert
            var senderObjectFromService = response.Should().BeAssignableTo<SendersResult.Found>().Subject;

            senderObjectFromService.Response.Senders.Single().Id.Should().Be(sender.Id);
            senderObjectFromService.Response.Senders.Single().Name.Should().Be(sender.Name);
            VerifyMocks();
        }

        [TestMethod]
        public async Task GetSender_WhenNotFoundInCacheButFoundInRepository_ReturnsSender()
        {
            var sender = new Sender { Id = "SENDER_ID", Name = "name" };
            var dbSender = new DbSender();
            var sendersResponse = new SendersResponse
            {
                Senders = new List<Sender> { sender }
            };

            // Arrange
            _mockCacheProvider
                .Setup(c => c.GetSender("SENDER_ID"))
                .Returns((Sender) null);

            _mockSenderRepository
                .Setup(s => s.Find("SENDER_ID"))
                .ReturnsAsync(new RepositoryFindResult<DbSender>.Found(new List<DbSender> { dbSender }));

            Sender cachedSender = null;
            _mockCacheProvider
                .Setup(cs => cs.SetSender(It.IsAny<Sender>()))
                .Callback<Sender>(ssc => cachedSender = ssc);

            _mockSenderMapper
                .Setup(x => x.Map(dbSender))
                .Returns(sendersResponse);

            // Act
            var response = await _systemUnderTest.GetSender("sender_Id");

            // Assert
            var senderObjectFromService = response.Should().BeAssignableTo<SendersResult.Found>();
            senderObjectFromService.Subject.Response.Should().Be(sendersResponse);
            cachedSender.Should().BeEquivalentTo(sender);
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