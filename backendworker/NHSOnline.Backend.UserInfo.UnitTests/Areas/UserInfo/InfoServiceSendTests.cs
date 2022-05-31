using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.UserInfo.Areas.UserInfo;
using NHSOnline.Backend.UserInfo.Areas.UserInfo.Models;
using NHSOnline.Backend.UserInfo.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UserInfo.UnitTests.Areas.UserInfo
{
    [TestClass]
    public class InfoServiceSendTests
    {
        private InfoService _systemUnderTest;
        private Mock<IInfoRepository> _mockInfoRepository;
        private TestUserInfoConfiguration _userInfoConfiguration;
        private string _nhsLoginId;
        private AccessToken _accessToken;
        private string _nhsNumber;
        private InfoUserProfile _userProfile;
        private string _odsCode;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockInfoRepository = new Mock<IInfoRepository>(MockBehavior.Strict);
            _nhsLoginId = "NHS Login Id";
            _nhsNumber = "NHS Number";
            _odsCode = "ODS code";
            _userProfile = new InfoUserProfile { NhsNumber = _nhsNumber, OdsCode = _odsCode };

            var mockLogger = new Mock<ILogger<InfoService>>();
            _userInfoConfiguration = new TestUserInfoConfiguration
            {
                SaveToSecondaryContainers = true,
                ReadFromSecondaryContainers = true,
            };

            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _nhsLoginId),
                new Claim("nhs_number", _nhsNumber)
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);
            _systemUnderTest = new InfoService(_mockInfoRepository.Object, _userInfoConfiguration, mockLogger.Object);
        }

        [TestMethod]
        public async Task Send_WithSameUserProfile_SuccessCreated()
        {
            // Arrange
            UserAndInfo actualUserInfo = null;

            var currentUserInfo = BuildUserInfo(_userProfile);

            var lastSavedUserInfo = BuildUserInfo(_userProfile);

            _mockInfoRepository
                .Setup(x => x.FindByNhsLoginId(_nhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(new [] { lastSavedUserInfo }));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateOdsCodeRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateNhsNumberRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdatePrimary(It.IsAny<UserAndInfo>()))
                .Callback<UserAndInfo>(u => actualUserInfo = u)
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            // Act
            var result = await _systemUnderTest.Send(_accessToken, _userProfile);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.Created>();
            actualUserInfo.NhsLoginId.Should().BeEquivalentTo(_nhsLoginId);
            actualUserInfo.Info.NhsNumber.Should().BeEquivalentTo(_nhsNumber);
            actualUserInfo.Info.OdsCode.Should().Be(_odsCode);
        }

        [TestMethod]
        public async Task Send_WithSameUserProfileWhenSaveToSecondaryContainersFalse_SuccessCreated()
        {
            // Arrange
            UserAndInfo actualUserInfo = null;

            _userInfoConfiguration.SaveToSecondaryContainers = false;

            var currentUserInfo = BuildUserInfo(_userProfile);

            var lastSavedUserInfo = BuildUserInfo(_userProfile);

            _mockInfoRepository
                .Setup(x => x.FindByNhsLoginId(_nhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(new [] { lastSavedUserInfo }));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdatePrimary(It.IsAny<UserAndInfo>()))
                .Callback<UserAndInfo>(u => actualUserInfo = u)
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            // Act
            var result = await _systemUnderTest.Send(_accessToken, _userProfile);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.Created>();
            actualUserInfo.NhsLoginId.Should().BeEquivalentTo(_nhsLoginId);
            actualUserInfo.Info.NhsNumber.Should().BeEquivalentTo(_nhsNumber);
            actualUserInfo.Info.OdsCode.Should().Be(_odsCode);
        }

        [TestMethod]
        public async Task Send_WithNewUserProfile_SuccessCreated()
        {
            // Arrange
            UserAndInfo actualUserInfo = null;

            var currentUserInfo = BuildUserInfo(_userProfile);

            _mockInfoRepository
                .Setup(x => x.FindByNhsLoginId(_nhsLoginId))
                .ReturnsAsync(() => new RepositoryFindResult<UserAndInfo>.NotFound());

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateOdsCodeRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateNhsNumberRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdatePrimary(It.IsAny<UserAndInfo>()))
                .Callback<UserAndInfo>(u => actualUserInfo = u)
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            // Act
            var result = await _systemUnderTest.Send(_accessToken, _userProfile);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.Created>();
            actualUserInfo.NhsLoginId.Should().BeEquivalentTo(_nhsLoginId);
            actualUserInfo.Info.NhsNumber.Should().BeEquivalentTo(_nhsNumber);
            actualUserInfo.Info.OdsCode.Should().Be(_odsCode);
        }

        [TestMethod]
        public async Task Send_RepositoryThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var currentUserInfo = BuildUserInfo(_userProfile);

            var lastSavedUserInfo = BuildUserInfo(_userProfile);

            _mockInfoRepository
                .Setup(x => x.FindByNhsLoginId(_nhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(new [] { lastSavedUserInfo }));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateNhsNumberRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateOdsCodeRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdatePrimary(It.IsAny<UserAndInfo>()))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.Send(_accessToken, _userProfile);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Send_RepositoryReturnsError_ReturnsBadGateway()
        {
            // Arrange
            var currentUserInfo = BuildUserInfo(_userProfile);

            var lastSavedUserInfo = BuildUserInfo(_userProfile);

            _mockInfoRepository
                .Setup(x => x.FindByNhsLoginId(_nhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(new [] { lastSavedUserInfo }));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateNhsNumberRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateOdsCodeRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdatePrimary(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(new RepositoryCreateResult<UserAndInfo>.RepositoryError());

            // Act
            var result = await _systemUnderTest.Send(_accessToken, _userProfile);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.BadGateway>();
        }

        [TestMethod]
        public async Task Send_WhenNhsNumberChanged_RecordDeletedAndInserted()
        {
            // Arrange
            UserAndInfo actualUserInfo = null;

            var currentUserProfile = new InfoUserProfile { NhsNumber = "Nhs Number One", OdsCode = _odsCode };

            var currentUserInfo = BuildUserInfo(currentUserProfile);

            var lastSavedUserInfo = BuildUserInfo(_userProfile);

            _mockInfoRepository
                .Setup(x => x.FindByNhsLoginId(_nhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(new [] {lastSavedUserInfo}));

            _mockInfoRepository
                .Setup(x => x.DeleteNhsNumberRecord(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => new RepositoryDeleteResult<UserAndInfo>.Deleted());

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateNhsNumberRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateOdsCodeRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdatePrimary(It.IsAny<UserAndInfo>()))
                .Callback<UserAndInfo>(u => actualUserInfo = u)
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            // Act
            var result = await _systemUnderTest.Send(_accessToken, currentUserProfile);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.Created>();
            actualUserInfo.NhsLoginId.Should().BeEquivalentTo(_nhsLoginId);
            actualUserInfo.Info.NhsNumber.Should().BeEquivalentTo("Nhs Number One");
            actualUserInfo.Info.OdsCode.Should().Be(_odsCode);
        }

        [TestMethod]
        [DataRow(null, DisplayName = "Null Nhs Number")]
        [DataRow("", DisplayName = "Empty Nhs Number")]
        public async Task Send_WithInvalidNhsNumber_RecordDeleted(string nhsNumber)
        {
            // Arrange
            UserAndInfo actualUserInfo = null;

            var currentUserProfile = new InfoUserProfile { NhsNumber = nhsNumber, OdsCode = _odsCode };

            var currentUserInfo = BuildUserInfo(currentUserProfile);

            var lastSavedUserInfo = BuildUserInfo(_userProfile);

            _mockInfoRepository
                .Setup(x => x.FindByNhsLoginId(_nhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(new [] {lastSavedUserInfo}));

            _mockInfoRepository
                .Setup(x => x.DeleteNhsNumberRecord(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => new RepositoryDeleteResult<UserAndInfo>.Deleted());

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateOdsCodeRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdatePrimary(It.IsAny<UserAndInfo>()))
                .Callback<UserAndInfo>(u => actualUserInfo = u)
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            // Act
            var result = await _systemUnderTest.Send(_accessToken, currentUserProfile);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.Created>();
            actualUserInfo.NhsLoginId.Should().BeEquivalentTo(_nhsLoginId);
            actualUserInfo.Info.NhsNumber.Should().Be(nhsNumber);
            actualUserInfo.Info.OdsCode.Should().Be(_odsCode);
        }

        [TestMethod]
        public async Task Send_WhenOdsCodeChanged_RecordDeletedAndInserted()
        {
            // Arrange
            UserAndInfo actualUserInfo = null;

            var currentUserProfile = new InfoUserProfile { NhsNumber = _nhsNumber, OdsCode = "Ods Code One" };

            var currentUserInfo = BuildUserInfo(currentUserProfile);

            var lastSavedUserInfo = BuildUserInfo(_userProfile);

            _mockInfoRepository
                .Setup(x => x.FindByNhsLoginId(_nhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(new [] {lastSavedUserInfo}));

            _mockInfoRepository
                .Setup(x => x.DeleteOdsCodeRecord(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => new RepositoryDeleteResult<UserAndInfo>.Deleted());

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateNhsNumberRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateOdsCodeRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdatePrimary(It.IsAny<UserAndInfo>()))
                .Callback<UserAndInfo>(u => actualUserInfo = u)
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            // Act
            var result = await _systemUnderTest.Send(_accessToken, currentUserProfile);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.Created>();
            actualUserInfo.NhsLoginId.Should().BeEquivalentTo(_nhsLoginId);
            actualUserInfo.Info.NhsNumber.Should().BeEquivalentTo(_nhsNumber);
            actualUserInfo.Info.OdsCode.Should().Be("Ods Code One");
        }

        [TestMethod]
        [DataRow(null, DisplayName = "Null Ods Code")]
        [DataRow("", DisplayName = "Empty Ods Code")]
        public async Task Send_WithNullOdsCode_RecordDeleted(string odsCode)
        {
            // Arrange
            UserAndInfo actualUserInfo = null;

            var currentUserProfile = new InfoUserProfile { NhsNumber = _nhsNumber, OdsCode = odsCode };

            var currentUserInfo = BuildUserInfo(currentUserProfile);

            var lastSavedUserInfo = BuildUserInfo(_userProfile);

            _mockInfoRepository
                .Setup(x => x.FindByNhsLoginId(_nhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(new [] {lastSavedUserInfo}));

            _mockInfoRepository
                .Setup(x => x.DeleteOdsCodeRecord(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => new RepositoryDeleteResult<UserAndInfo>.Deleted());

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateNhsNumberRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdatePrimary(It.IsAny<UserAndInfo>()))
                .Callback<UserAndInfo>(u => actualUserInfo = u)
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            // Act
            var result = await _systemUnderTest.Send(_accessToken, currentUserProfile);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.Created>();
            actualUserInfo.NhsLoginId.Should().BeEquivalentTo(_nhsLoginId);
            actualUserInfo.Info.OdsCode.Should().Be(odsCode);
            actualUserInfo.Info.NhsNumber.Should().Be(_nhsNumber);
        }

        [TestMethod]
        public async Task Send_WhenOdsCodeChangedAndNhsNumberChanged_RecordDeletedAndInserted()
        {
            // Arrange
            UserAndInfo actualUserInfo = null;

            var currentUserProfile = new InfoUserProfile { NhsNumber = "Nhs Number One", OdsCode = "Ods Code One" };

            var currentUserInfo = BuildUserInfo(currentUserProfile);

            var lastSavedUserInfo = BuildUserInfo(_userProfile);

            _mockInfoRepository
                .Setup(x => x.FindByNhsLoginId(_nhsLoginId))
                .ReturnsAsync(new RepositoryFindResult<UserAndInfo>.Found(new [] {lastSavedUserInfo}));

            _mockInfoRepository
                .Setup(x => x.DeleteNhsNumberRecord(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => new RepositoryDeleteResult<UserAndInfo>.Deleted());

            _mockInfoRepository
                .Setup(x => x.DeleteOdsCodeRecord(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => new RepositoryDeleteResult<UserAndInfo>.Deleted());

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateNhsNumberRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdateOdsCodeRecord(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            _mockInfoRepository
                .Setup(x => x.CreateOrUpdatePrimary(It.IsAny<UserAndInfo>()))
                .Callback<UserAndInfo>(u => actualUserInfo = u)
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(currentUserInfo));

            // Act
            var result = await _systemUnderTest.Send(_accessToken, currentUserProfile);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.Created>();
            actualUserInfo.NhsLoginId.Should().BeEquivalentTo(_nhsLoginId);
            actualUserInfo.Info.NhsNumber.Should().BeEquivalentTo("Nhs Number One");
            actualUserInfo.Info.OdsCode.Should().Be("Ods Code One");
        }

        private UserAndInfo BuildUserInfo(InfoUserProfile userProfile) => new UserAndInfo
        {
            NhsLoginId = _nhsLoginId,
            Info = new Info
            {
                NhsNumber = userProfile.NhsNumber,
                OdsCode = userProfile.OdsCode
            }
        };
    }
}