using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection.Cache;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;
using UnitTestHelper;
using Im1ConnectionErrorCodes = NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Im1Connection
{
    [TestClass]
    public class TppIm1ConnectionServiceTests
    {
        private const string DefaultConnectionToken =
            "{\"accountid\":\"account_id\",\"passphrase\":\"passphrase\",\"providerid\":\"providerid\"}";

        private const string DefaultOdsCode = "token";

        private IFixture _fixture;
        private Mock<ITppClientRequest<LinkAccount, LinkAccountReply>> _mockLinkAccount;
        private Mock<ITppClientRequest<Authenticate, AuthenticateReply>> _mockAuthenticate;
        private Mock<IIm1CacheService> _mockIm1CacheService;
        private Mock<IIm1CacheKeyGenerator> _mockIm1CacheKeyGenerator;
        private Mock<ILogger<TppIm1ConnectionService>> _mockLogger;


        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockLinkAccount = _fixture.Freeze<Mock<ITppClientRequest<LinkAccount, LinkAccountReply>>>();
            _mockAuthenticate = _fixture.Freeze<Mock<ITppClientRequest<Authenticate, AuthenticateReply>>>();
            _mockIm1CacheService = _fixture.Freeze<Mock<IIm1CacheService>>();
            _mockIm1CacheKeyGenerator = _fixture.Freeze<Mock<IIm1CacheKeyGenerator>>();
            _mockLogger = _fixture.Freeze<Mock<ILogger<TppIm1ConnectionService>>>();
        }

        [TestMethod]
        public async Task Verify_ReturnsAConnection_WhenRequested()
        {
            // Arrange
            var authenticateReply = _fixture.Create<AuthenticateReply>();

            var expectedNhsNumbers = new List<PatientNhsNumber>
            {
                new PatientNhsNumber
                {
                    NhsNumber = authenticateReply.User?.Person?.NationalId?.Value
                }
            };

            _mockAuthenticate.Setup(x => x.Post(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        Body = authenticateReply,
                    }));

            var systemUnderTest = CreateSystemUnderTest();

            // Act
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            var successResult = result.Should().BeAssignableTo<Im1ConnectionVerifyResult.Success>()
                .Subject;

            successResult.Response.ConnectionToken.Should().Be(DefaultConnectionToken);
            successResult.Response.NhsNumbers.Should().BeEquivalentTo(expectedNhsNumbers);
        }

        [TestMethod]
        public async Task Verify_ReturnsAEmptyNhsNumbers_WhenTppRespondsWithEmptyNhsNumber()
        {
            // Arrange
            _mockAuthenticate.Setup(x => x.Post(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        Body = null
                    }));

            var systemUnderTest = CreateSystemUnderTest();

            // Act
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            var successResult = result.Should().BeAssignableTo<Im1ConnectionVerifyResult.Success>()
                .Subject;

            successResult.Response.ConnectionToken.Should().Be(DefaultConnectionToken);
            successResult.Response.NhsNumbers.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Verify_ReturnsUnmappedErrorWithStatusCode_WhenTppClientReturnsErrorResponse()
        {
            // Arrange
            _mockAuthenticate.Setup(x => x.Post(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = _fixture.Create<Error>()
                    }));

            var systemUnderTest = CreateSystemUnderTest();

            // Act
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            //Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.UnmappedErrorWithStatusCode>();
        }

        [TestMethod]
        public async Task Verify_ReturnsBadGateway_WhenTppClientReturnsBadGateway()
        {
            // Arrange
            _mockAuthenticate.Setup(x => x.Post(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.BadGateway)
                    {
                        ErrorResponse = null
                    }));

            var systemUnderTest = CreateSystemUnderTest();

            // Act
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            //Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.UnmappedErrorWithStatusCode>();
        }

        [TestMethod]
        public async Task Verify_LoggerReturnsCorrectNHSNumberCount()
        {
            // Arrange
            var authenticateReply = _fixture.Create<AuthenticateReply>();
            _mockAuthenticate.Setup(x => x.Post(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        Body = authenticateReply,
                    }));

            authenticateReply.User.Person.NationalId.Value = "abc";

            var systemUnderTest = CreateSystemUnderTest();

            // Act
            await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            //Assert
            const string expectedLogMessage = "Tpp returned 1 NHS Numbers for the user";
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }

        [TestMethod]
        public async Task Verify_LoggerReturnsCorrectZeroNumberCount_WhenNoNHSNumbersAreReturned()
        {
            // Arrange
            var authenticateReply = _fixture.Create<AuthenticateReply>();
            _mockAuthenticate.Setup(x => x.Post(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        Body = authenticateReply,
                    }));

            authenticateReply.User.Person.NationalId.Value = null;

            var systemUnderTest = CreateSystemUnderTest();

            // Act
            await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            //Assert
            const string expectedLogMessage = "Tpp returned 0 NHS Numbers for the user";
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }

        [TestMethod]
        public async Task Register_Success_WhenIm1TokenIsCached()
        {
            // Arrange
            var linkAccountReply = _fixture.Create<LinkAccountReply>();
            _mockLinkAccount.Setup(x => x.Post(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        Body = linkAccountReply,
                    }));

            var authenticateReply = _fixture.Create<AuthenticateReply>();
            _mockAuthenticate.Setup(x => x.Post(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        Body = authenticateReply,
                    }));

            const string key = "Key";
            _mockIm1CacheKeyGenerator.Setup(x => x.GenerateCacheKey(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(key);

            var im1Token = _fixture.Create<TppConnectionToken>();

            _mockIm1CacheService.Setup(x => x.GetIm1ConnectionToken<TppConnectionToken>(key))
                .Returns(Task.FromResult(
                    Option.Some(im1Token)
                ));

            var systemUnderTest = CreateSystemUnderTest();
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            // Act
            var result = await systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.Success>();
            _mockIm1CacheService.Verify(x => x.GetIm1ConnectionToken<TppConnectionToken>(key), Times.Once);
            _mockLinkAccount.Verify(x => x.Post(It.IsAny<LinkAccount>()), Times.Never);
        }

        [TestMethod]
        public async Task Register_Success_WhenDataAreCorrect()
        {
            // Arrange
            var linkAccountReply = _fixture.Create<LinkAccountReply>();
            _mockLinkAccount.Setup(x => x.Post(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        Body = linkAccountReply,
                    }));

            var authenticateReply = _fixture.Create<AuthenticateReply>();
            _mockAuthenticate.Setup(x => x.Post(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        Body = authenticateReply,
                    }));

            const string key = "Key";
            _mockIm1CacheKeyGenerator.Setup(x => x.GenerateCacheKey(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(key);

            _mockIm1CacheService.Setup(x => x.GetIm1ConnectionToken<TppConnectionToken>(key))
                .Returns(Task.FromResult(
                    Option.None<TppConnectionToken>()
                )).Verifiable();

            _mockIm1CacheService.Setup(x => x.SaveIm1ConnectionToken(key,
                    It.IsAny<TppConnectionToken>()))
                .Returns(Task.FromResult(true))
                .Verifiable();

            var systemUnderTest = CreateSystemUnderTest();
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            // Act
            var result = await systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.Success>();

            _mockIm1CacheService.Verify();
        }

        [TestMethod]
        public async Task Register_LoggerReturnsNhsNumberCountOfZero_WhenNHSNumbersReturnedIsNull()
        {
            // Arrange
            var request = SetUpForRegister(null);

            var systemUnderTest = CreateSystemUnderTest();

            // Act
            await systemUnderTest.Register(request);

            const string expectedLogMessage = "Tpp returned 0 NHS Numbers for the user";
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }

        [TestMethod]
        public async Task Register_LoggerReturnsCorrectNhsNumberCount()
        {
            // Arrange
            var request = SetUpForRegister("abc");

            var systemUnderTest = CreateSystemUnderTest();

            // Act
            await systemUnderTest.Register(request);

            const string expectedLogMessage = "Tpp returned 1 NHS Numbers for the user";
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }


        [TestMethod]
        public async Task Register_ReturnsBadGateway_WhenTppClientLinkAccountReturnsInvalidProviderId()
        {
            // Arrange
            var invalidProviderErrorResponse = _fixture.Build<Error>()
                .With(x => x.ErrorCode, "6")
                .Create();

            _mockLinkAccount.Setup(x => x.Post(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = invalidProviderErrorResponse
                    }));
            var systemUnderTest = CreateSystemUnderTest();
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            // Act
            var result = await systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.InvalidLinkageDetailsTpp);
        }

        [TestMethod]
        public async Task Register_ReturnsNotFound_WhenTppClientLinkAccountReturnsInvalidLinkageCredentials()
        {
            // Arrange
            var invalidLinkageCredentialsErrorResponse = _fixture.Build<Error>()
                .With(x => x.ErrorCode, "8")
                .Create();

            _mockLinkAccount.Setup(x => x.Post(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = invalidLinkageCredentialsErrorResponse
                    }));
            var systemUnderTest = CreateSystemUnderTest();
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            // Act
            var result = await systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.InvalidLinkageDetailsTpp);
        }

        [TestMethod]
        public async Task Register_ReturnsUnknownError_WhenTppClientLinkAccountReturnsErrorResponse()
        {
            // Arrange
            var errorResponse = _fixture.Create<Error>();
            _mockLinkAccount.Setup(x => x.Post(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = errorResponse
                    }));
            var systemUnderTest = CreateSystemUnderTest();
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            // Act
            var result = await systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.UnmappedErrorWithStatusCode>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UnknownError);
        }

        [TestMethod]
        public async Task Register_ReturnsBadGateway_WhenTppClientAuthenticateReturnsErrorResponse()
        {
            // Arrange
            var linkAccountReply = _fixture.Create<LinkAccountReply>();
            _mockLinkAccount.Setup(x => x.Post(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        Body = linkAccountReply,
                    }));

            var errorResponse = _fixture.Create<Error>();
            _mockAuthenticate.Setup(x => x.Post(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = errorResponse
                    }));

            var systemUnderTest = CreateSystemUnderTest();
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            // Act
            var result = await systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.BadGateway>();
        }

        [TestMethod]
        public async Task Verify_ReturnsBadGateway_WhenTppClientLinkAccountThrowsHttpRequestException()
        {
            // Arrange
            _mockLinkAccount
                .Setup(x => x.Post(It.IsAny<LinkAccount>()))
                .Throws<HttpRequestException>();
            var systemUnderTest = CreateSystemUnderTest();
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            // Act
            var result = await systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.BadGateway>();
        }

        [TestMethod]
        public async Task Verify_ReturnsBadGateway_WhenTppClientAuthenticateThrowsHttpRequestException()
        {
            // Arrange
            var linkAccountReply = _fixture.Create<LinkAccountReply>();
            _mockLinkAccount.Setup(x => x.Post(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        Body = linkAccountReply,
                    }));

            _mockAuthenticate
                .Setup(x => x.Post(It.IsAny<Authenticate>()))
                .Throws<HttpRequestException>();
            var systemUnderTest = CreateSystemUnderTest();
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            // Act
            var result = await systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.BadGateway>();
        }

        private TppIm1ConnectionService CreateSystemUnderTest()
        {
            return new TppIm1ConnectionService(_mockLinkAccount.Object, _mockAuthenticate.Object, _mockIm1CacheService.Object,
                _mockIm1CacheKeyGenerator.Object, _mockLogger.Object);
        }

        private PatientIm1ConnectionRequest SetUpForRegister(string nhsNumber)
        {
            var linkAccountReply = _fixture.Create<LinkAccountReply>();
            _mockLinkAccount.Setup(x => x.Post(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        Body = linkAccountReply,
                    }));

            var authenticateReply = _fixture.Create<AuthenticateReply>();

            authenticateReply.User.Person.NationalId.Value = nhsNumber;

            _mockAuthenticate.Setup(x => x.Post(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        Body = authenticateReply
                    }));

            const string key = "Key";
            _mockIm1CacheKeyGenerator.Setup(x => x.GenerateCacheKey(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(key);

            _mockIm1CacheService.Setup(x => x.GetIm1ConnectionToken<TppConnectionToken>(key))
                .Returns(Task.FromResult(
                    Option.None<TppConnectionToken>()
                )).Verifiable();

            _mockIm1CacheService.Setup(x => x.SaveIm1ConnectionToken(key,
                    It.IsAny<TppConnectionToken>()))
                .Returns(Task.FromResult(true))
                .Verifiable();

            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            return request;
        }
    }
}