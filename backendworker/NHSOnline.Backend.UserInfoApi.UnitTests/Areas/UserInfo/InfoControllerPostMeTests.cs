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

        [TestInitialize]
        public void TestInitialize()
        {
            var mockHttpContext =  HttpContextGetAccessTokenHelper.CreateMockHttpContext();

            _mockInfoService = new Mock<IInfoService>();
            _mockCitizenIdService = new Mock<ICitizenIdService>();

            _systemUnderTest = new InfoController(
                _mockInfoService.Object,
                _mockCitizenIdService.Object,
                new Mock<ILogger<InfoController>>().Object)
            {
                ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object }
            };
        }

        [TestMethod]
        public async Task Post_Success()
        {
            // Arrange
            var odsCode = "ODS Code";
            MockUserProfileWithOdsCode(odsCode);
            _mockInfoService.Setup(x => x.Send(It.IsAny<AccessToken>(), odsCode))
                .ReturnsAsync(new PostInfoResult.Created(new Info()));

            // Act
            var result = await _systemUnderTest.Post();
            
            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [TestMethod]
        public async Task Post_WhenSendInfoReturnsBadGateway_ReturnsBadGateway()
        {
            // Arrange
            var odsCode = "ODS Code";
            MockUserProfileWithOdsCode(odsCode);
            _mockInfoService.Setup(x => x.Send(It.IsAny<AccessToken>(), odsCode))
                .ReturnsAsync(new PostInfoResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Post();

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Post_WhenSendInfoReturnsInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            var odsCode = "ODS Code";
            MockUserProfileWithOdsCode(odsCode);
            _mockInfoService.Setup(x => x.Send(It.IsAny<AccessToken>(), odsCode))
                .ReturnsAsync(new PostInfoResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post();

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_SendInfoException_ReturnsInternalServerError()
        {
            // Arrange
            var odsCode = "ODS Code";
            MockUserProfileWithOdsCode(odsCode);
            _mockInfoService.Setup(x => x.Send(It.IsAny<AccessToken>(), odsCode))
                .Throws(new ArgumentException("test"));

            // Act
            var result = await _systemUnderTest.Post();

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Post_CitizenIdFailsToReturnUserProfile_ReturnsBadGateway()
        {
            // Arrange
            MockUserProfileWithOdsCode(null);

            // Act
            var result = await _systemUnderTest.Post();

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }

        private void MockUserProfileWithOdsCode(string odsCode)
        {
            var userInfo = new Auth.CitizenId.Models.UserInfo { GpIntegrationCredentials = { OdsCode = odsCode } };
            var userProfile = new UserProfile(userInfo, "Access token");

            _mockCitizenIdService
                .Setup(x => x.GetUserProfile(It.IsAny<string>()))
                .ReturnsAsync(new GetUserProfileResult
                {
                    StatusCode = HttpStatusCode.OK,
                    UserProfile = Option.Some(userProfile)
                });

        }
    }
}
