using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.UserInfoApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UserInfoApi.UnitTests.Repository
{
    [TestClass]
    public class UserInfoRepositoryTests
    {
        private IFixture _fixture;
        private UserInfoRepository _systemUnderTest;
        private Mock<IApiMongoClient<IMongoConfiguration>> _mockMongoClient;
        private Mock<IMongoCollection<UserAndInfo>> _mongoCollectionMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mongoCollectionMock = _fixture.Create<Mock<IMongoCollection<UserAndInfo>>>();

            var mongoDatabaseMock = _fixture.Create<Mock<IMongoDatabase>>();
            mongoDatabaseMock.Setup(x => x.GetCollection<UserAndInfo>(It.IsAny<string>(), null))
                .Returns(_mongoCollectionMock.Object);

            _mockMongoClient = _fixture.Freeze<Mock<IApiMongoClient<IMongoConfiguration>>>();
            _mockMongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null))
                .Returns(mongoDatabaseMock.Object);

            var repository = new MongoRepositoryBase<IMongoConfiguration, UserAndInfo>(
                _mockMongoClient.Object,
                new Mock<IMongoConfiguration>().Object,
                new Mock<ILogger<MongoRepositoryBase<IMongoConfiguration, UserAndInfo>>>().Object);

            _systemUnderTest = new UserInfoRepository(new Mock<ILogger<UserInfoRepository>>().Object, repository);
        }

        [TestMethod]
        public void Create_WhenUserInfoIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.Create(null);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("userAndInfo", StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task Create_WithUserInfo_ReturnsCreated()
        {
            // Arrange
            var userInfo = _fixture.Create<UserAndInfo>();

            var mongoCollectionMock = _fixture.Create<Mock<IMongoCollection<UserAndInfo>>>();

            var mongoDatabaseMock = _fixture.Create<Mock<IMongoDatabase>>();
            mongoDatabaseMock.Setup(x => x.GetCollection<UserAndInfo>(It.IsAny<string>(), null))
                .Returns(mongoCollectionMock.Object);

            _mockMongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null))
                .Returns(mongoDatabaseMock.Object);
            mongoCollectionMock.Setup(
                    x => x.ReplaceOneAsync(It.IsAny<FilterDefinition<UserAndInfo>>(),
                        It.IsAny<UserAndInfo>(),
                        new ReplaceOptions { IsUpsert = true },
                        default))
                .ReturnsAsync(_fixture.Create<Mock<ReplaceOneResult>>().Object);

            var mongoReplaceResult = new ReplaceOneResult.Acknowledged(1, 1, new BsonString("MockUpsertedId"));

          mongoCollectionMock.Setup(
                  x => x.ReplaceOneAsync(
                      It.IsAny<FilterDefinition<UserAndInfo>>(),
                      It.IsAny<UserAndInfo>(),
                      It.IsAny<ReplaceOptions>(),
                      It.IsAny<CancellationToken>()))
              .ReturnsAsync(mongoReplaceResult);

            // Act
            var result = await _systemUnderTest.Create(userInfo);

            // Assert
            result.Should().BeAssignableTo<RepositoryCreateResult<UserAndInfo>.Created>();
        }

        [TestMethod]
        public async Task FindByNhsLoginId_ReturnsInfo()
        {
            // Arrange
            var userInfo = _fixture.Create<UserAndInfo>();
            var cursorMock = MongoHelper.CreateCursorMockFind(_fixture, new[] { userInfo });

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserAndInfo>>(),
                    It.IsAny<FindOptions<UserAndInfo, UserAndInfo>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.FindByNhsLoginId(_fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.Found>()
                .Subject.Records.Should().BeEquivalentTo(userInfo);
        }

        [TestMethod]
        public async Task FindByNhsLoginId_WhenRecordDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var cursorMock = MongoHelper.CreateCursorMockFindNone<UserAndInfo>(_fixture);

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserAndInfo>>(),
                    It.IsAny<FindOptions<UserAndInfo, UserAndInfo>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.FindByNhsLoginId(_fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.NotFound>();
        }

        [TestMethod]
        public async Task FindByNhsNumber_ReturnsInfo()
        {
            // Arrange
            var userInfo1 = _fixture.Create<UserAndInfo>();
            var userInfo2 = _fixture.Create<UserAndInfo>();
            var expectedResults = new[] { userInfo1, userInfo2 };
            var cursorMock = MongoHelper.CreateCursorMockFind(_fixture, expectedResults);

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserAndInfo>>(),
                    It.IsAny<FindOptions<UserAndInfo, UserAndInfo>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.FindByNhsNumber(_fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.Found>()
                .Subject.Records.Should().BeEquivalentTo(expectedResults);
        }

        [TestMethod]
        public async Task FindByNhsNumber_WhenRecordDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var cursorMock = MongoHelper.CreateCursorMockFindNone<UserAndInfo>(_fixture);

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserAndInfo>>(),
                    It.IsAny<FindOptions<UserAndInfo, UserAndInfo>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.FindByNhsNumber(_fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.NotFound>();
        }

        [TestMethod]
        public async Task FindByOdsCode_ReturnsInfo()
        {
            // Arrange
            var userInfo1 = _fixture.Create<UserAndInfo>();
            var userInfo2 = _fixture.Create<UserAndInfo>();
            var expectedResults = new[] { userInfo1, userInfo2 };
            var cursorMock = MongoHelper.CreateCursorMockFind(_fixture, expectedResults);

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserAndInfo>>(),
                    It.IsAny<FindOptions<UserAndInfo, UserAndInfo>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.FindByOdsCode(_fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();

            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.Found>().Subject.Records
                .Should().BeEquivalentTo(expectedResults);
        }

        [TestMethod]
        public async Task FindByOdsCode_WhenRecordDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var cursorMock = MongoHelper.CreateCursorMockFindNone<UserAndInfo>(_fixture);

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserAndInfo>>(),
                    It.IsAny<FindOptions<UserAndInfo, UserAndInfo>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.FindByOdsCode(_fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.NotFound>();
        }
    }
}