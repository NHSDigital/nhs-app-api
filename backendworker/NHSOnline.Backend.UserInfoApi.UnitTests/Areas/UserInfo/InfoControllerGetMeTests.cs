using System;
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
    public sealed class InfoControllerGetMeTests: IDisposable
    {
        private InfoController _systemUnderTest;
        private Mock<IInfoService> _mockInfoService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockInfoService = new Mock<IInfoService>();

            _systemUnderTest = new InfoController(
                _mockInfoService.Object,
                new Mock<ICitizenIdService>().Object,
                new Mock<IMapper<UserProfile, InfoUserProfile>>().Object,
                new Mock<ILogger<InfoController>>().Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = HttpContextGetAccessTokenHelper.CreateMockHttpContext().Object
                }
            };
        }

        [TestMethod]
        public async Task Get_SuccessFound()
        {
            // Arrange
            var userInfo = new UserAndInfo();
            _mockInfoService.Setup(x => x.GetInfo(It.IsAny<AccessToken>()))
                .ReturnsAsync(new GetInfoResult.Found(new []{userInfo}));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockInfoService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<UserAndInfo>()
                .Subject.Should().BeEquivalentTo(userInfo);
        }

        [TestMethod]
        public async Task Get_SuccessNotFound()
        {
            // Arrange
            _mockInfoService.Setup(x => x.GetInfo(It.IsAny<AccessToken>()))
                .ReturnsAsync(new GetInfoResult.NotFound());

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Get_WhenGetInfoReturnsBadGateway_ReturnsBadGateway()
        {
            // Arrange
            _mockInfoService.Setup(x => x.GetInfo(It.IsAny<AccessToken>()))
                .ReturnsAsync(new GetInfoResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Get_WhenGetInfoReturnsInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            _mockInfoService.Setup(x => x.GetInfo(It.IsAny<AccessToken>()))
                .ReturnsAsync(new GetInfoResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_WhenGetInfoException_ReturnsInternalServerError()
        {
            // Arrange
            _mockInfoService.Setup(x => x.GetInfo(It.IsAny<AccessToken>()))
                .Throws(new ArgumentException(string.Empty));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestCleanup]
        public void Dispose() => _systemUnderTest?.Dispose();
    }
}