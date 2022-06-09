using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Repository.SqlApi;

namespace NHSOnline.Backend.Messages.UnitTests.Repository
{
    [TestClass]
    public class SenderRepositoryTests
    {
        private ISenderRepository _systemUnderTest;
        private Mock<ILogger<SenderRepository>> _mockLogger;
        private Mock<ISqlApiRepository<DbSender>> _mockSqlApiRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<SenderRepository>>();
            _mockSqlApiRepository = new Mock<ISqlApiRepository<DbSender>>(MockBehavior.Strict);

            _systemUnderTest = new SenderRepository(_mockLogger.Object, _mockSqlApiRepository.Object);
        }

        [TestMethod]
        public void CreateOrUpdate_SenderIsNull_ThrowsException()
        {
            //Act
            Func<Task> act = async () => await _systemUnderTest.CreateOrUpdate(null);

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("sender");
        }

        [TestMethod]
        public async Task CreateOrUpdate_SenderIsValid_AddsToCollection()
        {
            //Arrange
            var sender = new DbSender { Id = "senderId", Name = "senderName" };
            _mockSqlApiRepository.Setup(s => s.CreateOrUpdate(sender, "Sender", sender.Id))
                .ReturnsAsync(new RepositoryCreateResult<DbSender>.Created(sender));

            //Act
            var result = await _systemUnderTest.CreateOrUpdate(sender);

            //Assert
            result.Should().BeOfType<RepositoryCreateResult<DbSender>.Created>();
            _mockSqlApiRepository.VerifyAll();
        }

        [TestMethod]
        public void CreateOrUpdate_SqlApiThrowsException_RethrowsException()
        {
            //Arrange
            _mockSqlApiRepository.Setup(s =>
                    s.CreateOrUpdate(It.IsAny<DbSender>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AggregateException("exception"));

            //Act
            Func<Task> act = async () => await _systemUnderTest.CreateOrUpdate(new DbSender());

            // Assert
            act.Should().ThrowAsync<AggregateException>();
            _mockSqlApiRepository.VerifyAll();
        }

        [TestMethod]
        public void Find_SenderIdIsNull_ThrowsException()
        {
            //Act
            Func<Task> act = async () => await _systemUnderTest.Find(null);

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("sender");
        }

        [TestMethod]
        public async Task Find_SenderIsValid_SuccessfullyRetrievesRecord()
        {
            //Arrange
            var sender = new DbSender { Id = "senderId", Name = "senderName" };

            _mockSqlApiRepository.Setup(s => s.Find(sender.Id, sender.Id,
                    "Sender"))
                .ReturnsAsync(new RepositoryFindResult<DbSender>.Found(new[] { sender }));

            //Act
            var result = await _systemUnderTest.Find(sender.Id);

            //Assert
            result.Should().BeOfType<RepositoryFindResult<DbSender>.Found>()
                .Subject.Records.Should().BeEquivalentTo(new[] { sender });

            _mockSqlApiRepository.VerifyAll();
        }

        [TestMethod]
        public void Find_SqlApiThrowsException_RethrowsException()
        {
            //Arrange
            _mockSqlApiRepository.Setup(s =>
                    s.Find(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AggregateException("exception"));

            //Act
            Func<Task> act = async () => await _systemUnderTest.Find("senderId");

            // Assert
            act.Should().ThrowAsync<AggregateException>();
        }
    }
}