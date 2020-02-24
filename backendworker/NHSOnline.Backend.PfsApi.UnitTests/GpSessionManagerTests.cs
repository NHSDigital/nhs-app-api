using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests
{
    [TestClass]
    public class GpSessionManagerTests
    {
        private const string DateFormat = "yyyy-MM-dd";

        private GpSessionManager _gpSessionManager;
        private IFixture _fixture;

        private StringValues _csrfToken;
        private UserSession _userSession;
        private string _sessionId;
        private UserProfile _userProfile;
        private CitizenIdSessionResult _citizenIdSessionResult;
        
        private Mock<ISessionCacheService> _mockSessionCacheService;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<ILogger<GpSessionManager>> _mockLogger;
        private Mock<ISessionMapper> _mockSessionMapper;
    
        [TestInitialize]
        public void TestInitialize()
        {            
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _csrfToken = _fixture.Freeze<StringValues>();
            _sessionId = _fixture.Freeze<string>();
            _userProfile = _fixture.Freeze<UserProfile>();
            _userProfile.DateOfBirth = DateTime.Now.ToString(DateFormat, CultureInfo.InvariantCulture);
            _userProfile.Im1ConnectionToken = _fixture.Create<EmisConnectionToken>().SerializeJson();
            
            var emisUserSession = _fixture.Create<EmisUserSession>();
            emisUserSession.OdsCode = _userProfile.OdsCode;
            emisUserSession.NhsNumber = _userProfile.NhsNumber;
            emisUserSession.Name = _fixture.Create<string>();
            
            var citizenIdUserSession = new CitizenIdUserSession { AccessToken = _userProfile.AccessToken };

            _citizenIdSessionResult = new CitizenIdSessionResult
            {
                StatusCode = (int) HttpStatusCode.OK,
                DateOfBirth = DateTime.ParseExact(_userProfile.DateOfBirth, DateFormat, CultureInfo.InvariantCulture),
                Im1ConnectionToken = _userProfile.Im1ConnectionToken,
                NhsNumber = _userProfile.NhsNumber,
                OdsCode = _userProfile.OdsCode,
                Session = citizenIdUserSession
            };
            
            _userSession = new UserSession
            {
                CsrfToken = _csrfToken,
                GpUserSession = emisUserSession,
                CitizenIdUserSession = citizenIdUserSession,
                OrganDonationSessionId =  Guid.NewGuid(),
                Im1ConnectionToken = _userProfile.Im1ConnectionToken
            }; 
             
            var mockHttpContextAccessor = _fixture.Freeze<Mock<IHttpContextAccessor>>();
            _mockLogger = _fixture.Freeze<Mock<ILogger<GpSessionManager>>>();
            _mockSessionMapper = _fixture.Freeze<Mock<ISessionMapper>>();
            _mockGpSystem = _fixture.Create<Mock<IGpSystem>>();
            _mockSessionService = _fixture.Freeze<Mock<ISessionService>>();
            _mockSessionCacheService = _fixture.Freeze<Mock<ISessionCacheService>>();
            
            _mockSessionCacheService
                .Setup(x => x.GetUserSession(_sessionId))
                .Returns(Task.FromResult(Option.Some(_userSession)))
                .Verifiable();
            
            _mockSessionCacheService.Setup(x => x.CreateUserSession(_userSession))
                .ReturnsAsync(_sessionId)
                .Verifiable();
            
            _mockSessionService
                .Setup(x => x.Create(_userProfile.Im1ConnectionToken, _userProfile.OdsCode, _userProfile.NhsNumber))
                .Returns(Task.FromResult(
                    (GpSessionCreateResult)new GpSessionCreateResult.Success(emisUserSession)));
            
            _mockGpSystem
                .SetupGet(x => x.Supplier)
                .Returns(Supplier.Emis);
            
            _mockGpSystem
                .Setup(x => x.GetSessionService())
                .Returns(_mockSessionService.Object);
            
            _mockSessionMapper
                .Setup(x => x.Map(
                    mockHttpContextAccessor.Object.HttpContext, 
                    emisUserSession, 
                    citizenIdUserSession, 
                    _userProfile.Im1ConnectionToken))
                .Returns(_userSession)
                .Verifiable();
     
            //create the class under test
            _gpSessionManager = _fixture.Create<GpSessionManager>();
        }
        
       [TestMethod]
        public async Task CreateSession_ReturnsCreateSessionResult_Failure()
        {
            // Arrange            
            _mockSessionService
                .Setup(x => x.Create(_userProfile.Im1ConnectionToken, _userProfile.OdsCode, _userProfile.NhsNumber))
                .ReturnsAsync(new GpSessionCreateResult.BadGateway())
                .Verifiable();
            
            // Act
            var result = await _gpSessionManager.CreateSession(_mockGpSystem.Object, _citizenIdSessionResult);

            // Assert
            var failureResult = result.Should().BeOfType<CreateSessionResult.Failure>().Subject;  
            failureResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            
            _mockSessionCacheService.Verify(x => x.CreateUserSession(It.IsAny<UserSession>()), Times.Never());
            _mockSessionService.Verify();
            _mockLogger.VerifyLogger(LogLevel.Debug,$"Fetched Session Id: sessionId", Times.Never());
        }
        
        [TestMethod]
        public async Task CreateSession_ReturnsCreateSessionResult_Success()
        {            
            // Act
            var result = await _gpSessionManager.CreateSession(_mockGpSystem.Object, _citizenIdSessionResult);

            // Assert
            var successResult = result.Should().BeOfType<CreateSessionResult.Success>().Subject;   
            successResult.UserSession.Should().BeEquivalentTo(_userSession);

            _mockLogger.VerifyLogger(LogLevel.Debug,$"Fetched Session Id: '{_sessionId}'", Times.Once());
            _mockSessionService.Verify();
            _mockSessionMapper.Verify();
            _mockSessionCacheService.Verify(x => x.CreateUserSession(_userSession));
        }
    
        [TestMethod]
        public async Task RetrieveSession_ReturnsSuccess_WhenSessionIdAndTokenAreValid()
        {
            //Act
            var retrieveSessionResult = await _gpSessionManager.RetrieveSession(_sessionId, _csrfToken);

            //Assert
            var result = retrieveSessionResult.Should().BeAssignableTo<RetrieveSessionResult.Success>();
            result.Subject.UserSession.Should().BeEquivalentTo(_userSession);
            _mockLogger.VerifyLogger(LogLevel.Warning, Times.Never());
            _mockLogger.VerifyLogger(LogLevel.Information, Times.Once());
            _mockSessionCacheService.Verify(x => x.GetUserSession(_sessionId));
        }
    
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task RetrieveSession_ReturnsFailure_WhenSessionIdIsMissing(string sessionId)
        {            
            //Act
            var retrieveSessionResult = await _gpSessionManager.RetrieveSession(sessionId, _csrfToken);

            //Assert
            retrieveSessionResult.Should().BeAssignableTo<RetrieveSessionResult.Failure>();    
            _mockLogger.VerifyLogger(LogLevel.Warning, Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Information, Times.Never());
        }  
        
        [TestMethod]
        public async Task RetrieveSession_ReturnsFailure_WhenNoSessionIsReturnedFromCacheService()
        {
            //Arrange
            _mockSessionCacheService
                .Setup(x => x.GetUserSession(_sessionId))
                .Returns(Task.FromResult(Option.None<UserSession>()))
                .Verifiable();
            
            //Act
            var retrieveSessionResult = await _gpSessionManager.RetrieveSession(_sessionId, _csrfToken);

            //Assert
            retrieveSessionResult.Should().BeAssignableTo<RetrieveSessionResult.Failure>();
            _mockLogger.VerifyLogger(LogLevel.Warning, Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Information, Times.Never());
            _mockSessionCacheService.Verify(x => x.GetUserSession(_sessionId));
        }
        
        [TestMethod]
        public async Task RetrieveSession_ReturnsFailure_WhenCsrfTokenDoesNotMatch()
        {   
            //Act
            var retrieveSessionResult = await _gpSessionManager.RetrieveSession(_sessionId, new StringValues("badToken"));

            //Assert
            retrieveSessionResult.Should().BeAssignableTo<RetrieveSessionResult.Failure>();
            _mockLogger.VerifyLogger(LogLevel.Warning, Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Information, Times.Never());
            _mockSessionCacheService.Verify(x => x.GetUserSession(_sessionId));
        }
    }
}