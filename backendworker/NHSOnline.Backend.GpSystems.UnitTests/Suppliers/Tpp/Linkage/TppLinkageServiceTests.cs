using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Temporal;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Linkage
{
    [TestClass]
    public class TppLinkageServiceTests
    {
        private IFixture _fixture;
        private Mock<ITppClientRequest<AddNhsUserRequest, AddNhsUserResponse>> _nhsUser;
        private Mock<IIm1CacheKeyGenerator> _mockIm1CacheKeyGenerator;
        private Mock<IIm1CacheService> _mockIm1CacheService;
        private ConfigurationSettings _settings;
        private Mock<ITppLinkageMapper> _mockLinkageMapper;
        private TppLinkageService _systemUnderTest;
        private Mock<IMinimumAgeValidator> _mockMinimumAgeValidator;
        private DateTime _dateOfBirth;
        private const string CookieDomain = "CookieDomain";
        private const int PrescriptionsDefaultLastNumberMonthsToDisplay = 12;
        private const int DefaultSessionExpiryMinutes  = 10;
        private const int DefaultHttpTimeoutSeconds = 6;
        private const int MinimumAppAge = 16;
        private const int MinimumLinkageAge = 16;

        private readonly DateTimeOffset? CurrentTermsConditionsEffectiveDate = DateTimeOffset.Now;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _nhsUser = _fixture.Freeze<Mock<ITppClientRequest<AddNhsUserRequest, AddNhsUserResponse>>>();
            _mockIm1CacheKeyGenerator = _fixture.Freeze<Mock<IIm1CacheKeyGenerator>>();
            _mockIm1CacheService = _fixture.Freeze<Mock<IIm1CacheService>>();
            _mockLinkageMapper = _fixture.Freeze<Mock<ITppLinkageMapper>>();

            _settings = new ConfigurationSettings(CookieDomain, PrescriptionsDefaultLastNumberMonthsToDisplay, DefaultHttpTimeoutSeconds, 
            DefaultSessionExpiryMinutes, MinimumAppAge, MinimumLinkageAge, CurrentTermsConditionsEffectiveDate);

            _fixture.Inject(_settings);

            _mockMinimumAgeValidator = _fixture.Freeze<Mock<IMinimumAgeValidator>>();
            _mockMinimumAgeValidator.Setup(x => x.IsValid(It.IsAny<DateTime>(), It.IsAny<int>())).Returns(true);
            _dateOfBirth = DateTime.Now.AddYears(-16);  //Doesn't matter what age, as validator is mocked

            _systemUnderTest = _fixture.Create<TppLinkageService>();
        }

        [TestMethod]
        public async Task GetLinkageKey_ReturnsNotFound_ForValidRequest()
        {
            // Arrange
            var request = _fixture.Create<GetLinkageRequest>();

            // Act
            var response = await _systemUnderTest.GetLinkageKey(request);

            // Assert
            response.Should().BeAssignableTo<LinkageResult.NotFound>();
        }
        
        [TestMethod]
        public async Task GetLinkageKey_ReturnsNotFound_ForNullRequest()
        {
            // Act
            var response = await _systemUnderTest.GetLinkageKey(null);

            // Assert
            response.Should().BeAssignableTo<LinkageResult.NotFound>();
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsSuccessfulResponse_ForValidRequest()
        {
            // Arrange
            var request = _fixture.Create<CreateLinkageRequest>();
            var nhsUserResponse = _fixture.Create<AddNhsUserResponse>();

            var linkageResponse = new LinkageResponse()
            {
                AccountId = nhsUserResponse.AccountId,
                OdsCode = request.OdsCode,
                LinkageKey = _fixture.Create<string>()
            };

            var userResponse = new TppApiObjectResponse<AddNhsUserResponse>(HttpStatusCode.OK)
                { Body = nhsUserResponse };

            _nhsUser
                .Setup(x => x.Post(It.IsAny<AddNhsUserRequest>()))
                .ReturnsAsync(userResponse);

            const string key = "CACHEKEY";
            _mockIm1CacheKeyGenerator
                .Setup(x => x.GenerateCacheKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(key)
                .Verifiable();

            _mockIm1CacheService
                .Setup(x => x.SaveIm1ConnectionToken(key, It.IsAny<TppConnectionToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _mockLinkageMapper
                .Setup(x => x.Map(It.IsAny<AddNhsUserRequest>(), It.IsAny<AddNhsUserResponse>()))
                .Returns(linkageResponse)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(request);

            // Assert
            var response = result.Should().BeAssignableTo<LinkageResult.SuccessfullyCreated>().Subject.Response;
            response.OdsCode.Should().Be(request.OdsCode);
            response.AccountId.Should().Be(nhsUserResponse.AccountId);
            response.LinkageKey.Should().Be(linkageResponse.LinkageKey);
            _mockIm1CacheKeyGenerator.Verify();
            _mockIm1CacheService.Verify();
            _mockLinkageMapper.Verify();
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsSupplierUnavailable_WhenHttpExceptionOccursCallingTpp()
        {
            // Arrange
            var request = ValidCreateLinkageRequest();

            _nhsUser
                .Setup(x => x.Post(It.IsAny<AddNhsUserRequest>()))
                .Throws<HttpRequestException>();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(request);

            // Assert
            result.Should().BeAssignableTo<LinkageResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsInternalServerError_WhenNoBodyOnNhsResponse()
        {
            // Arrange
            var request = ValidCreateLinkageRequest();
            var userResponse = new TppApiObjectResponse<AddNhsUserResponse>(HttpStatusCode.OK);

            _nhsUser
                .Setup(x => x.Post(It.IsAny<AddNhsUserRequest>()))
                .ReturnsAsync(userResponse);

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(request);

            // Assert
            result.Should().BeAssignableTo<LinkageResult.InternalServerError>();
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsSupplierSystemUnavailable_WhenUnauthorisedGpSystemHttpRequestExceptionOccurs()
        {
            // Arrange
            var request = ValidCreateLinkageRequest();
            var exception = new UnauthorisedGpSystemHttpRequestException();
            _nhsUser
                .Setup(x => x.Post(It.IsAny<AddNhsUserRequest>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(request);

            // Assert
            result.GetType().Should().Be(typeof(LinkageResult.SupplierSystemUnavailable));
        }
        
        [TestMethod]
        public async Task CreateLinkageKey_ReturnsUnknownError_WhenNotAuthenticated()
        {
            // Arrange, Act, Assert (via private helper)
            var result = await CreateLinkageKey_ReturnsError_WhenNotAuthenticated(TppApiErrorCodes.NotAuthenticated,
                typeof(LinkageResult.UnmappedErrorWithStatusCode));
            
            // Assert
            result.Should().BeAssignableTo<LinkageResult.UnmappedErrorWithStatusCode>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UnknownError);
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsNotFoundErrorCreatingNhsUser_WhenInvalidLinkageCredentials()
        {
            // Arrange, Act, Assert (via private helper)
            _settings.MinimumLinkageAge = 16;
            var result = await CreateLinkageKey_ReturnsError_WhenNotAuthenticated(
                "8",
                typeof(LinkageResult.ErrorCase));
            
            // Assert
            result.Should().BeAssignableTo<LinkageResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.ProblemLinkingAccount);
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsError_WhenUserUnder16()
        {
            // Arrange, Act, Assert (via private helper)
            _mockMinimumAgeValidator.Setup(x => x.IsValid(It.IsAny<DateTime>(), It.IsAny<int>())).Returns(false);

           var result = await CreateLinkageKey_ReturnsError_WhenNotAuthenticated(
               "8",
                typeof(LinkageResult.ErrorCase));
           
           // Assert
           result.Should().BeAssignableTo<LinkageResult.ErrorCase>()
               .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UnderMinimumAgeOrNonCompetent);
            _mockMinimumAgeValidator.Verify( x => x.IsValid(It.IsAny<DateTime>(), _settings.MinimumLinkageAge));
        }

        private async Task<LinkageResult> CreateLinkageKey_ReturnsError_WhenNotAuthenticated(string errorCode, Type expectedError)
        {
            // Arrange
            var request = ValidCreateLinkageRequest();
            var userResponse = new TppApiObjectResponse<AddNhsUserResponse>(HttpStatusCode.OK)
            {
                ErrorResponse = new Error { ErrorCode = errorCode }
            };

            _nhsUser
                .Setup(x => x.Post(It.IsAny<AddNhsUserRequest>()))
                .ReturnsAsync(userResponse);

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(request);

            // Assert
            result.GetType().Should().Be(expectedError);
            return result;
        }

        private CreateLinkageRequest ValidCreateLinkageRequest()
        {
            return _fixture.Build<CreateLinkageRequest>()
                .With(x => x.DateOfBirth, _dateOfBirth)
                .Create();
        }
    }
}