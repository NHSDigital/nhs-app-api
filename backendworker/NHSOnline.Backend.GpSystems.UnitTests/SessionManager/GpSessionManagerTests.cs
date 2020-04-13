using System;
using System.Globalization;
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
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.SessionManager.Model;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.SessionManager
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
        private string _patientId;
        private UserProfile _userProfile;
        private GpSessionManagerCitizenIdSessionResult _gpSessMgrCitizenIdSessionResult;

        private Mock<ISessionCacheService> _mockSessionCacheService;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<ILogger<GpSessionManager>> _mockLogger;
        private Mock<ISessionMapper> _mockSessionMapper;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IRecreateSessionMapperService> _mockRecreateSessionMapperService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _csrfToken = _fixture.Freeze<StringValues>();
            _sessionId = _fixture.Freeze<string>();
            _patientId = _fixture.Freeze<string>();

            var userInfo = _fixture.Freeze<UserInfo>();
            userInfo.Birthdate = DateTime.Now.ToString(DateFormat, CultureInfo.InvariantCulture);
            userInfo.Im1ConnectionToken = _fixture.Create<EmisConnectionToken>().SerializeJson();
            _userProfile = new UserProfile(userInfo, _fixture.Create<string>());

            var emisUserSession = _fixture.Create<EmisUserSession>();
            emisUserSession.OdsCode = _userProfile.OdsCode;
            emisUserSession.NhsNumber = _userProfile.NhsNumber;
            emisUserSession.Name = _fixture.Create<string>();

            var citizenIdUserSession = new CitizenIdUserSession { AccessToken = _userProfile.AccessToken };
            var gpSessMgrCitizenIdUserSession = new GpSessionManagerCitizenIdUserSession { AccessToken = _userProfile.AccessToken };

            _gpSessMgrCitizenIdSessionResult = new GpSessionManagerCitizenIdSessionResult
            {
                Im1ConnectionToken = _userProfile.Im1ConnectionToken,
                NhsNumber = _userProfile.NhsNumber,
                OdsCode = _userProfile.OdsCode,
                Session = gpSessMgrCitizenIdUserSession
            };

            _userSession = new UserSession
            {
                CsrfToken = _csrfToken,
                GpUserSession = emisUserSession,
                CitizenIdUserSession = citizenIdUserSession,
                OrganDonationSessionId =  Guid.NewGuid(),
                Im1ConnectionToken = _userProfile.Im1ConnectionToken
            };

            _mockHttpContextAccessor = _fixture.Freeze<Mock<IHttpContextAccessor>>();
            _mockLogger = _fixture.Freeze<Mock<ILogger<GpSessionManager>>>();
            _mockSessionMapper = _fixture.Freeze<Mock<ISessionMapper>>();
            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockSessionService = _fixture.Freeze<Mock<ISessionService>>();
            _mockSessionCacheService = _fixture.Freeze<Mock<ISessionCacheService>>();
            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockRecreateSessionMapperService = _fixture.Freeze<Mock<IRecreateSessionMapperService>>();

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
                    _mockHttpContextAccessor.Object.HttpContext,
                    emisUserSession,
                    gpSessMgrCitizenIdUserSession,
                    _userProfile.Im1ConnectionToken))
                .Returns(_userSession)
                .Verifiable();

            _mockHttpContextAccessor.Object.HttpContext.Items[Constants.HttpContextItems.UserSession] = _userSession;

            _mockHttpContextAccessor
                .Setup(x=> x.HttpContext.Items[Constants.HttpContextItems.UserSession])
                .Returns(_userSession);

            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(It.IsAny<Supplier>()))
                .Returns(_mockGpSystem.Object);

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
            var result = await _gpSessionManager.CreateSession(_mockGpSystem.Object, _gpSessMgrCitizenIdSessionResult);

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
            var result = await _gpSessionManager.CreateSession(_mockGpSystem.Object, _gpSessMgrCitizenIdSessionResult);

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

        [TestMethod]
        public async Task RecreateSession_ReturnsSuccessfullyAndUpdatesSession()
        {
            //Arrange
            var originalGpUserSession = BuildTppUserSession();
            _userSession.GpUserSession = originalGpUserSession;

            var gpSessionRecreateResult = _fixture.Freeze<GpSessionRecreateResult.Success>();

            var updatedGpUserSession = _fixture.Freeze<TppUserSession>();
            var updatedUserSession = _userSession;
            updatedUserSession.GpUserSession = updatedGpUserSession;

            _mockSessionService
                .Setup(x => x.Recreate(
                    _userSession.Im1ConnectionToken, _userProfile.OdsCode, _userProfile.NhsNumber, _patientId))
                .ReturnsAsync(gpSessionRecreateResult);

            _mockGpSystem
                .Setup(x => x.GetRecreateSessionMapperService())
                .Returns(_mockRecreateSessionMapperService.Object);

            _mockRecreateSessionMapperService
                .Setup(x => x.Map(originalGpUserSession, "suid", _patientId))
                .Returns(updatedGpUserSession);

            //Act
            var recreateSessionResult = await _gpSessionManager.RecreateSession(_patientId);

            //Assert
            var successResult = recreateSessionResult.Should().BeOfType<RecreateSessionResult.Success>().Subject;
            successResult.UserSession.Should().BeEquivalentTo(updatedUserSession);

            _mockGpSystem.Verify(x => x.GetRecreateSessionMapperService(), Times.Once);
            _mockRecreateSessionMapperService.Verify(x =>
                x.Map(It.IsAny<GpUserSession>(), gpSessionRecreateResult.Suid, _patientId), Times.Once);
            _mockSessionCacheService.Verify(x => x.UpdateUserSession(updatedUserSession));
        }

        [TestMethod]
        public async Task RecreateSession_ReturnsFailure()
        {
            //Arrange
            _userSession.GpUserSession = BuildTppUserSession();

            _mockSessionService
                .Setup(x => x.Recreate(
                    _userSession.Im1ConnectionToken, _userProfile.OdsCode, _userProfile.NhsNumber, _patientId))
                .ReturnsAsync(new GpSessionRecreateResult.Failure());

            //Act
            var recreateSessionResult = await _gpSessionManager.RecreateSession(_patientId);

            //Assert
            recreateSessionResult.Should().BeOfType<RecreateSessionResult.Failure>();

            _mockGpSystem.Verify(x =>
                x.GetRecreateSessionMapperService(), Times.Never());

            _mockRecreateSessionMapperService.Verify(x =>
                x.Map(It.IsAny<GpUserSession>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());

            _mockSessionCacheService.Verify(x =>
                x.UpdateUserSession(It.IsAny<UserSession>()), Times.Never());
        }

        [TestMethod]
        public async Task CloseSession_ReturnsSuccess_AndSessionWasDeleted()
        {
            //Arrange
            var tppUserSession = BuildTppUserSession();

            _mockSessionService
                .Setup(x => x.Logoff(tppUserSession))
                .ReturnsAsync(new SessionLogoffResult.Success(tppUserSession));

            //Act
            var logoffSessionResult = await _gpSessionManager.CloseSession(tppUserSession);

            //Assert
            var result = logoffSessionResult.Should().BeOfType<CloseSessionResult.Success>();
            _mockLogger.VerifyLogger(LogLevel.Information, Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, Times.Never());
        }


        [DataRow(typeof(SessionLogoffResult.BadGateway), StatusCodes.Status502BadGateway)]
        [DataRow(typeof(SessionLogoffResult.Forbidden), StatusCodes.Status403Forbidden)]
        [DataTestMethod]
        public async Task CloseSession_ReturnsSuccess_WhenSessionServiceDoesNotReturnSuccess(Type resultType, int statusCode)
        {
            //Arrange
            var logoffResult = (SessionLogoffResult) Activator.CreateInstance(resultType);
            var tppUserSession = BuildTppUserSession();

            _mockSessionService
                .Setup(x => x.Logoff(tppUserSession))
                .ReturnsAsync(logoffResult);

            //Act
            var logoffSessionResult = await _gpSessionManager.CloseSession(tppUserSession);

            //Assert
            logoffSessionResult.Should().BeOfType<CloseSessionResult.Success>();
            _mockLogger.VerifyLogger(LogLevel.Information, Times.Never());
            _mockLogger.VerifyLogger(LogLevel.Error, $"Deleting the GP Supplier session failed with status code: '{statusCode}'", Times.Once());
        }

        [DataTestMethod]
        public async Task CloseSession_ReturnsFailure_WhenSessionServiceThrowsException()
        {
            //Arrange
            var tppUserSession = BuildTppUserSession();

            _mockSessionService
                .Setup(x => x.Logoff(tppUserSession))
                .Throws<Exception>();

            //Act
            var logoffSessionResult = await _gpSessionManager.CloseSession(tppUserSession);

            //Assert
            logoffSessionResult.Should().BeOfType<CloseSessionResult.Failure>();
            _mockLogger.VerifyLogger(LogLevel.Information, Times.Never());
            _mockLogger.VerifyLogger(LogLevel.Error, Times.Once());
        }


        [TestMethod]
        public async Task CloseAndDeleteSession_ReturnsSuccess_WhenSessionCacheWasDeleted()
        {
            //Arrange
            _userSession.GpUserSession = BuildTppUserSession();

            _mockSessionService
                .Setup(x => x.Logoff(_userSession.GpUserSession))
                .ReturnsAsync(new SessionLogoffResult.Success(_userSession.GpUserSession));

            _mockSessionCacheService
                .Setup(x => x.DeleteUserSession(_userSession.Key))
                .ReturnsAsync(true);

            //Act
            var logoffSessionResult = await _gpSessionManager.CloseAndDeleteSession(_userSession);

            //Assert
            logoffSessionResult.Should().BeOfType<CloseSessionResult.Success>();
            _mockLogger.VerifyLogger(LogLevel.Information, Times.Exactly(2));
            _mockLogger.VerifyLogger(LogLevel.Error, Times.Never());
        }

        [TestMethod]
        public async Task CloseAndDeleteSessionAndDelete_ReturnsSuccess_WhenSessionCachesWasNotDeleted()
        {
            //Arrange
            _userSession.GpUserSession = BuildTppUserSession();

            _mockSessionService
                .Setup(x => x.Logoff(_userSession.GpUserSession))
                .ReturnsAsync(new SessionLogoffResult.Success(_userSession.GpUserSession));

            _mockSessionCacheService
                .Setup(x => x.DeleteUserSession(_userSession.Key))
                .ReturnsAsync(false);

            //Act
            var logoffSessionResult = await _gpSessionManager.CloseAndDeleteSession(_userSession);

            //Assert
            logoffSessionResult.Should().BeOfType<CloseSessionResult.Success>();
            _mockLogger.VerifyLogger(LogLevel.Information, Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, "No active session was found", Times.Once());
        }


        [TestMethod]
        public async Task CloseAndDeleteSession_ReturnsFailure_WhenSessionCachesServiceThrowsException()
        {
            //Arrange
            _userSession.GpUserSession = BuildTppUserSession();

            _mockSessionService
                .Setup(x => x.Logoff(_userSession.GpUserSession))
                .ReturnsAsync(new SessionLogoffResult.Success(_userSession.GpUserSession));

            _mockSessionCacheService
                .Setup(x => x.DeleteUserSession(_userSession.Key))
                .Throws<Exception>();

            //Act
            var logoffSessionResult = await _gpSessionManager.CloseAndDeleteSession(_userSession);

            //Assert
            logoffSessionResult.Should().BeOfType<CloseSessionResult.Failure>();
            _mockLogger.VerifyLogger(LogLevel.Information, Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, Times.Once());
        }

        [TestMethod]
        public async Task CloseAndDeleteSession_ReturnsFailure_WhenSessionServiceThrowsException()
        {
            //Arrange
            _userSession.GpUserSession = BuildTppUserSession();

            _mockSessionService
                .Setup(x => x.Logoff(_userSession.GpUserSession))
                .Throws<Exception>();

            //Act
            var logoffSessionResult = await _gpSessionManager.CloseAndDeleteSession(_userSession);

            //Assert
            logoffSessionResult.Should().BeOfType<CloseSessionResult.Failure>();
            _mockLogger.VerifyLogger(LogLevel.Information, Times.Never());
            _mockLogger.VerifyLogger(LogLevel.Error, Times.Once());
            _mockSessionCacheService.VerifyNoOtherCalls();
        }

        private TppUserSession BuildTppUserSession()
        {
            var tppUserSession = _fixture.Create<TppUserSession>();
            tppUserSession.OdsCode = _userProfile.OdsCode;
            tppUserSession.NhsNumber = _userProfile.NhsNumber;
            tppUserSession.PatientId = _patientId;
            return tppUserSession;
        }
    }
}