using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;
using NHSOnline.Backend.UserInfoApi.Areas.UserResearch;
using NHSOnline.Backend.UserInfoApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UserInfoApi.UnitTests.Areas.UserInfo
{
    [TestClass]
    public sealed class InfoControllerPostMeTests: IDisposable
    {
        private InfoController _systemUnderTest;
        private Mock<IInfoService> _mockInfoService;
        private Mock<IMapper<UserProfile, InfoUserProfile>> _mockInfoUserProfileMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockInfoService = new Mock<IInfoService>();
            _mockInfoUserProfileMapper = new Mock<IMapper<UserProfile, InfoUserProfile>>();

            var mockAccessTokenProvider = new Mock<IAccessTokenProvider>();
            mockAccessTokenProvider.SetupGet(x => x.AccessToken)
                .Returns(AccessTokenMock.Generate());

            _systemUnderTest = new InfoController(
                mockAccessTokenProvider.Object,
                _mockInfoService.Object,
                new Mock<IUserResearchService>().Object,
                _mockInfoUserProfileMapper.Object,
                new Mock<ILogger<InfoController>>().Object,
                new Mock<IAuditor>().Object,
                new Mock<IMetricLogger>().Object);
        }

        [TestMethod]
        public async Task Post_Success()
        {
            // Arrange
            var infoUserProfile = new InfoUserProfile
            {
                OdsCode = "ODS Code",
                NhsNumber = "NHS Number"
            };
            var userProfile  = MockUserProfileSetup(infoUserProfile);
            _mockInfoUserProfileMapper
                .Setup(x => x.Map(userProfile))
                .Returns(infoUserProfile);

            _mockInfoService.Setup(x => x.Send(It.IsAny<AccessToken>(), infoUserProfile))
                .ReturnsAsync(new PostInfoResult.Created(new UserAndInfo()));

            // Act
            var result = await _systemUnderTest.Post(userProfile);

            // Assert
            _mockInfoService.VerifyAll();
            _mockInfoUserProfileMapper.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [TestMethod]
        public async Task Post_WhenSendInfoReturnsBadGateway_ReturnsBadGateway()
        {
            // Arrange
            var infoUserProfile = new InfoUserProfile
            {
                OdsCode = "ODS Code",
                NhsNumber = "NHS Number"
            };
            var userProfile = MockUserProfileSetup(infoUserProfile);
            _mockInfoUserProfileMapper
                .Setup(x => x.Map(userProfile))
                .Returns(infoUserProfile);
            _mockInfoService.Setup(x => x.Send(It.IsAny<AccessToken>(), infoUserProfile))
                .ReturnsAsync(new PostInfoResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Post(userProfile);

            // Assert
            _mockInfoService.VerifyAll();
            _mockInfoUserProfileMapper.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Post_WhenSendInfoReturnsInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            var infoUserProfile = new InfoUserProfile
            {
                OdsCode = "ODS Code",
                NhsNumber = "NHS Number"
            };
            var userProfile = MockUserProfileSetup(infoUserProfile);
            _mockInfoUserProfileMapper
                .Setup(x => x.Map(userProfile))
                .Returns(infoUserProfile);
            _mockInfoService.Setup(x => x.Send(It.IsAny<AccessToken>(), infoUserProfile))
                .ReturnsAsync(new PostInfoResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post(userProfile);

            // Assert
            _mockInfoService.VerifyAll();
            _mockInfoUserProfileMapper.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_SendInfoException_ReturnsInternalServerError()
        {
            // Arrange
            var infoUserProfile = new InfoUserProfile
            {
                OdsCode = "ODS Code",
                NhsNumber = "NHS Number"
            };
            var userProfile = MockUserProfileSetup(infoUserProfile);
            _mockInfoUserProfileMapper
                .Setup(x => x.Map(userProfile))
                .Returns(infoUserProfile);
            _mockInfoService.Setup(x => x.Send(It.IsAny<AccessToken>(), infoUserProfile))
                .Throws(new ArgumentException(string.Empty));

            // Act
            var result = await _systemUnderTest.Post(userProfile);

            // Assert
            _mockInfoService.VerifyAll();
            _mockInfoUserProfileMapper.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }

        private UserProfile MockUserProfileSetup(InfoUserProfile userProfile)
        {
            var userInfo = new Auth.CitizenId.Models.UserInfo
            {
                GpIntegrationCredentials = { OdsCode = userProfile.OdsCode },
                NhsNumber = userProfile.NhsNumber,
            };
            return new UserProfile(userInfo, "Access token", "Refresh Token");
        }
    }
}
