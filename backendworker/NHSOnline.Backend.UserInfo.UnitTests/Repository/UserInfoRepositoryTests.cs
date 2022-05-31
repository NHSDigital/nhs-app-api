using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.UserInfo.Repository;

namespace NHSOnline.Backend.UserInfo.UnitTests.Repository
{
    [TestClass]
    public class UserInfoRepositoryTests
    {
        private UserInfoRepository _systemUnderTest;
        private Mock<IUserAndInfoSqlApiRepositoryFactory> _mockRepositoryFactory;
        private Mock<IRepository<UserAndInfo>> _mockPrimaryRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepositoryFactory = new Mock<IUserAndInfoSqlApiRepositoryFactory>(MockBehavior.Strict);
            _mockPrimaryRepository = new Mock<IRepository<UserAndInfo>>(MockBehavior.Strict);

            _systemUnderTest = new UserInfoRepository(
                new Mock<ILogger<UserInfoRepository>>().Object,
                _mockPrimaryRepository.Object,
                _mockRepositoryFactory.Object);
        }

        [TestMethod]
        public void CreateOrUpdatePrimary_WhenUserInfoIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.CreateOrUpdatePrimary(null);

            // Assert
            _mockRepositoryFactory.VerifyAll();
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("userAndInfo");

        }

        [TestMethod]
        public void CreateOrUpdateNhsNumberRecord_WhenUserInfoIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.CreateOrUpdateNhsNumberRecord(null);

            // Assert
            VerifyAll();
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("userAndInfo");

        }

        [TestMethod]
        public void CreateOrUpdateOdsCodeRecord_WhenUserInfoIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.CreateOrUpdateOdsCodeRecord(null);

            // Assert
            _mockRepositoryFactory.VerifyAll();
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("userAndInfo");

        }

        [TestMethod]
        public async Task CreateOrUpdatePrimary_WithUserInfo_ReturnsCreated()
        {
            // Arrange
            var userInfo = new UserAndInfo();

            _mockPrimaryRepository.Setup(x => x
                    .CreateOrUpdate(
                    It.IsAny<Expression<Func<UserAndInfo, bool>>>(),
                    userInfo,
                    It.IsAny<string>()))
                .ReturnsAsync(new RepositoryCreateResult<UserAndInfo>.Created(userInfo));

            // Act
            var result = await _systemUnderTest.CreateOrUpdatePrimary(userInfo);

            // Assert
            VerifyAll();
            result.Should().BeAssignableTo<RepositoryCreateResult<UserAndInfo>.Created>();
        }

        [TestMethod]
        public async Task CreateOrUpdateNhsNumber_WithUserInfo_ReturnsCreated()
        {
            // Arrange
            var partitionKeyValue = "TestPartitionKeyValue";
            var userInfo = new UserAndInfo
            {
                Info = new Info()
            };
            userInfo.Info.NhsNumber = partitionKeyValue;

            _mockRepositoryFactory.Setup(x => x
                    .GetUserAndInfoSqlApiRepository(UserAndInfoSqlApiRepositoryFactory.UserAndInfoRepositoryKey.NhsNumber)
                    .CreateOrUpdate(userInfo, It.IsAny<string>(), partitionKeyValue))
                .ReturnsAsync(new RepositoryCreateResult<UserAndInfo>.Created(userInfo));

            // Act
            var result = await _systemUnderTest.CreateOrUpdateNhsNumberRecord(userInfo);

            // Assert
            VerifyAll();
            result.Should().BeAssignableTo<RepositoryCreateResult<UserAndInfo>.Created>();
        }

        [TestMethod]
        public async Task CreateOrUpdateOdsCode_WithUserInfo_ReturnsCreated()
        {
            // Arrange
            var partitionKeyValue = "TestPartitionKeyValue";
            var userInfo = new UserAndInfo
            {
                Info = new Info()
            };
            userInfo.Info.OdsCode = partitionKeyValue;

            _mockRepositoryFactory.Setup(x => x
                    .GetUserAndInfoSqlApiRepository(UserAndInfoSqlApiRepositoryFactory.UserAndInfoRepositoryKey.OdsCode)
                    .CreateOrUpdate(userInfo, It.IsAny<string>(), partitionKeyValue))
                .ReturnsAsync(new RepositoryCreateResult<UserAndInfo>.Created(userInfo));

            // Act
            var result = await _systemUnderTest.CreateOrUpdateOdsCodeRecord(userInfo);

            // Assert
            VerifyAll();
            result.Should().BeAssignableTo<RepositoryCreateResult<UserAndInfo>.Created>();
        }

        [TestMethod]
        public async Task FindByNhsLoginId_WhenRecordExists_ReturnsInfo()
        {
            // Arrange
            var userInfo = new UserAndInfo();
            _mockPrimaryRepository.Setup(x => x
                    .Find(It.IsAny<Expression<Func<UserAndInfo, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(new[] { userInfo }));

            // Act
            var result = await _systemUnderTest.FindByNhsLoginId("Nhs Login Id");

            // Assert
            VerifyAll();
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.Found>()
                .Subject.Records.Should().BeEquivalentTo(new []{userInfo});
        }

        [TestMethod]
        public async Task FindByNhsLoginId_WhenRecordDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockPrimaryRepository.Setup(x => x
                    .Find(It.IsAny<Expression<Func<UserAndInfo, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.NotFound());

            // Act
            var result = await _systemUnderTest.FindByNhsLoginId("Nhs Login Id");

            // Assert
            VerifyAll();
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.NotFound>();
        }

        [TestMethod]
        public async Task FindByNhsNumber_WhenRecordExists_ReturnsInfo()
        {
            // Arrange
            var userInfo = new UserAndInfo();
            var expectedResults = new[] { userInfo };

            _mockPrimaryRepository.Setup(x => x
                    .Find(It.IsAny<Expression<Func<UserAndInfo, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(expectedResults));

            // Act
            var result = await _systemUnderTest.FindByNhsNumber("Nhs Number");

            // Assert
            VerifyAll();
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.Found>()
                .Subject.Records.Should().BeEquivalentTo(expectedResults);
        }

        [TestMethod]
        public async Task FindByNhsNumber_WhenRecordDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockPrimaryRepository.Setup(x => x
                    .Find(It.IsAny<Expression<Func<UserAndInfo, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.NotFound());

            // Act
            var result = await _systemUnderTest.FindByNhsNumber("Nhs Number");

            // Assert
            VerifyAll();
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.NotFound>();
        }

        [TestMethod]
        public async Task FindByOdsCode_WhenRecordExists_ReturnsInfo()
        {
            // Arrange
            var userInfo1 = new UserAndInfo();
            var userInfo2 = new UserAndInfo();
            var expectedResults = new[] { userInfo1, userInfo2 };
            _mockPrimaryRepository.Setup(x => x
                    .Find(It.IsAny<Expression<Func<UserAndInfo, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(expectedResults));

            // Act
            var result = await _systemUnderTest.FindByOdsCode("Ods Code");

            // Assert
            VerifyAll();
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.Found>().Subject.Records
                .Should().BeEquivalentTo(expectedResults);
        }

        [TestMethod]
        public async Task FindByOdsCode_WhenRecordDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockPrimaryRepository.Setup(x => x
                    .Find(It.IsAny<Expression<Func<UserAndInfo, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.NotFound());

            // Act
            var result = await _systemUnderTest.FindByOdsCode("Ods Code");

            // Assert
            VerifyAll();
            result.Should().BeAssignableTo<RepositoryFindResult<UserAndInfo>.NotFound>();
        }

        [TestMethod]
        [DataRow(null, null, DisplayName = "Null Nhs Number and Nhs Login Id")]
        [DataRow("", "", DisplayName = "Empty Ods Code and Nhs Login Id")]
        [DataRow(null, "Nhs Login Id", DisplayName = "Null Nhs Number")]
        [DataRow("", "Nhs Login Id", DisplayName = "Empty Nhs Number")]
        [DataRow("Nhs Number", null, DisplayName = "Null Nhs Login Id")]
        [DataRow("Nhs Number", "", DisplayName = "Empty Nhs Login Id")]
        public void DeleteNhsNumberRecord_WithInvalidArguments_ThrowsException(string nhsNumber, string nhsLoginId)
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.DeleteNhsNumberRecord(nhsNumber, nhsLoginId);

            // Assert
            VerifyAll();
            act.Should().ThrowAsync<AggregateException>();
        }

        [TestMethod]
        public async Task DeleteNhsNumberRecord_WhenNhsNumberAndNhsLoginIdDoesNotExist_ShouldNotDeleteRecord()
        {
            // Arrange
            var nhsNumber = "Nhs Number";
            var nhsLoginId = "Nhs Login Id";

            _mockRepositoryFactory.Setup(x => x
                    .GetUserAndInfoSqlApiRepository(UserAndInfoSqlApiRepositoryFactory.UserAndInfoRepositoryKey.NhsNumber)
                    .Delete(
                        It.Is<string>(s => s == nhsLoginId),
                        It.Is<string>(s => s== nhsNumber),
                        It.IsAny<string>()))
                .ReturnsAsync(new RepositoryDeleteResult<UserAndInfo>.NotFound());

            // Act
            var result = await _systemUnderTest.DeleteNhsNumberRecord(nhsNumber, nhsLoginId);

            // Assert
            VerifyAll();
            result.Should().BeAssignableTo<RepositoryDeleteResult<UserAndInfo>.NotFound>();
        }

        [TestMethod]
        public async Task DeleteNhsNumberRecord_WhenRecordExists_ReturnsDeleted()
        {
            // Arrange
            var nhsNumber = "Nhs Number";
            var nhsLoginId = "Nhs Login Id";

            _mockRepositoryFactory.Setup(x => x
                    .GetUserAndInfoSqlApiRepository(UserAndInfoSqlApiRepositoryFactory.UserAndInfoRepositoryKey.NhsNumber)
                    .Delete(It.Is<string>(s => s == nhsLoginId),
                        It.Is<string>(s => s== nhsNumber),
                        It.IsAny<string>()))
                .ReturnsAsync(new RepositoryDeleteResult<UserAndInfo>.Deleted());

            // Act
            var result = await _systemUnderTest.DeleteNhsNumberRecord(nhsNumber, nhsLoginId);

            // Assert
            VerifyAll();
            result.Should().BeAssignableTo<RepositoryDeleteResult<UserAndInfo>.Deleted>();
        }

        [TestMethod]
        [DataRow(null, null, DisplayName = "Null Ods Code and Nhs Login Id")]
        [DataRow("", "", DisplayName = "Empty Ods Code and Nhs Login Id")]
        [DataRow(null, "Nhs Login Id", DisplayName = "Null Ods Code")]
        [DataRow("", "Nhs Login Id", DisplayName = "Empty Ods Code")]
        [DataRow("Ods Code", null, DisplayName = "Null Nhs Login Id")]
        [DataRow("Ods Code", "", DisplayName = "Empty Nhs Login Id")]
        public void DeleteOdsCodeRecord_WithInvalidArguments_ThrowsException(string odsCode, string nhsLoginId)
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.DeleteOdsCodeRecord(odsCode, nhsLoginId);

            // Assert
            VerifyAll();
            act.Should().ThrowAsync<AggregateException>();
        }

        [TestMethod]
        public async Task DeleteOdsCodeRecord_WhenOdsCodeAndNhsLoginIdDoesNotExist_ShouldNotDeleteRecord()
        {
            // Arrange
            var odsCode = "ODS Code";
            var nhsLoginId = "Nhs Login Id";

            _mockRepositoryFactory.Setup(x => x
                    .GetUserAndInfoSqlApiRepository(UserAndInfoSqlApiRepositoryFactory.UserAndInfoRepositoryKey.OdsCode)
                    .Delete(It.Is<string>(s => s == nhsLoginId),
                        It.Is<string>(s => s== odsCode),
                        It.IsAny<string>()))
                .ReturnsAsync(new RepositoryDeleteResult<UserAndInfo>.NotFound());

            // Act
            var result = await _systemUnderTest.DeleteOdsCodeRecord(odsCode, nhsLoginId);

            // Assert
            VerifyAll();
            result.Should().BeAssignableTo<RepositoryDeleteResult<UserAndInfo>.NotFound>();
        }

        [TestMethod]
        public async Task DeleteOdsCodeRecord_WhenRecordExists_ReturnsDeleted()
        {
            // Arrange
            var odsCode = "ODS Code";
            var nhsLoginId = "Nhs Login Id";

            _mockRepositoryFactory.Setup(x => x
                    .GetUserAndInfoSqlApiRepository(UserAndInfoSqlApiRepositoryFactory.UserAndInfoRepositoryKey.OdsCode)
                    .Delete(nhsLoginId, odsCode, It.IsAny<string>()))
                .ReturnsAsync(new RepositoryDeleteResult<UserAndInfo>.Deleted());

            // Act
            var result = await _systemUnderTest.DeleteOdsCodeRecord(odsCode, nhsLoginId);

            // Assert
            VerifyAll();
            result.Should().BeAssignableTo<RepositoryDeleteResult<UserAndInfo>.Deleted>();
        }

        private void VerifyAll()
        {
            _mockPrimaryRepository.VerifyAll();
            _mockRepositoryFactory.VerifyAll();
        }
    }
}