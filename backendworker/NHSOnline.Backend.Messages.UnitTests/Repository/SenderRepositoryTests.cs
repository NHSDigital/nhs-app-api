using System;
using System.Collections.Generic;
using System.Linq;
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
        private Mock<IMessagesConfiguration> _mockMessagesConfiguration;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<SenderRepository>>();
            _mockSqlApiRepository = new Mock<ISqlApiRepository<DbSender>>(MockBehavior.Strict);
            _mockMessagesConfiguration = new Mock<IMessagesConfiguration>(MockBehavior.Strict);

            _systemUnderTest = new SenderRepository(
                _mockLogger.Object,
                _mockSqlApiRepository.Object,
                _mockMessagesConfiguration.Object);
        }

        [TestMethod]
        public void CreateOrUpdate_SenderIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.CreateOrUpdate(null);

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("sender");

            VerifyMocks();
        }

        [TestMethod]
        public async Task CreateOrUpdate_SenderIsValid_AddsToCollection()
        {
            // Arrange
            var sender = new DbSender { Id = "senderId", Name = "senderName" };
            _mockSqlApiRepository.Setup(s => s.CreateOrUpdate(sender, "Sender", sender.Id))
                .ReturnsAsync(new RepositoryCreateResult<DbSender>.Created(sender));

            // Act
            var result = await _systemUnderTest.CreateOrUpdate(sender);

            // Assert
            result.Should().BeOfType<RepositoryCreateResult<DbSender>.Created>();

            VerifyMocks();
        }

        [TestMethod]
        public void CreateOrUpdate_SqlApiThrowsException_RethrowsException()
        {
            // Arrange
            _mockSqlApiRepository.Setup(s =>
                    s.CreateOrUpdate(It.IsAny<DbSender>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AggregateException("exception"));

            // Act
            Func<Task> act = async () => await _systemUnderTest.CreateOrUpdate(new DbSender());

            // Assert
            act.Should().ThrowAsync<AggregateException>();

            VerifyMocks();
        }

        [TestMethod]
        public void FindBySenderId_SenderIdIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.Find(null);

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("sender");

            VerifyMocks();
        }

        [TestMethod]
        public async Task FindBySenderId_SenderIsValid_SuccessfullyRetrievesRecord()
        {
            // Arrange
            var sender = new DbSender { Id = "senderId", Name = "senderName" };

            _mockSqlApiRepository.Setup(s => s.Find(sender.Id, sender.Id,
                    "Sender"))
                .ReturnsAsync(new RepositoryFindResult<DbSender>.Found(new[] { sender }));

            // Act
            var result = await _systemUnderTest.Find(sender.Id);

            // Assert
            result.Should().BeOfType<RepositoryFindResult<DbSender>.Found>()
                .Subject.Records.Should().BeEquivalentTo(new[] { sender });

            VerifyMocks();
        }

        [TestMethod]
        public void FindBySenderId_SqlApiThrowsException_RethrowsException()
        {
            // Arrange
            _mockSqlApiRepository.Setup(s =>
                    s.Find(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new AggregateException("exception"));

            // Act
            Func<Task> act = async () => await _systemUnderTest.Find("senderId");

            // Assert
            act.Should().ThrowAsync<AggregateException>();

            VerifyMocks();
        }

        [TestMethod]
        public async Task FindByLastUpdated_ValidInputs_SuccessfullyRetrievesRecords()
        {
            // Arrange
            var sender1 = new DbSender
            {
                Id = "senderId1",
                Name = "senderName",
                Timestamp = new DateTime(2022,1,20)
            };

            var sender2 = new DbSender
            {
                Id = "senderId2",
                Name = "senderName",
                Timestamp = new DateTime(2022,1,1)
            };

            var sender3 = new DbSender
            {
                Id = "senderId3",
                Name = "senderName",
                Timestamp = new DateTime(2022,1,2)
            };

            _mockMessagesConfiguration.SetupGet(x => x.SenderIdNhsApp)
                .Returns("Y0E3J");

            Func<IQueryable<DbSender>, IQueryable<DbSender>> capturedQuery = null;

            _mockSqlApiRepository
                .Setup(s => s.Find(It.IsAny<Func<IQueryable<DbSender>, IQueryable<DbSender>>>(), "Sender"))
                .Callback<Func<IQueryable<DbSender>, IQueryable<DbSender>>, string>((query, recordName) => capturedQuery = query)
                .ReturnsAsync(new RepositoryFindResult<DbSender>.Found(new List<DbSender> {
                    sender2, sender3
                }));

            // Act
            var result = await _systemUnderTest.Find(new DateTime(2022,1,5), 1);

            var capturedQueryResult = capturedQuery.Invoke(new List<DbSender>
            {
                sender1, sender2, sender3
            }.AsQueryable());

            // Assert
            capturedQueryResult.Should().BeOfType<EnumerableQuery<DbSender>>()
                .Subject.Should().BeEquivalentTo(new List<DbSender> {
                    sender2
                }.AsEnumerable());

            result.Should().BeOfType<RepositoryFindResult<DbSender>.Found>()
                .Subject.Records.Should().BeEquivalentTo(new List<DbSender> {
                    sender2, sender3
                });

            VerifyMocks();
        }

        [TestMethod]
        public async Task FindByLastUpdated_StaleRecordsIncludesNhsAppSender_NhsAppSenderExcludedFromResults()
        {
            // Arrange
            var sender1 = new DbSender
            {
                Id = "Y0E3J",
                Name = "NHS App",
                Timestamp = new DateTime(2000,1,1)
            };

            _mockMessagesConfiguration.SetupGet(x => x.SenderIdNhsApp)
                .Returns("Y0E3J");

            Func<IQueryable<DbSender>, IQueryable<DbSender>> capturedQuery = null;

            _mockSqlApiRepository.Setup(s => s.Find(It.IsAny<Func<IQueryable<DbSender>, IQueryable<DbSender>>>(), "Sender"))
                .Callback<Func<IQueryable<DbSender>, IQueryable<DbSender>>, string>((query, recordName) => capturedQuery = query)
                .ReturnsAsync(new RepositoryFindResult<DbSender>.NotFound());

            // Act
            var result = await _systemUnderTest.Find(new DateTime(2022,1,1), 1);

            var capturedQueryResult = capturedQuery.Invoke(new List<DbSender>
            {
                sender1
            }.AsQueryable());

            // Assert
            capturedQueryResult.Should().BeOfType<EnumerableQuery<DbSender>>()
                .Subject.Should().BeEquivalentTo(new List<DbSender>().AsEnumerable());

            result.Should().BeOfType<RepositoryFindResult<DbSender>.NotFound>();

            VerifyMocks();
        }

        [TestMethod]
        public void FindByLastUpdated_ButSqlApiThrowsException_ThrowException()
        {
            // Arrange
            _mockMessagesConfiguration.SetupGet(x => x.SenderIdNhsApp)
                .Returns("Y0E3J");

            _mockSqlApiRepository.Setup(s =>
                    s.Find(It.IsAny<Func<IQueryable<DbSender>, IQueryable<DbSender>>>(), "Sender"))
                .ThrowsAsync(new AggregateException("exception"));

            // Act
            Func<Task> act = async () => await _systemUnderTest.Find(new DateTime(2022,1,5), 1);

            // Assert
            act.Should().ThrowAsync<AggregateException>();

            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _mockSqlApiRepository.VerifyAll();
            _mockMessagesConfiguration.VerifyAll();
        }
    }
}