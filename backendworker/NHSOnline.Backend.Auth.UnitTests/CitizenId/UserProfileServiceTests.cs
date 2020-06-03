using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.Auth.UnitTests.CitizenId
{
    [TestClass]
    public class UserProfileServiceTests
    {
        private Mock<IAccessTokenProvider> _mockAccessTokenProvider;
        private Mock<ICitizenIdService> _mockCitizenIdService;
        private UserProfileService _systemUnderTest;
        private AccessToken _accessToken;

        [TestInitialize]
        public void TestInitialize()
        {
            _accessToken = AccessTokenMock.Generate();
            _mockAccessTokenProvider = new Mock<IAccessTokenProvider>();
            _mockAccessTokenProvider.SetupGet(x => x.AccessToken)
                .Returns(_accessToken);

            _mockCitizenIdService = new Mock<ICitizenIdService>();

            _systemUnderTest = new UserProfileService(_mockAccessTokenProvider.Object,
                _mockCitizenIdService.Object);
        }

        [TestMethod]
        public void GetExistingUserProfileOrThrow_ThrowsError_WhenNoProfileSet()
        {
            //Act
            Action act = () => _systemUnderTest.GetExistingUserProfileOrThrow("context");

            //Assert
            _mockAccessTokenProvider.VerifyNoOtherCalls();
            _mockCitizenIdService.VerifyNoOtherCalls();
            act.Should().Throw<InvalidOperationException>().WithMessage("context: Required user profile but no profile has been set");
        }

        [TestMethod]
        public void GetExistingUserProfileOrThrow_ReturnsUserProfile_WhenProfileIsSet()
        {
            //Arrange
            var userProfile = new UserProfile(
                new UserInfo(),
                "AccessToken",
                "RefreshToken");

            _systemUnderTest.SetUserProfile(userProfile);

            //Act
            var result = _systemUnderTest.GetExistingUserProfileOrThrow("context");

            //Assert
            _mockAccessTokenProvider.VerifyNoOtherCalls();
            _mockCitizenIdService.VerifyNoOtherCalls();
            result.Should().Be(userProfile);
        }

        [TestMethod]
        public async Task GetUserProfile_ReturnsUserProfileOption_WhenUserProfileSucceeds()
        {
            //Arrange
            var userProfile =
                Option.Some(
                    new UserProfile(
                        new UserInfo(),
                        "AccessToken",
                        "RefreshToken"
                    )
                );

            var getUserProfileResult = new GetUserProfileResult
            {
                UserProfile = userProfile
            };

            _mockCitizenIdService
                .Setup(x => x.GetUserProfile(_accessToken.ToString()))
                .ReturnsAsync(getUserProfileResult);

            //Act
            var result = await _systemUnderTest.GetUserProfile();

            //Assert
            _mockAccessTokenProvider.VerifyAll();
            _mockCitizenIdService.VerifyAll();
            result.Should().Be(userProfile);
        }

        [TestMethod]
        public async Task GetUserProfile_ThrowsException_WhenCitizenIdServiceThrowsException()
        {
            //Arrange
            _mockCitizenIdService
                .Setup(x => x.GetUserProfile(_accessToken.ToString()))
                .ThrowsAsync(new InvalidCastException("Call Failed"));

            //Act
            Func<Task> act = async () => await _systemUnderTest.GetUserProfile();

            //Assert
            await act.Should().ThrowAsync<InvalidCastException>().WithMessage("Call Failed");
            _mockAccessTokenProvider.VerifyAll();
        }
    }
}