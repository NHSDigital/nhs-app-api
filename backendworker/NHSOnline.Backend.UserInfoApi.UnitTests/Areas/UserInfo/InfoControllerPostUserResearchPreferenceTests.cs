using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auditing.UnitTestsSupport;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;
using NHSOnline.Backend.UserInfoApi.Areas.UserResearch;
using NHSOnline.Backend.UserInfoApi.Areas.UserResearch.Models;
using UnitTestHelper;

namespace NHSOnline.Backend.UserInfoApi.UnitTests.Areas.UserInfo
{
    [TestClass]
    public sealed class InfoControllerPostUserResearchPreferenceTests : IDisposable
    {
        private InfoController _systemUnderTest;
        private Mock<IInfoService> _mockInfoService;
        private Mock<IUserResearchService> _mockQualtricsService;
        private Mock<ICitizenIdService> _mockCitizenIdService;
        private Mock<IMapper<UserProfile, InfoUserProfile>> _mockUserProfileMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockInfoService = new Mock<IInfoService>();
            _mockCitizenIdService = new Mock<ICitizenIdService>();
            _mockQualtricsService = new Mock<IUserResearchService>();
            _mockUserProfileMapper = new Mock<IMapper<UserProfile, InfoUserProfile>>();
                var mockAuditor = new Mock<IAuditor>();
                mockAuditor.Setup(x => x.Audit()).Returns(new AuditBuilderStub());


            _systemUnderTest = new InfoController(
                _mockInfoService.Object,
                _mockQualtricsService.Object,
                _mockCitizenIdService.Object,
                _mockUserProfileMapper.Object,
                new Mock<ILogger<InfoController>>().Object,
                mockAuditor.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = HttpContextGetAccessTokenHelper.CreateMockHttpContext().Object
                }
            };
        }

        [TestMethod]
        public async Task Put_OptIn_Success()
        {
            // Arrange
            MockUserProfile();

            _mockQualtricsService.Setup(x => x.Post(It.IsAny<InfoUserProfile>(), It.IsAny<AccessToken>()))
                .ReturnsAsync(new PostUserResearchResult.Success());

            // Act
            var result = await _systemUnderTest.PostUserResearchPreference(new UserResearchRequest
            {
                Preference = UserResearchPreference.OptIn
            });

            // Assert
            _mockInfoService.VerifyAll();
            _mockQualtricsService.VerifyAll();
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task Put_OptIn_CallToCitizenIdException_Failure()
        {
            // Arrange
            _mockCitizenIdService
                .Setup(x => x.GetUserProfile(It.IsAny<string>()))
                .Throws(new ArgumentException("Test"));

            // Act
            var result = await _systemUnderTest.PostUserResearchPreference(new UserResearchRequest
            {
                Preference = UserResearchPreference.OptIn
            });

            // Assert
            _mockInfoService.VerifyAll();
            _mockQualtricsService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Put_OptIn_QualtricsThrowsException_Failure()
        {
            // Arrange
            MockUserProfile();

            _mockQualtricsService.Setup(x => x.Post(It.IsAny<InfoUserProfile>(), It.IsAny<AccessToken>()))
                .Throws(new ArgumentException("test"));

            // Act
            var result = await _systemUnderTest.PostUserResearchPreference(new UserResearchRequest
            {
                Preference = UserResearchPreference.OptIn
            });

            // Assert
            _mockInfoService.VerifyAll();
            _mockQualtricsService.VerifyAll();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Put_OptOut_Success()
        {
            // Arrange

            // Act
            var result = await _systemUnderTest.PostUserResearchPreference(new UserResearchRequest
            {
                Preference = UserResearchPreference.OptOut
            });

            // Assert
            _mockInfoService.VerifyAll();
            _mockQualtricsService.VerifyAll();
            result.Should().BeAssignableTo<NoContentResult>();
        }

        private void MockUserProfile()
        {
            var userInfo = new Auth.CitizenId.Models.UserInfo { NhsNumber = "NhsNumber", Email = "mockEmail" };
            var userProfile = new UserProfile(userInfo, "accessToken", "refreshToken");

            _mockCitizenIdService
                .Setup(x => x.GetUserProfile(It.IsAny<string>()))
                .ReturnsAsync(new GetUserProfileResult
                {
                    StatusCode = HttpStatusCode.OK,
                    UserProfile = Option.Some(userProfile)
                });

            _mockUserProfileMapper.Setup(x => x.Map(userProfile)).Returns(
                new InfoUserProfile()
                {
                    NhsNumber = userInfo.NhsNumber,
                    Email = userInfo.Email,
                    OdsCode = "OdsCode"
                });
        }

        [TestCleanup]
        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}