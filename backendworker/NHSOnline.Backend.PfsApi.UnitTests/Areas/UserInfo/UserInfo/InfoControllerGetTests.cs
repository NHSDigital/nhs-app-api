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
using NHSOnline.Backend.Auth;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UserInfo.Areas.UserInfo;
using NHSOnline.Backend.UserInfo.Areas.UserInfo.Models;
using NHSOnline.Backend.UserInfo.Repository;
using NHSOnline.Backend.PfsApi.Areas.UserInfo.UserInfo;
using NHSOnline.Backend.PfsApi.Areas.UserInfo.UserResearch;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.UserInfo.UserInfo
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

            var mockAccessTokenProvider = new Mock<IAccessTokenProvider>();
            mockAccessTokenProvider.SetupGet(x => x.AccessToken)
                .Returns(AccessTokenMock.Generate());

            _systemUnderTest = new InfoController(
                mockAccessTokenProvider.Object,
                _mockInfoService.Object,
                new Mock<IUserResearchService>().Object,
                new Mock<IMapper<UserProfile, InfoUserProfile>>().Object,
                new Mock<ILogger<InfoController>>().Object,
                new Mock<IAuditor>().Object,
                new Mock<IMetricLogger<AccessTokenMetricContext>>().Object);
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
            var result = await _systemUnderTest.GetV1(odsCode, null);

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
            var result = await _systemUnderTest.GetV1(null, nhsNumber);

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
            var result = await _systemUnderTest.GetV1(null, nhsNumber);

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
            var result = await _systemUnderTest.GetV1(odsCode, null);

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
            var result = await _systemUnderTest.GetV1(null, nhsNumber);

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
            var result = await _systemUnderTest.GetV1(odsCode, null);

            // Assert
            _mockInfoService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Get_WithNhsNumberAndOdsCode_ReturnsBadRequest()
        {
            // Act
            var result = await _systemUnderTest.GetV1("ods code", "nhs number");

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
            var result = await _systemUnderTest.GetV1(value, value);

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