using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;
using NHSOnline.Backend.UserInfoApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UserInfoApi.UnitTests.Areas.UserInfo
{
    [TestClass]
    public sealed class InfoControllerPostMeTests: IDisposable
    {
        private InfoController _systemUnderTest;
        private Mock<IInfoService> _mockInfoService;
        private Mock<ICitizenIdService> _mockCitizenIdService;
        private Mock<IMapper<UserProfile, InfoUserProfile>> _mockInfoUserProfileMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockInfoService = new Mock<IInfoService>();
            _mockCitizenIdService = new Mock<ICitizenIdService>();
            _mockInfoUserProfileMapper = new Mock<IMapper<UserProfile, InfoUserProfile>>();

            _systemUnderTest = new InfoController(
                _mockInfoService.Object,
                _mockCitizenIdService.Object,
                _mockInfoUserProfileMapper.Object,
                new Mock<ILogger<InfoController>>().Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = HttpContextGetAccessTokenHelper.CreateMockHttpContext().Object
                }
            };
        }

        [TestMethod]
        public async Task Post_Success()
        {
            // Arrange
            var userProfile = new InfoUserProfile
            {
                OdsCode = "ODS Code",
                NhsNumber = "NHS Number"
            };
            MockUserProfileSetup(userProfile);
            _mockInfoService.Setup(x => x.Send(It.IsAny<AccessToken>(), userProfile))
                .ReturnsAsync(new PostInfoResult.Created(new UserAndInfo()));

            // Act
            var result = await _systemUnderTest.Post();

            // Assert
            _mockInfoService.VerifyAll();
            _mockCitizenIdService.VerifyAll();
            _mockInfoUserProfileMapper.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [TestMethod]
        public async Task Post_WhenSendInfoReturnsBadGateway_ReturnsBadGateway()
        {
            // Arrange
            var userProfile = new InfoUserProfile
            {
                OdsCode = "ODS Code",
                NhsNumber = "NHS Number"
            };
            MockUserProfileSetup(userProfile);
            _mockInfoService.Setup(x => x.Send(It.IsAny<AccessToken>(), userProfile))
                .ReturnsAsync(new PostInfoResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Post();

            // Assert
            _mockInfoService.VerifyAll();
            _mockCitizenIdService.VerifyAll();
            _mockInfoUserProfileMapper.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Post_WhenSendInfoReturnsInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            var userProfile = new InfoUserProfile
            {
                OdsCode = "ODS Code",
                NhsNumber = "NHS Number"
            };
            MockUserProfileSetup(userProfile);
            _mockInfoService.Setup(x => x.Send(It.IsAny<AccessToken>(), userProfile))
                .ReturnsAsync(new PostInfoResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post();

            // Assert
            _mockInfoService.VerifyAll();
            _mockCitizenIdService.VerifyAll();
            _mockInfoUserProfileMapper.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_SendInfoException_ReturnsInternalServerError()
        {
            // Arrange
            var userProfile = new InfoUserProfile
            {
                OdsCode = "ODS Code",
                NhsNumber = "NHS Number"
            };
            MockUserProfileSetup(userProfile);
            _mockInfoService.Setup(x => x.Send(It.IsAny<AccessToken>(), userProfile))
                .Throws(new ArgumentException(string.Empty));

            // Act
            var result = await _systemUnderTest.Post();

            // Assert
            _mockInfoService.VerifyAll();
            _mockCitizenIdService.VerifyAll();
            _mockInfoUserProfileMapper.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_CitizenIdFailsToReturnUserProfile_ReturnsBadGateway()
        {
            // Arrange
            _mockCitizenIdService.Setup(x => x.GetUserProfile(It.IsAny<string>()))
                .ReturnsAsync(new GetUserProfileResult
                {
                    StatusCode = HttpStatusCode.BadGateway,
                    UserProfile = Option.None<UserProfile>()
                });

            // Act
            var result = await _systemUnderTest.Post();

            // Assert
            _mockCitizenIdService.VerifyAll();
            _mockInfoUserProfileMapper.VerifyNoOtherCalls();
            _mockInfoService.VerifyNoOtherCalls();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }

        private void MockUserProfileSetup(InfoUserProfile userProfile)
        {
            var userInfo = new Auth.CitizenId.Models.UserInfo
            {
                GpIntegrationCredentials = { OdsCode = userProfile.OdsCode },
                NhsNumber = userProfile.NhsNumber,
            };
            var cidUserProfile = new UserProfile(userInfo, "Access token", "Refresh Token");

            _mockCitizenIdService
                .Setup(x => x.GetUserProfile(It.IsAny<string>()))
                .ReturnsAsync(new GetUserProfileResult
                {
                    StatusCode = HttpStatusCode.OK,
                    UserProfile = Option.Some(cidUserProfile)
                });

            _mockInfoUserProfileMapper.Setup(x => x.Map(cidUserProfile))
                .Returns(userProfile);
        }
    }
}
