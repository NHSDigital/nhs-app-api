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
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
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
        private P9UserSession _userSession;
        private string _sessionId;
        private string _patientId;
        private UserProfile _userProfile;

        private Mock<ISessionCacheService> _mockSessionCacheService;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<ILogger<GpSessionManager>> _mockLogger;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IRecreateSessionMapperService> _mockRecreateSessionMapperService;
        private Mock<IUserSessionService> _mockUserSessionService;

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
            _userProfile = new UserProfile(userInfo, _fixture.Create<string>(), _fixture.Create<string>());

            var emisUserSession = _fixture.Create<EmisUserSession>();
            emisUserSession.OdsCode = _userProfile.OdsCode;
            emisUserSession.NhsNumber = _userProfile.NhsNumber;
            emisUserSession.Name = _fixture.Create<string>();

            var citizenIdUserSession = new CitizenIdUserSession { AccessToken = _userProfile.AccessToken };

            _userSession = new P9UserSession(_csrfToken, citizenIdUserSession, emisUserSession, _userProfile.Im1ConnectionToken);

            _mockLogger = _fixture.Freeze<Mock<ILogger<GpSessionManager>>>();
            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockSessionService = _fixture.Freeze<Mock<ISessionService>>();
            _mockSessionCacheService = _fixture.Freeze<Mock<ISessionCacheService>>();
            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockRecreateSessionMapperService = _fixture.Freeze<Mock<IRecreateSessionMapperService>>();

            _mockSessionCacheService
                .Setup(x => x.GetUserSession(_sessionId))
                .Returns(Task.FromResult(Option.Some<UserSession>(_userSession)))
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

            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(It.IsAny<Supplier>()))
                .Returns(_mockGpSystem.Object);

            _mockUserSessionService = _fixture.Freeze<Mock<IUserSessionService>>();
            _mockUserSessionService
                .Setup(x => x.GetRequiredUserSession<P9UserSession>(It.IsAny<string>()))
                .Returns(_userSession);

            //create the class under test
            _gpSessionManager = _fixture.Create<GpSessionManager>();
        }

       [TestMethod]
        public async Task CreateSession_ReturnsCreateSessionResult_Failure()
        {
            // Arrange
            _mockSessionService
                .Setup(x => x.Create(_userProfile.Im1ConnectionToken, _userProfile.OdsCode, _userProfile.NhsNumber))
                .ReturnsAsync(new GpSessionCreateResult.BadGateway(""))
                .Verifiable();
            var args = CreateGpSessionCreateArgs();

            // Act
            var result = await _gpSessionManager.CreateSession(args.Object);

            // Assert
            result.Should().BeOfType<GpSessionCreateResult.BadGateway>();

            _mockSessionCacheService.Verify(x => x.CreateUserSession(It.IsAny<P9UserSession>()), Times.Never());
            _mockSessionService.Verify();
            _mockLogger.VerifyLogger(LogLevel.Debug,$"Fetched Session Id: sessionId", Times.Never());
        }

        [TestMethod]
        public async Task CreateSession_ReturnsCreateSessionResult_Success()
        {
            // Arrange
            var args = CreateGpSessionCreateArgs();

            // Act
            var result = await _gpSessionManager.CreateSession(args.Object);

            // Assert
            var successResult = result.Should().BeOfType<GpSessionCreateResult.Success>().Subject;
            successResult.UserSession.Should().BeEquivalentTo(_userSession.GpUserSession);

            _mockSessionService.Verify();
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
            // Arrange
            var originalGpUserSession = BuildTppUserSession();
            _userSession.GpUserSession = originalGpUserSession;

            var gpSessionRecreateResult = _fixture.Freeze<GpSessionRecreateResult.Success>();

            var updatedGpUserSession = new TppUserSession
            {
                OdsCode = originalGpUserSession.OdsCode,
                NhsNumber = originalGpUserSession.NhsNumber,
                PatientId = originalGpUserSession.PatientId,
            };

            _mockSessionService
                .Setup(x => x.Recreate(
                    _userSession.Im1ConnectionToken, _userProfile.OdsCode, _userProfile.NhsNumber, _patientId))
                .ReturnsAsync(gpSessionRecreateResult);

            _mockGpSystem
                .Setup(x => x.GetRecreateSessionMapperService())
                .Returns(_mockRecreateSessionMapperService.Object);

            _mockRecreateSessionMapperService
                .Setup(x => x.Map(originalGpUserSession, gpSessionRecreateResult.Suid, _patientId))
                .Returns(updatedGpUserSession);

            // Act
            var recreateSessionResult = await _gpSessionManager.RecreateSession(_patientId);

            // Assert
            var successResult = recreateSessionResult.Should().BeOfType<RecreateSessionResult.Success>().Subject;
            successResult.UserSession.GpUserSession.Should().BeEquivalentTo(updatedGpUserSession);
            _userSession.GpUserSession.Should().Be(updatedGpUserSession);
            _mockGpSystem.Verify(x => x.GetRecreateSessionMapperService(), Times.Once);
            _mockRecreateSessionMapperService.Verify(x =>
                x.Map(It.IsAny<GpUserSession>(), gpSessionRecreateResult.Suid, _patientId), Times.Once);
            _mockSessionCacheService.Verify(x => x.UpdateUserSession(_userSession));
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
                x.UpdateUserSession(It.IsAny<P9UserSession>()), Times.Never());
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
            logoffSessionResult.Should().BeOfType<CloseSessionResult.Success>();
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

        private Mock<IGpSessionCreateArgs> CreateGpSessionCreateArgs()
        {
            var args = new Mock<IGpSessionCreateArgs>();
            args.Setup(x => x.GpSystem).Returns(_mockGpSystem.Object);
            args.Setup(x => x.Im1ConnectionToken).Returns(_userProfile.Im1ConnectionToken);
            args.Setup(x => x.OdsCode).Returns(_userProfile.OdsCode);
            args.Setup(x => x.NhsNumber).Returns(_userProfile.NhsNumber);
            return args;
        }
    }
}