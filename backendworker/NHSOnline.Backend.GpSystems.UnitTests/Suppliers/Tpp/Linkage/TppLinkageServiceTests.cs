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
using NHSOnline.Backend.GpSystems.Im1Connection.Cache;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Temporal;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Linkage;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Linkage
{
    [TestClass]
    public class TppLinkageServiceTests
    {
        private IFixture _fixture;
        private Mock<ITppClientRequest<LinkAccountCreate, LinkAccountReply>> _linkAccountCreate;
        private Mock<ITppClientRequest<LinkAccountRetrieve, LinkAccountReply>> _linkAccountRetrieve;
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

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _linkAccountCreate = _fixture.Freeze<Mock<ITppClientRequest<LinkAccountCreate, LinkAccountReply>>>();
            _linkAccountRetrieve = _fixture.Freeze<Mock<ITppClientRequest<LinkAccountRetrieve, LinkAccountReply>>>();
            _mockIm1CacheKeyGenerator = _fixture.Freeze<Mock<IIm1CacheKeyGenerator>>();
            _mockIm1CacheService = _fixture.Freeze<Mock<IIm1CacheService>>();
            _mockLinkageMapper = _fixture.Freeze<Mock<ITppLinkageMapper>>();

            _settings = new ConfigurationSettings(CookieDomain,
                PrescriptionsDefaultLastNumberMonthsToDisplay,
                DefaultSessionExpiryMinutes,
                DefaultHttpTimeoutSeconds,
                MinimumAppAge,
                MinimumLinkageAge);

            _fixture.Inject(_settings);

            _mockMinimumAgeValidator = _fixture.Freeze<Mock<IMinimumAgeValidator>>();
            _mockMinimumAgeValidator.Setup(x => x.IsValid(It.IsAny<DateTime>(), It.IsAny<int>())).Returns(true);
            _dateOfBirth = DateTime.Now.AddYears(-16);  //Doesn't matter what age, as validator is mocked

            _systemUnderTest = _fixture.Create<TppLinkageService>();
        }

        [TestMethod]
        public async Task GetLinkageKey_ReturnsSuccessfullyRetrieved_ForValidRequest()
        {
            var request = _fixture.Create<GetLinkageRequest>();
            var linkAccountReply = _fixture.Create<LinkAccountReply>();

            MockRetrieveResponseForRequest(request, linkAccountReply);

            var linkageResponse = new LinkageResponse
            {
                AccountId = linkAccountReply.AccountId,
                OdsCode = request.OdsCode,
                LinkageKey = _fixture.Create<string>()
            };

            _mockLinkageMapper
                .Setup(x => x.Map(It.IsAny<LinkAccountRetrieve>(), It.IsAny<LinkAccountReply>()))
                .Returns(linkageResponse)
                .Verifiable();

            var result = await _systemUnderTest.GetLinkageKey(request);

            var response = result.Should().BeAssignableTo<LinkageResult.SuccessfullyRetrieved>().Subject.Response;
            response.Should().Be(linkageResponse);
            _mockLinkageMapper.Verify();
        }

        [TestMethod]
        public async Task GetLinkageKey_ReturnsNotFound_WhenAccountIdInResponseIsEmpty()
        {
            var request = _fixture.Create<GetLinkageRequest>();
            var linkAccountReply = _fixture.Create<LinkAccountReply>();
            linkAccountReply.AccountId = string.Empty;

            MockRetrieveResponseForRequest(request, linkAccountReply);

            var result = await _systemUnderTest.GetLinkageKey(request);

            var response = result.Should().BeAssignableTo<LinkageResult.NotFound>().Subject;
            response.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.NotValidForOnlineUser);
        }

        [TestMethod]
        public async Task GetLinkageKey_ReturnsNotFound_WhenPassphraseInResponseIsEmpty()
        {
            var request = _fixture.Create<GetLinkageRequest>();
            var linkAccountReply = _fixture.Create<LinkAccountReply>();
            linkAccountReply.PassphraseToLink = string.Empty;

            MockRetrieveResponseForRequest(request, linkAccountReply);

            var result = await _systemUnderTest.GetLinkageKey(request);

            var response = result.Should().BeAssignableTo<LinkageResult.NotFound>().Subject;
            response.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.NotValidForOnlineUser);
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsSuccessfulResponse_ForValidRequest()
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            var linkAccountReply = _fixture.Create<LinkAccountReply>();

            var linkageResponse = new LinkageResponse
            {
                AccountId = linkAccountReply.AccountId,
                OdsCode = request.OdsCode,
                LinkageKey = _fixture.Create<string>()
            };

            MockCreateResponseForRequest(request, linkAccountReply);

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
                .Setup(x => x.Map(It.IsAny<LinkAccountCreate>(), It.IsAny<LinkAccountReply>()))
                .Returns(linkageResponse)
                .Verifiable();

            var result = await _systemUnderTest.CreateLinkageKey(request);

            var response = result.Should().BeAssignableTo<LinkageResult.SuccessfullyCreated>().Subject.Response;
            response.OdsCode.Should().Be(request.OdsCode);
            response.AccountId.Should().Be(linkAccountReply.AccountId);
            response.LinkageKey.Should().Be(linkageResponse.LinkageKey);
            _mockIm1CacheKeyGenerator.Verify();
            _mockIm1CacheService.Verify();
            _mockLinkageMapper.Verify();
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsSupplierUnavailable_WhenHttpExceptionOccursCallingTpp()
        {
            var request = ValidCreateLinkageRequest();

            _linkAccountCreate
                .Setup(x => x.Post(It.IsAny<LinkAccountCreate>()))
                .Throws<HttpRequestException>();

            var result = await _systemUnderTest.CreateLinkageKey(request);

            result.Should().BeAssignableTo<LinkageResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsInternalServerError_WhenNoBodyOnNhsResponse()
        {
            var request = ValidCreateLinkageRequest();
            var userResponse = new TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK);

            _linkAccountCreate
                .Setup(x => x.Post(It.IsAny<LinkAccountCreate>()))
                .ReturnsAsync(userResponse);

            var result = await _systemUnderTest.CreateLinkageKey(request);

            result.Should().BeAssignableTo<LinkageResult.InternalServerError>();
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsSupplierSystemUnavailable_WhenUnauthorisedGpSystemHttpRequestExceptionOccurs()
        {
            var request = ValidCreateLinkageRequest();
            var exception = new UnauthorisedGpSystemHttpRequestException();
            _linkAccountCreate
                .Setup(x => x.Post(It.IsAny<LinkAccountCreate>()))
                .ThrowsAsync(exception);

            var result = await _systemUnderTest.CreateLinkageKey(request);

            result.GetType().Should().Be(typeof(LinkageResult.SupplierSystemUnavailable));
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsUnknownError_WhenNotAuthenticated()
        {
            var result = await CreateLinkageKey_ReturnsError_WhenNotAuthenticated(TppApiErrorCodes.NotAuthenticated,
                typeof(LinkageResult.UnmappedErrorWithStatusCode));

            result.Should().BeAssignableTo<LinkageResult.UnmappedErrorWithStatusCode>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UnknownError);
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsNotFoundErrorCreatingNhsUser_WhenInvalidLinkageCredentials()
        {
            _settings.MinimumLinkageAge = 16;
            var result = await CreateLinkageKey_ReturnsError_WhenNotAuthenticated(
                "8",
                typeof(LinkageResult.ErrorCase));

            result.Should().BeAssignableTo<LinkageResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.ProblemLinkingAccount);
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsError_WhenUserUnder16()
        {
            _mockMinimumAgeValidator.Setup(x => x.IsValid(It.IsAny<DateTime>(), It.IsAny<int>())).Returns(false);

           var result = await CreateLinkageKey_ReturnsError_WhenNotAuthenticated(
               "8",
                typeof(LinkageResult.ErrorCase));

           result.Should().BeAssignableTo<LinkageResult.ErrorCase>()
               .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UnderMinimumAgeOrNonCompetent);
            _mockMinimumAgeValidator.Verify( x => x.IsValid(It.IsAny<DateTime>(), _settings.MinimumLinkageAge));
        }

        [TestMethod]
        public async Task<LinkageResult> CreateLinkageKey_ReturnsError_WhenNotAuthenticated(string errorCode, Type expectedError)
        {
            var request = ValidCreateLinkageRequest();
            var userResponse = new TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
            {
                ErrorResponse = new Error { ErrorCode = errorCode }
            };

            _linkAccountCreate
                .Setup(x => x.Post(It.IsAny<LinkAccountCreate>()))
                .ReturnsAsync(userResponse);

            var result = await _systemUnderTest.CreateLinkageKey(request);

            result.GetType().Should().Be(expectedError);
            return result;
        }

        private CreateLinkageRequest ValidCreateLinkageRequest()
        {
            return _fixture.Build<CreateLinkageRequest>()
                .With(x => x.DateOfBirth, _dateOfBirth)
                .Create();
        }

        private void MockRetrieveResponseForRequest(GetLinkageRequest request, LinkAccountReply linkAccountReply)
        {
            var userResponse = new TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                { Body = linkAccountReply };

            _linkAccountRetrieve
                .Setup(x => x.Post(It.Is<LinkAccountRetrieve>(
                    x =>
                        x.NhsNumber == request.NhsNumber
                        && x.RetrieveOnly == "y"
                        && x.LastName == request.Surname
                        && x.DateofBirth == request.DateOfBirth
                        && x.OrganisationCode == request.OdsCode)))
                .ReturnsAsync(userResponse);
        }

        private void MockCreateResponseForRequest(CreateLinkageRequest request, LinkAccountReply linkAccountReply)
        {
            var userResponse = new TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                { Body = linkAccountReply };

            _linkAccountCreate
                .Setup(x => x.Post(It.Is<LinkAccountCreate>(
                    x =>
                        x.NhsNumber == request.NhsNumber
                        && x.LastName == request.Surname
                        && x.DateofBirth == request.DateOfBirth
                        && x.OrganisationCode == request.OdsCode
                        && x.EmailAddress == request.EmailAddress)))
                .ReturnsAsync(userResponse);
        }
    }
}
