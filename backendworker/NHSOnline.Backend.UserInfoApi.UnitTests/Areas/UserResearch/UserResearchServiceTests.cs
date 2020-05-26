using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;
using NHSOnline.Backend.UserInfoApi.Areas.UserResearch;
using NHSOnline.Backend.UserInfoApi.Clients;
using UnitTestHelper;

namespace NHSOnline.Backend.UserInfoApi.UnitTests.Areas.UserResearch
{
    [TestClass]
    public class UserResearchServiceTests
    {
        private UserResearchService _systemUnderTest;
        private Mock<IUserResearchClient> _mockUserResearchClient;
        private InfoUserProfile _userInfoProfile;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockUserResearchClient = new Mock<IUserResearchClient>();
            _userInfoProfile = new InfoUserProfile() { Email = "Email" };
            var mockLogger = new Mock<ILogger<InfoController>>();
            var accessTokenString = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "Nhslogin"),
                new Claim("nhs_number", "NhsNumber"),
            });

            _accessToken = AccessToken.Parse(mockLogger.Object, accessTokenString);

            _systemUnderTest = new UserResearchService(mockLogger.Object, _mockUserResearchClient.Object);
        }

        [TestMethod]
        public async Task Post_Success()
        {
            // Arrange
            var response = new UserResearchClientResponse(HttpStatusCode.Created);
            _mockUserResearchClient.Setup(x => x.Post(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.Post(_userInfoProfile, _accessToken);

            // Assert
            _mockUserResearchClient.VerifyAll();
            result.Should().BeAssignableTo<PostUserResearchResult.Success>();
        }

        [TestMethod]
        public async Task Post_Failure()
        {
            // Arrange
            var response = new UserResearchClientResponse(HttpStatusCode.InternalServerError);
            _mockUserResearchClient.Setup(x => x.Post(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var result = await _systemUnderTest.Post(_userInfoProfile, _accessToken);

            // Assert
            _mockUserResearchClient.VerifyAll();
            result.Should().BeAssignableTo<PostUserResearchResult.Failure>();
        }

        [TestMethod]
        public async Task Post_Failure_NoEmail()
        {
            // Act
            var result = await _systemUnderTest.Post(new InfoUserProfile() { Email = null }, _accessToken);

            // Assert
            _mockUserResearchClient.VerifyAll();
            result.Should().BeAssignableTo<PostUserResearchResult.EmailMissing>();
        }

        [TestMethod]
        public async Task Post_ThrowsException_InternalServerError()
        {
            // Arrange
            _mockUserResearchClient.Setup(x => x.Post(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _systemUnderTest.Post(_userInfoProfile, _accessToken);

            // Assert
            _mockUserResearchClient.VerifyAll();
            result.Should().BeAssignableTo<PostUserResearchResult.InternalServerError>();
        }
    }
}