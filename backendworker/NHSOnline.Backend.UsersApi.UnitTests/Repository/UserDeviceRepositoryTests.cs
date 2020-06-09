using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.UsersApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Repository
{
    [TestClass]
    public class UserDeviceRepositoryTests
    {
        private IFixture _fixture;
        private UserDeviceRepository _systemUnderTest;
        private Mock<IMongoCollection<UserDevice>> _mongoCollectionMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mongoCollectionMock = _fixture.Create<Mock<IMongoCollection<UserDevice>>>();

            var mongoDatabaseMock = _fixture.Create<Mock<IMongoDatabase>>();
            mongoDatabaseMock.Setup(x => x.GetCollection<UserDevice>(It.IsAny<string>(), null))
                .Returns(_mongoCollectionMock.Object);

            var mockMongoClient = _fixture.Freeze<Mock<IApiMongoClient<IMongoConfiguration>>>();
            mockMongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null))
                .Returns(mongoDatabaseMock.Object);

            var repository = new MongoRepository<IMongoConfiguration, UserDevice>(
                mockMongoClient.Object,
                new Mock<IMongoConfiguration>().Object,
                new Mock<ILogger<MongoRepository<IMongoConfiguration, UserDevice>>>().Object);

            _systemUnderTest = new UserDeviceRepository(new Mock<ILogger<UserDeviceRepository>>().Object, repository);
        }

        [TestMethod]
        public void Create_WhenUserDeviceIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.Create(null);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("userDevice", StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task Create_WithUserDevice_AddsToCollection()
        {
            // Arrange
            var userDevice = _fixture.Create<UserDevice>();
            _mongoCollectionMock.Setup(x => x.InsertOneAsync(userDevice, null, default))
                .Returns(Task.CompletedTask);

            // Act
            await _systemUnderTest.Create(userDevice);

            // Assert
            _mongoCollectionMock.VerifyAll();
        }

        [TestMethod]
        public async Task Find_WhenDeviceIdRecordExists_ShouldReturnRecord()
        {
            // Arrange
            var userDevice = _fixture.Create<UserDevice>();
            var cursorMock = MongoHelper.CreateCursorMockFind(_fixture, new[]{userDevice});

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserDevice>>(),
                    It.IsAny<FindOptions<UserDevice, UserDevice>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.Find(_fixture.Create<string>(), userDevice.DeviceId);

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeOfType<RepositoryFindResult<UserDevice>.Found>()
                .Subject.Records.Should().BeEquivalentTo(userDevice);
        }

        [TestMethod]
        public async Task Find_WhenDeviceIdRecordDoesNotExist_ShouldNotReturnRecord()
        {
            // Arrange
            var cursorMock = MongoHelper.CreateCursorMockFindNone<UserDevice>(_fixture);

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserDevice>>(),
                    It.IsAny<FindOptions<UserDevice, UserDevice>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.Find(_fixture.Create<string>(), _fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeAssignableTo<RepositoryFindResult<UserDevice>.NotFound>();
        }

        [TestMethod]
        [DataRow(null, null)]
        [DataRow(null, "test")]
        [DataRow("test", null)]
        public void Find_WithInvalidArguments_ThrowsException(string nhsLoginId, string deviceId)
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.Find(nhsLoginId, deviceId);

            // Assert
            act.Should().Throw<AggregateException>();
        }

        [TestMethod]
        public async Task Delete_Success()
        {
            // Arrange
            _mongoCollectionMock
                .Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<UserDevice>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            await _systemUnderTest.Delete(_fixture.Create<string>(), _fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
        }

        [TestMethod]
        [DataRow(null, null)]
        [DataRow(null, "test")]
        [DataRow("test", null)]
        public void Delete_WithInvalidArguments_ThrowsException(string nhsLoginId, string deviceId)
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.Delete(nhsLoginId, deviceId);

            // Assert
            act.Should().Throw<AggregateException>();
        }
    }
}