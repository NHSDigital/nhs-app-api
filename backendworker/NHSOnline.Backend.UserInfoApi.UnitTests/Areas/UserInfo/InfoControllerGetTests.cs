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
using UnitTestHelper;

namespace NHSOnline.Backend.UserInfoApi.UnitTests.Areas.UserInfo
{
    [TestClass]
    public sealed class InfoControllerGetTests : IDisposable
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
        public async Task Get_WithOdsCode_SuccessFoundMultiple()
        {
            // Arrange
            var odsCode = "ods code";
            var nhsLoginIds = new[] { "login id 1", "login id 2" };
            _mockInfoService.Setup(x => x.GetInfoByOdsCode(odsCode))
                .ReturnsAsync(new GetInfoResult.FoundMultiple(nhsLoginIds));

            // Act
            var result = await _systemUnderTest.Get(odsCode, null);

            // Assert
            _mockInfoService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(nhsLoginIds);
        }

        [TestMethod]
        public async Task Get_WithNhsNumber_SuccessFoundMultiple()
        {
            // Arrange
            var nhsNumber = "NHS number";
            var nhsLoginIds = new[] { "login id 1", "login id 2" };
            _mockInfoService.Setup(x => x.GetInfoByNhsNumber(nhsNumber))
                .ReturnsAsync(new GetInfoResult.FoundMultiple(nhsLoginIds));

            // Act
            var result = await _systemUnderTest.Get(null, nhsNumber);

            // Assert
            _mockInfoService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(nhsLoginIds);
        }

        [TestMethod]
        public async Task Get_WithNhsNumber_NotFound_ReturnsNotFound()
        {
            // Arrange
            var nhsNumber = "NHS number";
            _mockInfoService.Setup(x => x.GetInfoByNhsNumber(nhsNumber))
                .ReturnsAsync(new GetInfoResult.NotFound());

            // Act
            var result = await _systemUnderTest.Get(null, nhsNumber);

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }


        [TestMethod]
        public async Task Get_WithOdsCode_NotFound_ReturnsNotFound()
        {
            // Arrange
            var odsCode = "ods code";
            _mockInfoService.Setup(x => x.GetInfoByOdsCode(odsCode))
                .ReturnsAsync(new GetInfoResult.NotFound());

            // Act
            var result = await _systemUnderTest.Get(odsCode, null);

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Get_WithNhsNumber_BadGateway_ReturnsBadGateway()
        {
            // Arrange
            var nhsNumber = "NHS number";
            _mockInfoService.Setup(x => x.GetInfoByNhsNumber(nhsNumber))
                .ReturnsAsync(new GetInfoResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Get(null, nhsNumber);

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }


        [TestMethod]
        public async Task Get_WithOdsCode_BadGateway_ReturnsBadGateway()
        {
            // Arrange
            var odsCode = "ods code";
            _mockInfoService.Setup(x => x.GetInfoByOdsCode(odsCode))
                .ReturnsAsync(new GetInfoResult.BadGateway());

            // Act
            var result = await _systemUnderTest.Get(odsCode, null);

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Get_WithNhsNumberAndOdsCode_ReturnsBadRequest()
        {
            // Act
            var result = await _systemUnderTest.Get("ods code", "nhs number");

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("     ")]
        public async Task Get_WithoutNhsNumberAndOdsCode_ReturnsBadRequest(string value)
        {
            // Act
            var result = await _systemUnderTest.Get(value, value);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}