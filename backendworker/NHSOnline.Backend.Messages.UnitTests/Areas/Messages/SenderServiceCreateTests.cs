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
    public class SenderServiceCreateTests
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
        public async Task Create_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var incomingSender = new Sender { Id = "ID", Name = "name" };
            var mappedSender = new DbSender();

            _mockRequestMapper.Setup(x => x.Map(incomingSender))
                .Returns(mappedSender);

            _mockSenderRepository.Setup(s => s.CreateOrUpdate(mappedSender))
                .ThrowsAsync(new AggregateException());

            // Act
            var response = await _systemUnderTest.Create(incomingSender);

            // Assert
            response.Should().BeAssignableTo<SenderPostResult.InternalServerError>();
            VerifyMocks();
        }

        [TestMethod]
        public async Task Create_RepositoryReturnsRepositoryError_ReturnsBadGateway()
        {
            // Arrange
            var incomingSender = new Sender { Id = "ID", Name = "name" };
            var mappedSender = new DbSender();

            _mockRequestMapper.Setup(x => x.Map(incomingSender))
                .Returns(mappedSender);

            _mockSenderRepository.Setup(s => s.CreateOrUpdate(mappedSender))
                .ReturnsAsync(new RepositoryCreateResult<DbSender>.RepositoryError());

            // Act
            var response = await _systemUnderTest.Create(incomingSender);

            // Assert
            response.Should().BeAssignableTo<SenderPostResult.BadGateway>();
            VerifyMocks();
        }

        [TestMethod]
        public async Task Create_RepositoryReturnsCreated_ReturnsCreated()
        {
            // Arrange
            var incomingSender = new Sender { Id = "ID", Name = "name" };
            var mappedSender = new DbSender();

            _mockRequestMapper.Setup(x => x.Map(incomingSender))
                .Returns(mappedSender);

            _mockSenderRepository.Setup(s => s.CreateOrUpdate(mappedSender))
                .ReturnsAsync(new RepositoryCreateResult<DbSender>.Created(mappedSender));

            // Act
            var response = await _systemUnderTest.Create(incomingSender);

            // Assert
            var resultObject = response.Should().BeAssignableTo<SenderPostResult.Created>().Subject;
            resultObject.DbSender.Should().Be(mappedSender);
            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _mockSenderRepository.VerifyAll();
            _mockSenderMapper.VerifyAll();
            _mockSendersMapper.VerifyAll();
            _mockRequestMapper.VerifyAll();
        }
    }
}