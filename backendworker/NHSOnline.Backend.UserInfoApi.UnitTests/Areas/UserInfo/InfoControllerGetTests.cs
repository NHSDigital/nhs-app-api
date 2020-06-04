using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;
using NHSOnline.Backend.UserInfoApi.Areas.UserResearch;
using NHSOnline.Backend.UserInfoApi.Repository;
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
                new Mock<IUserResearchService>().Object,
                new Mock<ICitizenIdService>().Object,
                new Mock<IMapper<UserProfile, InfoUserProfile>>().Object,
                new Mock<ILogger<InfoController>>().Object,
                new Mock<IAuditor>().Object)
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
            var nhsLoginId1 = "nhsLoginId1";
            var nhsLoginId2 = "nhsLoginId2";
            var userInfoRecords = new[]
            {
                new UserAndInfo { NhsLoginId = nhsLoginId1 }, new UserAndInfo { NhsLoginId = nhsLoginId2 }
            };
            _mockInfoService.Setup(x => x.GetInfoByOdsCode(odsCode))
                .ReturnsAsync(new GetInfoResult.Found(userInfoRecords));

            // Act
            var result = await _systemUnderTest.Get(odsCode, null);

            // Assert
            _mockInfoService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(new List<string> { nhsLoginId1, nhsLoginId2 });
        }

        [TestMethod]
        public async Task Get_WithNhsNumber_SuccessFoundMultiple()
        {
            // Arrange
            var nhsNumber = "NHS number";
            var nhsLoginId1 = "nhsLoginId1";
            var nhsLoginId2 = "nhsLoginId2";
            var userInfoRecords = new[]
            {
                new UserAndInfo { NhsLoginId = nhsLoginId1 }, new UserAndInfo { NhsLoginId = nhsLoginId2 }
            };
            _mockInfoService.Setup(x => x.GetInfoByNhsNumber(nhsNumber))
                .ReturnsAsync(new GetInfoResult.Found(userInfoRecords));

            // Act
            var result = await _systemUnderTest.Get(null, nhsNumber);

            // Assert
            _mockInfoService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(new List<string> { nhsLoginId1, nhsLoginId2 });
        }

        [TestMethod]
        public async Task Get_WithNhsNumber_NotFound_ReturnsEmptyList()
        {
            // Arrange
            var nhsNumber = "NHS number";
            _mockInfoService.Setup(x => x.GetInfoByNhsNumber(nhsNumber))
                .ReturnsAsync(new GetInfoResult.NotFound());

            // Act
            var result = await _systemUnderTest.Get(null, nhsNumber);

            // Assert
            _mockInfoService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(new List<string>());
        }


        [TestMethod]
        public async Task Get_WithOdsCode_NotFound_ReturnsEmptyList()
        {
            // Arrange
            var odsCode = "ods code";
            _mockInfoService.Setup(x => x.GetInfoByOdsCode(odsCode))
                .ReturnsAsync(new GetInfoResult.NotFound());

            // Act
            var result = await _systemUnderTest.Get(odsCode, null);

            // Assert
            _mockInfoService.VerifyAll();
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(new List<string>());
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