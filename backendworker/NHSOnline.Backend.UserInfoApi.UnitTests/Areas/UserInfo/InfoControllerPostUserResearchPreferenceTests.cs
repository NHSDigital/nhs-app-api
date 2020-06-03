using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auditing.UnitTestsSupport;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Metrics;
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
        private Mock<IMapper<UserProfile, InfoUserProfile>> _mockUserProfileMapper;
        private Mock<IMetricLogger> _mockMetricLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockInfoService = new Mock<IInfoService>();
            _mockQualtricsService = new Mock<IUserResearchService>();
            _mockUserProfileMapper = new Mock<IMapper<UserProfile, InfoUserProfile>>();
                var mockAuditor = new Mock<IAuditor>();
                mockAuditor.Setup(x => x.Audit()).Returns(new AuditBuilderStub());
            _mockMetricLogger = new Mock<IMetricLogger>();

            var mockAccessTokenProvider = new Mock<IAccessTokenProvider>();
                mockAccessTokenProvider.SetupGet(x => x.AccessToken)
                    .Returns(AccessTokenMock.Generate());


                _systemUnderTest = new InfoController(
                    mockAccessTokenProvider.Object,
                    _mockInfoService.Object,
                    _mockQualtricsService.Object,
                    _mockUserProfileMapper.Object,
                    new Mock<ILogger<InfoController>>().Object,
                    mockAuditor.Object,
                    _mockMetricLogger.Object);
        }

        [TestMethod]
        public async Task Put_OptIn_Success()
        {
            // Arrange
            var infoUserProfile = new InfoUserProfile
            {
                OdsCode = "ODS Code",
                NhsNumber = "NHS Number"
            };
            var userProfile = MockUserProfileSetup(infoUserProfile);
            _mockUserProfileMapper
                .Setup(x => x.Map(userProfile))
                .Returns(infoUserProfile);

            _mockQualtricsService.Setup(x => x.Post(infoUserProfile, It.IsAny<AccessToken>()))
                .ReturnsAsync(new PostUserResearchResult.Success());

            // Act
            var result = await _systemUnderTest.PostUserResearchPreference(new UserResearchRequest
            {
                Preference = UserResearchPreference.OptIn
            }, userProfile);

            // Assert
            _mockInfoService.VerifyAll();
            _mockQualtricsService.VerifyAll();
            _mockUserProfileMapper.VerifyAll();
            _mockMetricLogger.Verify(x => x.UserResearchOptIn(), Times.Once);
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task Put_OptIn_QualtricsThrowsException_Failure()
        {
            // Arrange
            var infoUserProfile = new InfoUserProfile
            {
                OdsCode = "ODS Code",
                NhsNumber = "NHS Number"
            };
            var userProfile = MockUserProfileSetup(infoUserProfile);
            _mockUserProfileMapper
                .Setup(x => x.Map(userProfile))
                .Returns(infoUserProfile);

            _mockQualtricsService.Setup(x => x.Post(infoUserProfile, It.IsAny<AccessToken>()))
                .Throws(new ArgumentException("test"));

            // Act
            var result = await _systemUnderTest.PostUserResearchPreference(new UserResearchRequest
            {
                Preference = UserResearchPreference.OptIn
            }, userProfile);

            // Assert
            _mockInfoService.VerifyAll();
            _mockQualtricsService.VerifyAll();
            _mockUserProfileMapper.VerifyAll();
            _mockMetricLogger.VerifyNoOtherCalls();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>();
            statusCodeResult.Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Put_OptOut_Success()
        {
            // Arrange
            var userProfile = new UserProfile(
                new Auth.CitizenId.Models.UserInfo(),
                "AccessToken",
                "RefreshToken");

            // Act
            var result = await _systemUnderTest.PostUserResearchPreference(new UserResearchRequest
            {
                Preference = UserResearchPreference.OptOut
            },userProfile);

            // Assert
            _mockInfoService.VerifyAll();
            _mockQualtricsService.VerifyAll();
            _mockMetricLogger.Verify(x => x.UserResearchOptOut(), Times.Once);
            result.Should().BeAssignableTo<NoContentResult>();
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