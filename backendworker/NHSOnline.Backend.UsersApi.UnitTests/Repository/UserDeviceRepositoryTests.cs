using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.UsersApi.Repository;

namespace NHSOnline.Backend.UsersApi.UnitTests.Repository
{
    [TestClass]
    public class UserDeviceRepositoryTests
    {
        private UserDeviceRepository _systemUnderTest;
        private Mock<IRepository<UserDevice>> _mockRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new Mock<IRepository<UserDevice>>();

            _systemUnderTest = new UserDeviceRepository(new Mock<ILogger<UserDeviceRepository>>().Object, _mockRepository.Object);
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
            var userDevice = new UserDevice();

            _mockRepository.Setup(x => x.Create(userDevice, It.IsAny<string>()))
                .ReturnsAsync(new RepositoryCreateResult<UserDevice>.Created(userDevice));

            // Act
            var result = await _systemUnderTest.Create(userDevice);

            // Assert
            result.Should().BeOfType<RepositoryCreateResult<UserDevice>.Created>();
        }

        [TestMethod]
        public async Task Find_WhenDeviceIdRecordExists_ShouldReturnRecord()
        {
            // Arrange
            var userDevice = new UserDevice();
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserDevice, bool>>>(), It.IsAny<string>(), -1))
                .ReturnsAsync(new RepositoryFindResult<UserDevice>.Found(new []{ userDevice }));

            // Act
            var result = await _systemUnderTest.Find("nhsLoginId", "DeviceId");

            // Assert
            result.Should().BeOfType<RepositoryFindResult<UserDevice>.Found>()
                .Subject.Records.Should().BeEquivalentTo(userDevice);
        }

        [TestMethod]
        public async Task Find_WhenDeviceIdRecordDoesNotExist_ShouldNotReturnRecord()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserDevice, bool>>>(), It.IsAny<string>(),-1))
                .ReturnsAsync(new RepositoryFindResult<UserDevice>.NotFound());

            // Act
            var result = await _systemUnderTest.Find("nhsLoginId", "deviceId");

            // Assert
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
            _mockRepository.Setup(x =>
                    x.Delete(It.IsAny<Expression<Func<UserDevice, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync(new RepositoryDeleteResult<UserDevice>.Deleted());

            // Act
            var result = await _systemUnderTest.Delete("NhsLoginId", "DeviceId");

            // Assert
            result.Should().BeAssignableTo<RepositoryDeleteResult<UserDevice>.Deleted>();
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

        [TestMethod]
        public async Task UpdateOne_WhenRecordExists_ReturnsUpdatedResult()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Update(It.IsAny<Expression<Func<UserDevice, bool>>>(), It.IsAny<UpdateRecordBuilder<UserDevice>>(), It.IsAny<string>()))
                .ReturnsAsync(new RepositoryUpdateResult<UserDevice>.Updated());

            // Act
            var result = await _systemUnderTest.UpdateOne("NhsLoginId", "DeviceId", new UpdateRecordBuilder<UserDevice>());

            // Assert
            _mockRepository.VerifyAll();

            result.Should().BeOfType<RepositoryUpdateResult<UserDevice>.Updated>();
        }

        [TestMethod]
        [DataRow(null, null)]
        [DataRow(null, "test")]
        [DataRow("test", null)]
        public void UpdateOne_WithInvalidArguments_ThrowsException(string nhsLoginId, string deviceId)
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.UpdateOne(nhsLoginId, deviceId, new UpdateRecordBuilder<UserDevice>());

            // Assert
            act.Should().Throw<AggregateException>();
        }
    }
}