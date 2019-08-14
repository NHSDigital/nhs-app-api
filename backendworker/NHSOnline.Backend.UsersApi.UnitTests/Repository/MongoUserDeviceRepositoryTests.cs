using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.UnitTests.Repository
{
    [TestClass]
    public class MongoUserDeviceRepositoryTests
    {
        private IFixture _fixture;
        private MongoUserDeviceRepository _systemUnderTest;
        private Mock<IMongoClient> _mockMongoClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockMongoClient = _fixture.Freeze<Mock<IMongoClient>>();

            _systemUnderTest = _fixture.Create<MongoUserDeviceRepository>();
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

            var mongoCollectionMock = _fixture.Create<Mock<IMongoCollection<UserDevice>>>();
            mongoCollectionMock.Setup(x => x.InsertOneAsync(userDevice, null, default))
                .Returns(Task.CompletedTask);

            var mongoDatabaseMock = _fixture.Create<Mock<IMongoDatabase>>();
            mongoDatabaseMock.Setup(x => x.GetCollection<UserDevice>(It.IsAny<string>(), null))
                .Returns(mongoCollectionMock.Object);

            _mockMongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null))
                .Returns(mongoDatabaseMock.Object);

            // Act
            await _systemUnderTest.Create(userDevice);

            // Assert
            mongoCollectionMock.Verify(x => x.InsertOneAsync(userDevice, null, default));
        }
    }
}