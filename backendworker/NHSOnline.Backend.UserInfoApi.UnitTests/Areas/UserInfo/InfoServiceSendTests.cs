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
using NHSOnline.Backend.UserInfoApi.Repository;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;
using UnitTestHelper;

namespace NHSOnline.Backend.UserInfoApi.UnitTests.Areas.UserInfo
{
    [TestClass]
    public class InfoServiceSendTests
    {
        private InfoService _systemUnderTest;
        private Mock<IInfoRepository> _mockInfoRepository;
        private string _nhsLoginId;
        private AccessToken _accessToken;
        private string _nhsNumber;
        private InfoUserProfile _userProfile;
        private string _odsCode;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockInfoRepository = new Mock<IInfoRepository>();
            _nhsLoginId = "NHS Login Id";
            _nhsNumber = "NHS Number";
            _odsCode = "ODS code";
            _userProfile = new InfoUserProfile { NhsNumber = _nhsNumber, OdsCode = _odsCode };
            var mockLogger = new Mock<ILogger<InfoService>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _nhsLoginId),
                new Claim("nhs_number", _nhsNumber)
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);
            _systemUnderTest = new InfoService(_mockInfoRepository.Object, mockLogger.Object);
        }

        [TestMethod]
        public async Task Send_SuccessCreated()
        {
            // Arrange
            UserAndInfo actualUserInfo = null;

            var userAndInfo = new UserAndInfo
            {
                NhsLoginId = _nhsLoginId,
                Info = new Info
                {
                    NhsNumber = _userProfile.NhsNumber,
                    OdsCode = _userProfile.OdsCode
                }
            };
            _mockInfoRepository.Setup(x => x.Create(It.IsAny<UserAndInfo>()))
                .Callback<UserAndInfo>(u => actualUserInfo = u)
                .ReturnsAsync(() => new RepositoryCreateResult<UserAndInfo>.Created(userAndInfo));

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
            _mockInfoRepository.Setup(x => x.Create(It.IsAny<UserAndInfo>())).Throws(new ArgumentException("Test"));

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
            _mockInfoRepository.Setup(x => x.Create(It.IsAny<UserAndInfo>()))
                .ReturnsAsync(new RepositoryCreateResult<UserAndInfo>.RepositoryError());

            // Act
            var result = await _systemUnderTest.Send(_accessToken, _userProfile);

            // Assert
            _mockInfoRepository.VerifyAll();
            result.Should().BeAssignableTo<PostInfoResult.BadGateway>();
        }
    }
}