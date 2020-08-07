using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.UserInfoApi.Repository;

namespace NHSOnline.Backend.UserInfoApi.UnitTests.Repository
{
    [TestClass]
    public class UserInfoRepositoryTests
    {
        private UserInfoRepository _systemUnderTest;
        private Mock<IRepository<UserAndInfo>> _mockRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new Mock<IRepository<UserAndInfo>>();

            _systemUnderTest = new UserInfoRepository(new Mock<ILogger<UserInfoRepository>>().Object, _mockRepository.Object);
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
            var userInfo = new UserAndInfo();

            _mockRepository.Setup(x => x.CreateOrUpdate(
                    It.IsAny<Expression<Func<UserAndInfo, bool>>>(),
                    userInfo,
                    It.IsAny<string>()))
                .ReturnsAsync(new RepositoryCreateResult<UserAndInfo>.Created(userInfo));

            // Act
            var result = await _systemUnderTest.Create(userInfo);

            // Assert
            result.Should().BeAssignableTo<RepositoryCreateResult<UserAndInfo>.Created>();
        }

        [TestMethod]
        public async Task FindByNhsLoginId_ReturnsInfo()
        {
            // Arrange
            var userInfo = new UserAndInfo();
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserAndInfo, bool>>>(), It.IsAny<string>(), -1))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(new[] { userInfo }));

            // Act
            var result = await _systemUnderTest.FindByNhsLoginId("nhsLoginId");

            // Assert
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.Found>()
                .Subject.Records.Should().BeEquivalentTo(userInfo);
        }

        [TestMethod]
        public async Task FindByNhsLoginId_WhenRecordDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserAndInfo, bool>>>(), It.IsAny<string>(), -1))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.NotFound());

            // Act
            var result = await _systemUnderTest.FindByNhsLoginId("nhsLoginId");

            // Assert
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.NotFound>();
        }

        [TestMethod]
        public async Task FindByNhsNumber_ReturnsInfo()
        {
            // Arrange
            var userInfo1 = new UserAndInfo();
            var userInfo2 = new UserAndInfo();
            var expectedResults = new[] { userInfo1, userInfo2 };
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserAndInfo, bool>>>(), It.IsAny<string>(), -1))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(expectedResults));

            // Act
            var result = await _systemUnderTest.FindByNhsNumber("NhsLoginId");

            // Assert
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.Found>()
                .Subject.Records.Should().BeEquivalentTo(expectedResults);
        }

        [TestMethod]
        public async Task FindByNhsNumber_WhenRecordDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserAndInfo, bool>>>(), It.IsAny<string>(), -1))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.NotFound());

            // Act
            var result = await _systemUnderTest.FindByNhsNumber("NhsLoginId");

            // Assert
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.NotFound>();
        }

        [TestMethod]
        public async Task FindByOdsCode_ReturnsInfo()
        {
            // Arrange
            var userInfo1 = new UserAndInfo();
            var userInfo2 = new UserAndInfo();
            var expectedResults = new[] { userInfo1, userInfo2 };
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserAndInfo, bool>>>(), It.IsAny<string>(), -1))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(expectedResults));

            // Act
            var result = await _systemUnderTest.FindByOdsCode("NhsLoginId");

            // Assert
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.Found>().Subject.Records
                .Should().BeEquivalentTo(expectedResults);
        }

        [TestMethod]
        public async Task FindByOdsCode_WhenRecordDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserAndInfo, bool>>>(), It.IsAny<string>(), -1))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.NotFound());

            // Act
            var result = await _systemUnderTest.FindByOdsCode("NhsLoginId");

            // Assert
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.NotFound>();
        }
    }
}