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
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Im1Connection
{
    [TestClass]
    public class TppIm1ConnectionServiceTests
    {
        private const string DefaultConnectionToken = "{\"accountid\":\"account_id\",\"passphrase\":\"passphrase\",\"providerid\":\"providerid\"}";
        private const string DefaultOdsCode = "token";

        private IFixture _fixture;
        private Mock<ITppClient> _mockTppClient;
        private Mock<IIm1CacheService> _mockIm1CacheService;
        private ILogger<TppIm1ConnectionService> _logger;
        private Mock<IIm1CacheKeyGenerator> _mockIm1CacheKeyGenerator;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockTppClient = _fixture.Freeze<Mock<ITppClient>>();
            _mockIm1CacheService = _fixture.Freeze<Mock<IIm1CacheService>>();
            _mockIm1CacheKeyGenerator = _fixture.Freeze<Mock<IIm1CacheKeyGenerator>>();
            _logger = Mock.Of<ILogger<TppIm1ConnectionService>>();
        }

        [TestMethod]
        public async Task Verify_ReturnsAConnection_WhenRequested()
        {
            var authenticateReply = _fixture.Create<AuthenticateReply>();

            var expectedNhsNumbers = new List<PatientNhsNumber>
            {
                new PatientNhsNumber
                {
                    NhsNumber = authenticateReply.User?.Person?.NationalId?.Value
                }
            };

            _mockTppClient.Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        Body = authenticateReply,
                    }));

            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object, _mockIm1CacheService.Object
                , _mockIm1CacheKeyGenerator.Object, _logger);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            var successResult = result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SuccessfullyVerified>()
                .Subject;

            successResult.Response.ConnectionToken.Should().Be(DefaultConnectionToken);
            successResult.Response.NhsNumbers.Should().BeEquivalentTo(expectedNhsNumbers);
        }

        [TestMethod]
        public async Task Verify_ReturnsAEmptyNhsNumbers_WhenTppRespondsWithEmptyNhsNumber()
        {
            _mockTppClient.Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        Body = null
                    }));

            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object, _mockIm1CacheService.Object
                , _mockIm1CacheKeyGenerator.Object, _logger);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            var successResult = result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SuccessfullyVerified>()
                .Subject;

            successResult.Response.ConnectionToken.Should().Be(DefaultConnectionToken);
            successResult.Response.NhsNumbers.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Verify_ReturnsSupplierSystemUnavailable_WhenTppClientReturnsErrorResponse()
        {
            _mockTppClient.Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = _fixture.Create<Error>()
                    }));

            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object,  _mockIm1CacheService.Object
                , _mockIm1CacheKeyGenerator.Object,_logger);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Verify_ReturnsSupplierSystemUnavailable_WhenTppClientReturnsBadgateway()
        {
            _mockTppClient.Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.BadGateway)
                    {
                        ErrorResponse = null
                    }));

            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object,  _mockIm1CacheService.Object
                , _mockIm1CacheKeyGenerator.Object,_logger);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Register_SuccessfullyRegistered_WhenIm1TokenIsCached()
        {
            var linkAccountReply = _fixture.Create<LinkAccountReply>();
            _mockTppClient.Setup(x => x.LinkAccountPost(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        Body = linkAccountReply,
                    }));
            
            var authenticateReply = _fixture.Create<AuthenticateReply>();
            _mockTppClient.Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        Body = authenticateReply,
                    }));
            
            string key = "Key";
            _mockIm1CacheKeyGenerator.Setup(x => x.GenerateCacheKey(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(key);

            var im1Token = _fixture.Create<TppConnectionToken>();
            
            _mockIm1CacheService.Setup(x => x.GetIm1ConnectionToken<TppConnectionToken>(key))
                .Returns(Task.FromResult(
                    Option.Some(im1Token)
                ));
            
            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object,  _mockIm1CacheService.Object
                , _mockIm1CacheKeyGenerator.Object,_logger);         
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var result = await systemUnderTest.Register(request);

            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.SuccessfullyRegistered>();
            _mockIm1CacheService.Verify(x => x.GetIm1ConnectionToken<TppConnectionToken>(key), Times.Once);
            _mockTppClient.Verify(x => x.LinkAccountPost(It.IsAny<LinkAccount>()), Times.Never);
        }
        
        [TestMethod]
        public async Task Register_SuccessfullyRegistered_WhenDataAreCorrect()
        {
            var linkAccountReply = _fixture.Create<LinkAccountReply>();
            _mockTppClient.Setup(x => x.LinkAccountPost(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        Body = linkAccountReply,
                    }));
            
            var authenticateReply = _fixture.Create<AuthenticateReply>();
            _mockTppClient.Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        Body = authenticateReply,
                    }));
            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object,  _mockIm1CacheService.Object
                , _mockIm1CacheKeyGenerator.Object,_logger);         
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var result = await systemUnderTest.Register(request);

            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.SuccessfullyRegistered>();
        }
        
        [TestMethod]
        public async Task Register_ReturnsSupplierSystemUnavailable_WhenTppClientLinkAccountReturnsInvalidProviderId()
        {
            var invalidProviderErrorResponse = _fixture.Build<Error>()
                .With(x => x.ErrorCode, TppApiErrorCodes.LinkAccount.InvalidProviderId)
                .Create();
                
            _mockTppClient.Setup(x => x.LinkAccountPost(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = invalidProviderErrorResponse
                    }));
            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object,  _mockIm1CacheService.Object
                , _mockIm1CacheKeyGenerator.Object,_logger);         
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var result = await systemUnderTest.Register(request);

            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Register_ReturnsNotFound_WhenTppClientLinkAccountReturnsInvalidLinkageCredentials()
        {
            var invalidLinkageCredentialsErrorResponse = _fixture.Build<Error>()
                .With(x => x.ErrorCode, TppApiErrorCodes.LinkAccount.InvalidLinkageCredentials)
                .Create();
                
            _mockTppClient.Setup(x => x.LinkAccountPost(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = invalidLinkageCredentialsErrorResponse
                    }));
            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object,  _mockIm1CacheService.Object
                , _mockIm1CacheKeyGenerator.Object,_logger);         
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var result = await systemUnderTest.Register(request);

            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.NotFound>();
        }
        
        [TestMethod]
        public async Task Register_ReturnsSupplierSystemUnavailable_WhenTppClientLinkAccountReturnsErrorResponse()
        {
            var errorResponse = _fixture.Create<Error>();                
            _mockTppClient.Setup(x => x.LinkAccountPost(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = errorResponse
                    }));
            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object,  _mockIm1CacheService.Object
                , _mockIm1CacheKeyGenerator.Object,_logger);         
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var result = await systemUnderTest.Register(request);

            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Register_ReturnsSupplierSystemUnavailable_WhenTppClientAuthenticateReturnsErrorResponse()
        {
            var linkAccountReply = _fixture.Create<LinkAccountReply>();
            _mockTppClient.Setup(x => x.LinkAccountPost(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        Body = linkAccountReply,
                    }));
            
            var errorResponse = _fixture.Create<Error>();                
            _mockTppClient.Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
                    {
                        ErrorResponse = errorResponse
                    }));
            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object,  _mockIm1CacheService.Object
                , _mockIm1CacheKeyGenerator.Object,_logger);         
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var result = await systemUnderTest.Register(request);

            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Verify_ReturnsSupplierSystemUnavailable_WhenTppClientLinkAccountThrowsHttpRequestException()
        {
            _mockTppClient
                .Setup(x => x.LinkAccountPost(It.IsAny<LinkAccount>()))
                .Throws<HttpRequestException>();
            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object,  _mockIm1CacheService.Object
                , _mockIm1CacheKeyGenerator.Object,_logger);
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var result = await systemUnderTest.Register(request);

            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Verify_ReturnsSupplierSystemUnavailable_WhenTppClientAuthenticateThrowsHttpRequestException()
        {
            var linkAccountReply = _fixture.Create<LinkAccountReply>();
            _mockTppClient.Setup(x => x.LinkAccountPost(It.IsAny<LinkAccount>())).Returns(
                Task.FromResult(
                    new TppClient.TppApiObjectResponse<LinkAccountReply>(HttpStatusCode.OK)
                    {
                        Body = linkAccountReply,
                    }));
            
            _mockTppClient
                .Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>()))
                .Throws<HttpRequestException>();
            var systemUnderTest = new TppIm1ConnectionService(_mockTppClient.Object,  _mockIm1CacheService.Object
                , _mockIm1CacheKeyGenerator.Object,_logger);
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var result = await systemUnderTest.Register(request);

            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.SupplierSystemUnavailable>();
        }
    }
}