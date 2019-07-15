using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.PfsApi.CitizenId.Models;
using NHSOnline.Backend.Support.Settings;
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
        private ConfigurationSettings _mockSettings;
        private CitizenIdSessionService _systemUnderTest;

        private string _authCode;
        private string _codeVerifier;
        private string _redirectUrl;
        private string _accessToken;
        private string _im1Token;
        private string _nhsNumber = "0123456789";
        private string _formattedNhsNumber = "012 345 6789";
        private string _odsCode;
        private string _familyName;

        
        private const string _dateFormat = "yyyy-MM-dd";
        private const string CookieDomain = "CookieDomain";
        private int PrescriptionsDefaultLastNumberMonthsToDisplay = 12;   
        private const int DefaultSessionExpiryMinutes  = 10;
        private const int DefaultHttpTimeoutSeconds = 6;
        private int MinimumAppAge = 16;
        private int MinimumLinkageAge = 16;
        private DateTimeOffset? CurrentTermsConditionsEffectiveDate = DateTimeOffset.Now;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockCitizenIdService = _fixture.Freeze<Mock<ICitizenIdService>>();
            _mockMinimumAgeValidator = _fixture.Freeze<Mock<IMinimumAgeValidator>>();
            _mockSettings = new ConfigurationSettings(CookieDomain, PrescriptionsDefaultLastNumberMonthsToDisplay, DefaultSessionExpiryMinutes, 
                DefaultHttpTimeoutSeconds, MinimumAppAge, MinimumLinkageAge, CurrentTermsConditionsEffectiveDate); 

            _authCode = _fixture.Create<string>();
            _codeVerifier = _fixture.Create<string>();
            _redirectUrl = _fixture.Create<string>();
            _accessToken = _fixture.Create<string>();
            _im1Token = _fixture.Create<string>();
            _odsCode = _fixture.Create<string>();
            _familyName = _fixture.Create<string>();

            _systemUnderTest = _fixture.Create<CitizenIdSessionService>();
        }

        [TestMethod]
        public async Task Create_HappyPath_ReturnsValidGetUserProfileResult()
        {
            _mockMinimumAgeValidator
                .Setup(x => x.IsValid(It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(true)
                .Verifiable();

            var dateTimeNow = DateTime.Now;
            
            var userProfile = new UserProfile()
            {
                AccessToken = _accessToken,
                DateOfBirth = dateTimeNow.ToString(_dateFormat, CultureInfo.InvariantCulture),
                Im1ConnectionToken = _im1Token,
                NhsNumber = _nhsNumber,
                OdsCode = _odsCode,
                FamilyName = _familyName
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
                DateOfBirth = dateTimeNow.Date,
                Im1ConnectionToken = _im1Token,
                NhsNumber = _formattedNhsNumber,
                OdsCode = _odsCode,
                Session = new CitizenIdUserSession
                {
                    AccessToken = _accessToken,
                    DateOfBirth = dateTimeNow.Date,
                    FamilyName = _familyName
                },
                StatusCode = 200
            };

            var result = await _systemUnderTest.Create(_authCode, _codeVerifier, _redirectUrl);
            
            result.Should().BeEquivalentTo(expectedResult);
            _mockMinimumAgeValidator.Verify();
            _mockCitizenIdService.Verify();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.GatewayTimeout)]
        public async Task Create_CitizenIdServiceReturnsNoUserProfile_ReturnsResultWithSameStatusCode(HttpStatusCode statusCode)
        {
            _mockCitizenIdService
                .Setup(x => x.GetUserProfile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new GetUserProfileResult
                {
                    StatusCode = statusCode,
                    UserProfile = Option.None<UserProfile>()
                });

            var expectedStatusCode = (int) statusCode;

            var result = await _systemUnderTest.Create(_authCode, _codeVerifier, _redirectUrl);

            result.StatusCode.Should().Be((expectedStatusCode));
            result.Session.Should().BeNull();
        }
        
        [TestMethod]
        public async Task Create_MinimumAgeValidationFails_ReturnsStatus465FailedAgeRequirement()
        {
            _mockMinimumAgeValidator
                .Setup(x => x.IsValid(It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(false)
                .Verifiable();

            var dateTimeNow = DateTime.Now;
            
            var userProfile = new UserProfile()
            {
                AccessToken = _accessToken,
                DateOfBirth = dateTimeNow.ToString(_dateFormat, CultureInfo.InvariantCulture),
                Im1ConnectionToken = _im1Token,
                NhsNumber = _nhsNumber,
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

            var result = await _systemUnderTest.Create(_authCode, _codeVerifier, _redirectUrl);
            
            result.StatusCode.Should().Be(expectedResult.StatusCode);
            _mockMinimumAgeValidator.Verify();
            _mockCitizenIdService.Verify();
        }
        
        [TestMethod]
        public async Task Create_UserProfileHasNoDateOfBirth_ReturnsStatus465FailedAgeRequirement()
        {
            var userProfile = new UserProfile()
            {
                AccessToken = _accessToken,
                DateOfBirth = null,
                Im1ConnectionToken = _im1Token,
                NhsNumber = _nhsNumber,
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

            var result = await _systemUnderTest.Create(_authCode, _codeVerifier, _redirectUrl);
            
            result.StatusCode.Should().Be(expectedResult.StatusCode);
            _mockCitizenIdService.Verify();
        }
        
        [TestMethod]
        public async Task Create_InvalidNhsNumber_ReturnsStatus464OdsCodeNotSupportedOrNoNhsNumber()
        {
            _mockMinimumAgeValidator
                .Setup(x => x.IsValid(It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(true)
                .Verifiable();

            var dateTimeNow = DateTime.Now;
            
            var userProfile = new UserProfile()
            {
                AccessToken = _accessToken,
                DateOfBirth = dateTimeNow.ToString(_dateFormat, CultureInfo.InvariantCulture),
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

            var result = await _systemUnderTest.Create(_authCode, _codeVerifier, _redirectUrl);
            
            result.StatusCode.Should().Be(expectedResult.StatusCode);
            _mockMinimumAgeValidator.Verify();
            _mockCitizenIdService.Verify();
        }
    }
}