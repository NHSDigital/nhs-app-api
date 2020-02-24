using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.PfsApi.UnitTests.CitizenId
{
    [TestClass]
    public class CitizenIdSessionServiceTests
    {
        private IFixture _fixture;
        private Mock<ICitizenIdService> _mockCitizenIdService;
        private Mock<IMinimumAgeValidator> _mockMinimumAgeValidator;
        private CitizenIdSessionService _systemUnderTest;

        private string _authCode;
        private string _codeVerifier;
        private string _redirectUrl;
        private string _accessToken;
        private string _im1Token;
        private const string NhsNumber = "0123456789";
        private const string FormattedNhsNumber = "012 345 6789";
        private string _odsCode;
        private string _familyName;
        private string _idTokenJti;
        private const string DateFormat = "yyyy-MM-dd";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockCitizenIdService = _fixture.Freeze<Mock<ICitizenIdService>>();
            _mockMinimumAgeValidator = _fixture.Freeze<Mock<IMinimumAgeValidator>>();

            _authCode = _fixture.Create<string>();
            _codeVerifier = _fixture.Create<string>();
            _redirectUrl = _fixture.Create<string>();
            _accessToken = _fixture.Create<string>();
            _im1Token = _fixture.Create<string>();
            _odsCode = _fixture.Create<string>();
            _familyName = _fixture.Create<string>();
            _idTokenJti = _fixture.Create<string>();

            _systemUnderTest = _fixture.Create<CitizenIdSessionService>();
        }

        [TestMethod]
        public async Task Create_HappyPath_ReturnsValidGetUserProfileResult()
        {
            // Arrange
            _mockMinimumAgeValidator
                .Setup(x => x.IsValid(It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(true)
                .Verifiable();

            var dateTimeNow = DateTime.Now;

            var userProfile = new UserProfile
            {
                AccessToken = _accessToken,
                DateOfBirth = dateTimeNow.ToString(DateFormat, CultureInfo.InvariantCulture),
                Im1ConnectionToken = _im1Token,
                NhsNumber = NhsNumber,
                OdsCode = _odsCode,
                FamilyName = _familyName
            };

            _mockCitizenIdService
                .Setup(x => x.GetUserProfile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new GetUserProfileResult
                {
                    StatusCode = HttpStatusCode.OK,
                    UserProfile = Option.Some(userProfile),
                    IdTokenJti = _idTokenJti
                })
                .Verifiable();

            var expectedResult = new CitizenIdSessionResult
            {
                DateOfBirth = dateTimeNow.Date,
                Im1ConnectionToken = _im1Token,
                NhsNumber = FormattedNhsNumber,
                OdsCode = _odsCode,
                Session = new CitizenIdUserSession
                {
                    AccessToken = _accessToken,
                    DateOfBirth = dateTimeNow.Date,
                    FamilyName = _familyName,
                    IdTokenJti = _idTokenJti
                },
                StatusCode = 200
            };

            // Act
            var result = await _systemUnderTest.Create(_authCode, _codeVerifier, _redirectUrl);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockMinimumAgeValidator.Verify();
            _mockCitizenIdService.Verify();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.GatewayTimeout)]
        public async Task Create_CitizenIdServiceReturnsNoUserProfile_ReturnsResultWithSameStatusCode(HttpStatusCode statusCode)
        {
            // Arrange
            _mockCitizenIdService
                .Setup(x => x.GetUserProfile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new GetUserProfileResult
                {
                    StatusCode = statusCode,
                    UserProfile = Option.None<UserProfile>()
                });

            // Act
            var result = await _systemUnderTest.Create(_authCode, _codeVerifier, _redirectUrl);

            // Assert
            using (new AssertionScope())
            {
                result.StatusCode.Should().Be((int) statusCode);
                result.Session.Should().BeNull();
            }
        }

        [TestMethod]
        public async Task Create_MinimumAgeValidationFails_ReturnsStatus465FailedAgeRequirement()
        {
            // Arrange
            _mockMinimumAgeValidator
                .Setup(x => x.IsValid(It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(false)
                .Verifiable();

            var dateTimeNow = DateTime.Now;

            var userProfile = new UserProfile
            {
                AccessToken = _accessToken,
                DateOfBirth = dateTimeNow.ToString(DateFormat, CultureInfo.InvariantCulture),
                Im1ConnectionToken = _im1Token,
                NhsNumber = NhsNumber,
                OdsCode = _odsCode
            };

            _mockCitizenIdService
                .Setup(x => x.GetUserProfile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new GetUserProfileResult
                {
                    StatusCode = HttpStatusCode.OK,
                    UserProfile = Option.Some(userProfile)
                })
                .Verifiable();

            var expectedResult = new CitizenIdSessionResult
            {
                StatusCode = Constants.CustomHttpStatusCodes.Status465FailedAgeRequirement
            };

            // Act
            var result = await _systemUnderTest.Create(_authCode, _codeVerifier, _redirectUrl);

            // Assert
            result.StatusCode.Should().Be(expectedResult.StatusCode);
            _mockMinimumAgeValidator.Verify();
            _mockCitizenIdService.Verify();
        }

        [TestMethod]
        public async Task Create_UserProfileHasNoDateOfBirth_ReturnsStatus465FailedAgeRequirement()
        {
            // Arrange
            var userProfile = new UserProfile
            {
                AccessToken = _accessToken,
                DateOfBirth = null,
                Im1ConnectionToken = _im1Token,
                NhsNumber = NhsNumber,
                OdsCode = _odsCode
            };

            _mockCitizenIdService
                .Setup(x => x.GetUserProfile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new GetUserProfileResult
                {
                    StatusCode = HttpStatusCode.OK,
                    UserProfile = Option.Some(userProfile)
                })
                .Verifiable();

            var expectedResult = new CitizenIdSessionResult
            {
                StatusCode = Constants.CustomHttpStatusCodes.Status465FailedAgeRequirement
            };

            // Act
            var result = await _systemUnderTest.Create(_authCode, _codeVerifier, _redirectUrl);

            // Assert
            result.StatusCode.Should().Be(expectedResult.StatusCode);
            _mockCitizenIdService.Verify();
        }

        [TestMethod]
        public async Task Create_InvalidNhsNumber_ReturnsStatus464OdsCodeNotSupportedOrNoNhsNumber()
        {
            // Arrange
            _mockMinimumAgeValidator
                .Setup(x => x.IsValid(It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(true)
                .Verifiable();

            var dateTimeNow = DateTime.Now;

            var userProfile = new UserProfile
            {
                AccessToken = _accessToken,
                DateOfBirth = dateTimeNow.ToString(DateFormat, CultureInfo.InvariantCulture),
                Im1ConnectionToken = _im1Token,
                NhsNumber = null,
                OdsCode = _odsCode
            };

            _mockCitizenIdService
                .Setup(x => x.GetUserProfile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new GetUserProfileResult
                {
                    StatusCode = HttpStatusCode.OK,
                    UserProfile = Option.Some(userProfile)
                })
                .Verifiable();

            var expectedResult = new CitizenIdSessionResult
            {
                StatusCode = Constants.CustomHttpStatusCodes.Status464OdsCodeNotSupportedOrNoNhsNumber
            };

            // Act
            var result = await _systemUnderTest.Create(_authCode, _codeVerifier, _redirectUrl);

            // Assert
            result.StatusCode.Should().Be(expectedResult.StatusCode);
            _mockMinimumAgeValidator.Verify();
            _mockCitizenIdService.Verify();
        }
    }
}