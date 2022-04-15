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
    public sealed class InfoControllerGetV2Tests : IDisposable
    {
        private const string OdsCode = "OdsCode";
        private const string NhsLoginId1 = "NhsLoginId1";
        private const string NhsLoginId2 = "NhsLoginId2";
        private const string NhsNumber = "NhsNumber";
        private static readonly DateTime LastLogin = DateTime.UtcNow;

        private InfoController _systemUnderTest;

        private Mock<IInfoService> _mockInfoService;

        private readonly UserAndInfo[] _userInfoRecords =
        {
            new UserAndInfo
            {
                NhsLoginId = NhsLoginId1,
                Info = new Info { NhsNumber = NhsNumber, OdsCode = OdsCode },
                Timestamp = LastLogin
            },
            new UserAndInfo
            {
                NhsLoginId = NhsLoginId2,
                Info = new Info { NhsNumber = NhsNumber, OdsCode = OdsCode },
                Timestamp = LastLogin
            }
        };

        private readonly IEnumerable<InfoUserV2> _outputRecords = new[]
        {
            new InfoUserV2 { NhsNumber = NhsNumber, NhsLoginId = NhsLoginId1, OdsCode = OdsCode, LastLogin = LastLogin },
            new InfoUserV2 { NhsNumber = NhsNumber, NhsLoginId = NhsLoginId2, OdsCode = OdsCode, LastLogin = LastLogin }
        };

        [TestInitialize]
        public void TestInitialize()
        {
            _mockInfoService = new Mock<IInfoService>(MockBehavior.Strict);

            var mockAccessTokenProvider = new Mock<IAccessTokenProvider>();

            mockAccessTokenProvider
                .SetupGet(x => x.AccessToken)
                .Returns(AccessTokenMock.Generate());

            _systemUnderTest = new InfoController(
                mockAccessTokenProvider.Object,
                _mockInfoService.Object,
                new Mock<IUserResearchService>().Object,
                new Mock<IMapper<UserProfile, InfoUserProfile>>().Object,
                new Mock<ILogger<InfoController>>().Object,
                new Mock<IAuditor>().Object,
                new Mock<IMetricLogger<AccessTokenMetricContext>>().Object
            );
        }

        [TestMethod]
        public async Task GetV2_WithOdsCode_SuccessFoundMultiple()
        {
            // Arrange
            _mockInfoService
                .Setup(x => x.GetInfoByOdsCode(OdsCode))
                .ReturnsAsync(new GetInfoResult.Found(_userInfoRecords));

            // Act
            var result = await _systemUnderTest.GetV2(OdsCode, null);

            // Assert
            _mockInfoService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            var response = statusCodeResult.Value.Should().BeOfType<UserInfoResponseV2>().Subject;
            response.Users.Should().BeEquivalentTo(_outputRecords);
        }

        [TestMethod]
        public async Task GetV2_WithNhsNumber_SuccessFoundMultiple()
        {
            // Arrange
            _mockInfoService
                .Setup(x => x.GetInfoByNhsNumber(NhsNumber))
                .ReturnsAsync(new GetInfoResult.Found(_userInfoRecords));

            // Act
            var result = await _systemUnderTest.GetV2(null, NhsNumber);

            // Assert
            _mockInfoService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            var response = statusCodeResult.Value.Should().BeOfType<UserInfoResponseV2>().Subject;
            response.Users.Should().BeEquivalentTo(_outputRecords);
        }

        [TestMethod]
        public async Task GetV2_WithNhsNumber_NotFound_ReturnsEmptyList()
        {
            // Arrange
            _mockInfoService
                .Setup(x => x.GetInfoByNhsNumber(NhsNumber))
                .ReturnsAsync(new GetInfoResult.NotFound());

            // Act
            var result = await _systemUnderTest.GetV2(null, NhsNumber);

            // Assert
            _mockInfoService.VerifyAll();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(new List<string>());
        }


        [TestMethod]
        public async Task GetV2_WithOdsCode_NotFound_ReturnsEmptyList()
        {
            // Arrange
            _mockInfoService
                .Setup(x => x.GetInfoByOdsCode(OdsCode))
                .ReturnsAsync(new GetInfoResult.NotFound());

            // Act
            var result = await _systemUnderTest.GetV2(OdsCode, null);

            // Assert
            _mockInfoService.VerifyAll();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeEquivalentTo(new List<string>());
        }

        [TestMethod]
        public async Task GetV2_WithNhsNumber_BadGateway_ReturnsBadGateway()
        {
            // Arrange
            _mockInfoService
                .Setup(x => x.GetInfoByNhsNumber(NhsNumber))
                .ReturnsAsync(new GetInfoResult.BadGateway());

            // Act
            var result = await _systemUnderTest.GetV2(null, NhsNumber);

            // Assert
            _mockInfoService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }


        [TestMethod]
        public async Task GetV2_WithOdsCode_BadGateway_ReturnsBadGateway()
        {
            // Arrange
            _mockInfoService
                .Setup(x => x.GetInfoByOdsCode(OdsCode))
                .ReturnsAsync(new GetInfoResult.BadGateway());

            // Act
            var result = await _systemUnderTest.GetV2(OdsCode, null);

            // Assert
            _mockInfoService.VerifyAll();

            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task GetV2_WithNhsNumberAndOdsCode_ReturnsBadRequest()
        {
            // Act
            var result = await _systemUnderTest.GetV2(OdsCode, NhsNumber);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("     ")]
        public async Task GetV2_WithoutNhsNumberAndOdsCode_ReturnsBadRequest(string value)
        {
            // Act
            var result = await _systemUnderTest.GetV2(value, value);

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