using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.Areas.Session;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using static NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.TppClient;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Linkage
{
    [TestClass]
    public class TppLinkageServiceTests
    {
        private IFixture _fixture;
        private Mock<ITppClient> _tppClient;
        private Mock<IRegistrationGuidKeyGenerator> _mockRegistrationGuidKeyGenerator;
        private Mock<IRegistrationCacheService> _mockRegistrationCacheService;
        private Mock<ITppLinkageMapper> _mockLinkageMapper;
        private TppLinkageService _systemUnderTest;
        private Mock<IMinimumAgeValidator> _mockMinimumAgeValidator;
        private DateTime _dateOfBirth;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _tppClient = _fixture.Freeze<Mock<ITppClient>>();
            _mockRegistrationGuidKeyGenerator = _fixture.Freeze<Mock<IRegistrationGuidKeyGenerator>>();
            _mockRegistrationCacheService = _fixture.Freeze<Mock<IRegistrationCacheService>>();
            _mockLinkageMapper = _fixture.Freeze<Mock<ITppLinkageMapper>>();


            _mockMinimumAgeValidator = _fixture.Freeze<Mock<IMinimumAgeValidator>>();
            _mockMinimumAgeValidator.Setup(x => x.IsValid(It.IsAny<DateTime>())).Returns(true);
            _dateOfBirth = DateTime.Now.AddYears(-16);  //Doesn't matter what age, as validator is mocked

            _systemUnderTest = _fixture.Create<TppLinkageService>();
        }

        [TestMethod]
        public async Task GetLinkageKey_ReturnsNotFound_ForValidRequest()
        {
            var request = _fixture.Create<GetLinkageRequest>();

            var response = await _systemUnderTest.GetLinkageKey(request);

            response.Should().BeAssignableTo<LinkageResult.NotFoundErrorRetrievingNhsUser>();
        }
        
        [TestMethod]
        public async Task GetLinkageKey_ReturnsNotFound_ForNullRequest()
        {
            var response = await _systemUnderTest.GetLinkageKey(null);

            response.Should().BeAssignableTo<LinkageResult.NotFoundErrorRetrievingNhsUser>();
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsSuccessfulResponse_ForValidRequest()
        {
            //Arrange
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

            _tppClient
                .Setup(x => x.NhsUserPost(It.IsAny<AddNhsUserRequest>()))
                .ReturnsAsync(userResponse);

            string key = "CACHEKEY";
            _mockRegistrationGuidKeyGenerator
                .Setup(x => x.GenerateRegistrationKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(key)
                .Verifiable();

            _mockRegistrationCacheService
                .Setup(x => x.CreateRegistrationToken(key, It.IsAny<TppConnectionToken>()))
                .ReturnsAsync("ENCRYPTED KEY")
                .Verifiable();

            _mockLinkageMapper
                .Setup(x => x.Map(It.IsAny<AddNhsUserRequest>(), It.IsAny<AddNhsUserResponse>()))
                .Returns(linkageResponse)
                .Verifiable();

            //Act
            var result = await _systemUnderTest.CreateLinkageKey(request);

            //Assert
            result.Should().BeAssignableTo<LinkageResult.SuccessfullyCreated>();
            var successResult = (LinkageResult.SuccessfullyCreated) result;
            successResult.Response.Should().NotBeNull();
            successResult.Response.OdsCode.Should().Be(request.OdsCode);
            successResult.Response.AccountId.Should().Be(nhsUserResponse.AccountId);
            successResult.Response.LinkageKey.Should().Be(linkageResponse.LinkageKey);
            _mockRegistrationGuidKeyGenerator.Verify();
            _mockRegistrationCacheService.Verify();
            _mockLinkageMapper.Verify();
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsSupplierUnavailable_WhenHttpExceptionOccursCallingTpp()
        {
            //Arrange
            var request = ValidCreateLinkageRequest();

            _tppClient
                .Setup(x => x.NhsUserPost(It.IsAny<AddNhsUserRequest>()))
                .Throws<HttpRequestException>();

            //Act
            var result = await _systemUnderTest.CreateLinkageKey(request);

            //Assert
            result.Should().BeAssignableTo<LinkageResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsInternalServerError_WhenNoBodyOnNhsResponse()
        {
            //Arrange
            var request = ValidCreateLinkageRequest();
            var userResponse = new TppApiObjectResponse<AddNhsUserResponse>(HttpStatusCode.OK);

            _tppClient
                .Setup(x => x.NhsUserPost(It.IsAny<AddNhsUserRequest>()))
                .ReturnsAsync(userResponse);

            //Act
            var result = await _systemUnderTest.CreateLinkageKey(request);

            //Assert
            result.Should().BeAssignableTo<LinkageResult.InternalServerError>();
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsSupplierSystemUnavailable_WhenNotAuthenticated()
        {
            await CreateLinkageKey_ReturnsError_WhenNotAuthenticated(TppApiErrorCodes.NotAuthenticated,
                typeof(LinkageResult.SupplierSystemUnavailable));
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsNotFoundErrorCreatingNhsUser_WhenInvalidLinkageCredentials()
        {
            await CreateLinkageKey_ReturnsError_WhenNotAuthenticated(
                TppApiErrorCodes.LinkAccount.InvalidLinkageCredentials,
                typeof(LinkageResult.NotFoundErrorCreatingNhsUser));
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsInternalServerError_WhenNotAuthenticated()
        {
            await CreateLinkageKey_ReturnsError_WhenNotAuthenticated(TppApiErrorCodes.LinkAccount.InvalidProviderId,
                typeof(LinkageResult.InternalServerError));
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsError_WhenUserUnder16()
        {
            _mockMinimumAgeValidator.Setup(x => x.IsValid(It.IsAny<DateTime>())).Returns(false);

            await CreateLinkageKey_ReturnsError_WhenNotAuthenticated(TppApiErrorCodes.LinkAccount.InvalidLinkageCredentials,
                typeof(LinkageResult.PatientNonCompetentOrUnderMinimumAge));
        }

        private async Task CreateLinkageKey_ReturnsError_WhenNotAuthenticated(string errorCode, Type expectedError)
        {
            //Arrange
            var request = ValidCreateLinkageRequest();
            var userResponse = new TppApiObjectResponse<AddNhsUserResponse>(HttpStatusCode.OK)
            {
                ErrorResponse = new Error { ErrorCode = errorCode }
            };

            _tppClient
                .Setup(x => x.NhsUserPost(It.IsAny<AddNhsUserRequest>()))
                .ReturnsAsync(userResponse);

            //Act
            var result = await _systemUnderTest.CreateLinkageKey(request);

            //Assert
            result.GetType().Should().Be(expectedError);
        }

        private CreateLinkageRequest ValidCreateLinkageRequest()
        {
            var request = _fixture.Create<CreateLinkageRequest>();
            request.DateOfBirth = _dateOfBirth;
            return request;
        }
    }
}