using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo;
using NHSOnline.Backend.UserInfoApi.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.UserInfoApi.UnitTests.Areas.UserInfo
{
    [TestClass]
    public class InfoControllerGetMeTests
    {
        private IFixture _fixture;
        private InfoController _systemUnderTest;
        private Mock<IInfoService> _mockInfoService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            var mockHttpContext =  HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);

            _mockInfoService = _fixture.Freeze<Mock<IInfoService>>();

            _systemUnderTest = _fixture.Create<InfoController>();
            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            };
        }

        [TestMethod]
        public async Task Get_SuccessFound()
        {
            // Arrange
            HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);
            var userInfo = _fixture.Create<UserAndInfo>();
            _mockInfoService.Setup(x => x.GetInfo(It.IsAny<AccessToken>()))
                .ReturnsAsync(new GetInfoResult.Found(userInfo));

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
            HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);
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
            HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);
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
            HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);
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
            HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);
            _mockInfoService.Setup(x => x.GetInfo(It.IsAny<AccessToken>()))
                .Throws(new ArgumentException("test"));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}