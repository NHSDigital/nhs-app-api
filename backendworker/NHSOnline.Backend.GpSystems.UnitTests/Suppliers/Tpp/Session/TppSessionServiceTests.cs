using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Session
{
    [TestClass]
    public class TppSessionServiceTests
    {
        private IFixture _fixture;
        private Mock<ITppClient> _mockTppClient;
        private Mock<IOptions<ConfigurationSettings>> _mockConfigurationSettings;
        private Mock<ITppSessionMapper> _mockTppSessionMapper;
        private TppSessionService _systemUnderTest;
        private Authenticate _actual;
        private GpUserSession _tppUserSession;
        private int _sessionTimeoutMinutes;
        private string _nhsNumber;
        private const string ResponseSuidHeader = "suid";
        private TppClient.TppApiObjectResponse<AuthenticateReply> _authenticatePostResult;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _tppUserSession = _fixture.Create<TppUserSession>();
            
            _actual = null;
            _authenticatePostResult = null;
            
            _mockTppClient = _fixture.Freeze<Mock<ITppClient>>();
            _mockTppClient
                .Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => _authenticatePostResult)
                .Callback<Authenticate>(x =>
                {
                    _actual = x;
                });
            
            _mockTppClient
                .Setup(x => x.PatientSelectedPost(It.IsAny<TppUserSession>()))
                .ReturnsAsync(() => null);

            _nhsNumber = _fixture.Create<string>();
            _sessionTimeoutMinutes = _fixture.Create<int>();
            
            _mockConfigurationSettings = _fixture.Freeze<Mock<IOptions<ConfigurationSettings>>>();
            _mockConfigurationSettings
                .Setup(x => x.Value)
                .Returns(new ConfigurationSettings()
                {
                    DefaultSessionExpiryMinutes = _sessionTimeoutMinutes
                });

            _mockTppSessionMapper = _fixture.Freeze<Mock<ITppSessionMapper>>();
        }

        [TestMethod]
        public async Task
            Create_TppClientThrowsHttpRequestExceptionFromSessionCreate_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            // Tpp client throws HttpRequestException
            _mockTppClient
                .Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>()))
                .Throws<HttpRequestException>()
                .Verifiable();

            _systemUnderTest = _fixture.Create<TppSessionService>();

            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), "bar", _nhsNumber);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<GpSessionCreateResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Create_WhenCalledWithIm1ConnectionToken_DeserializesFromJsonAndPassesItToTheTppClient()        
        {
            // Arrange
            const string accountId = "boo";
            const string passphrase = "hoo";
            var tppConnectionToken = CreateConnectionTokenJson(accountId, passphrase);
            _authenticatePostResult = new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.Accepted)
            {
                Body = new AuthenticateReply()
            };

            _systemUnderTest = _fixture.Create<TppSessionService>();
        
            // Act
            await _systemUnderTest.Create(tppConnectionToken, "bar", _nhsNumber);
        
            // Assert
            _actual.AccountId.Should().Be(accountId);
            _actual.Passphrase.Should().Be(passphrase);
        }
    
        [TestMethod]
        public async Task Create_WhenCalledWithOdsCode_PassesItToTheTppClientAsUnitId()
        {
            // Arrange
            const string expected = "bar";
            _systemUnderTest = _fixture.Create<TppSessionService>();
            _authenticatePostResult = new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.Accepted)
            {
                Body = new AuthenticateReply()
            };
            
            // Act
            await _systemUnderTest.Create(CreateConnectionTokenJson(), expected, _nhsNumber);

            //Assert
            _actual.UnitId.Should().Be(expected);
        }

        [TestMethod]
        public async Task Create_WhenCalledSuccessfully_SetsTheName()
        {
            // Arrange
            const string expectedName = "Montel";
            var odsCode = "1234";
            var reply = CreateReply(expectedName);
        
            _mockTppClient
                .Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);

            _mockTppSessionMapper
                .Setup(x => x.Map(reply, odsCode, _nhsNumber))
                .Returns(Option.Some(CreateUserSession(odsCode, _nhsNumber)));
        
            _systemUnderTest = _fixture.Create<TppSessionService>();
        
            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), odsCode, _nhsNumber);
        
            // Assert
            var created = (GpSessionCreateResult.SuccessfullyCreated) result;
            created.Name.Should().Be(expectedName);
        }
    
        [TestMethod]
        public async Task Create_WhenCalledSuccessfully_SetsTheUserSessionAsATppUserSession()
        {
            // Arrange
            const string expectedSessionId = "ff5246bc-ef03-458a-a1ff-4b6e80268671";
            const string expectedOnlineUserId = "abcde";
            const string expectedPatientId = "12345";
            const string odsCode = "1234";
            var reply = CreateReply(
                suid: expectedSessionId, 
                onlineUserId: expectedOnlineUserId, 
                patientId: expectedPatientId);

            var userSession = CreateUserSession(odsCode, expectedSessionId,
                expectedOnlineUserId, expectedPatientId, _nhsNumber);
        
            _mockTppClient.Setup(x => x
                .AuthenticatePost(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);
            
            _mockTppSessionMapper
                .Setup(x => x.Map(reply, odsCode, _nhsNumber))
                .Returns(Option.Some(userSession));
        
            _systemUnderTest = _fixture.Create<TppSessionService>();
        
            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), odsCode, _nhsNumber);
        
            // Assert
            var createdResult = result.Should().BeAssignableTo<GpSessionCreateResult.SuccessfullyCreated>().Subject;
            var tppUserSession = createdResult.UserSession.Should().BeAssignableTo<TppUserSession>().Subject;
            
            tppUserSession.Suid.Should().Be(expectedSessionId);
            tppUserSession.OnlineUserId.Should().Be(expectedOnlineUserId);
            tppUserSession.PatientId.Should().Be(expectedPatientId);
            tppUserSession.OdsCode.Should().Be(odsCode);
            tppUserSession.NhsNumber.Should().Be(_nhsNumber);
        }

        [TestMethod]
        public async Task Create_WhenCalledWithErrorResponse_ReturnsSupplierSystemUnavailable()
        {
            // Arrange 
            var reply = CreateReply();
            reply.ErrorResponse = new Error();

            _mockTppClient.Setup(x => x
                .AuthenticatePost(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);

            _systemUnderTest = _fixture.Create<TppSessionService>();

            // Act 
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), "1234", _nhsNumber);
            
            // Assert 
            result.Should().BeAssignableTo<GpSessionCreateResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Create_WhenCalledWithHttpError_ReturnsSupplierSystemUnavailable()
        {
            // Arrange 
            var reply = CreateReply();
            reply.StatusCode = HttpStatusCode.BadGateway;

            _mockTppClient.Setup(x => x
                .AuthenticatePost(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);

            _systemUnderTest = _fixture.Create<TppSessionService>();

            // Act 
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), "1234", _nhsNumber);

            // Assert 
            result.Should().BeAssignableTo<GpSessionCreateResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Create_WhenCalledWithMappingError_ReturnsSupplierSystemBadResponse()
        {
            // Arrange 
            var odsCode = "1234";
            var reply = CreateReply();
            reply.StatusCode = HttpStatusCode.BadGateway;

            _mockTppClient.Setup(x => x
                    .AuthenticatePost(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);
            
            _mockTppSessionMapper
                .Setup(x => x.Map(reply, odsCode, _nhsNumber))
                .Returns(Option.None<TppUserSession>());

            _systemUnderTest = _fixture.Create<TppSessionService>();

            // Act 
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), odsCode, _nhsNumber);

            // Assert 
            result.Should().BeAssignableTo<GpSessionCreateResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Logoff_WhenCalledWithAValidUserSession_ReturnsSuccessfullyDeleted()
        {
            // Arrange
            var reply = LogoffReply();
        
            _mockTppClient.Setup(x => x
                .LogoffPost(It.IsAny<TppUserSession>()))
                .ReturnsAsync(() => reply);
        
            _systemUnderTest = _fixture.Create<TppSessionService>();
        
            // Act
            var result = await _systemUnderTest.Logoff(_tppUserSession);
        
            // Assert
            result.Should().BeOfType<SessionLogoffResult.SuccessfullyDeleted>();
        }

        [TestMethod]
        public async Task Logoff_WhenCalledWithErrorResponse_ReturnsSupplierSystemUnavailable()
        {
            // Arrange 
            var reply = LogoffReply();
            reply.ErrorResponse = new Error();

            _mockTppClient
                .Setup(x => x.LogoffPost(It.IsAny<TppUserSession>()))
                .ReturnsAsync(() => reply);

            _systemUnderTest = _fixture.Create<TppSessionService>();

            // Act 
            var result = await _systemUnderTest.Logoff(_tppUserSession);
            
            // Assert 
            result.Should().BeAssignableTo<SessionLogoffResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Logoff_WhenCalledWithHttpError_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var reply = LogoffReply();
            reply.StatusCode = HttpStatusCode.BadGateway;

            _mockTppClient
                .Setup(x => x.LogoffPost(It.IsAny<TppUserSession>()))
                .Throws<HttpRequestException>()
                .Verifiable();

            _systemUnderTest = _fixture.Create<TppSessionService>();

            // Act
            var result = await _systemUnderTest.Logoff(_tppUserSession);

            // Assert
            result.Should().BeAssignableTo<SessionLogoffResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Logoff_WhenCalledWithInvalidSession_ReturnsNotAuthenticated()
        {
            // Arrange
            _mockTppClient
                .Setup(x => x.LogoffPost(It.IsAny<TppUserSession>()))
                .ThrowsAsync(new UnauthorisedGpSystemHttpRequestException());

            _systemUnderTest = _fixture.Create<TppSessionService>();

            // Act
            var result = await _systemUnderTest.Logoff(_tppUserSession);

            // Assert
            result.Should().BeAssignableTo<SessionLogoffResult.NotAuthenticated>();
        }
        
        private static string CreateConnectionTokenJson(string accountId = "", string passphrase = "") =>
            $"{{ \"accountId\": \"{accountId}\", \"passphrase\": \"{passphrase}\" }}";

        private TppClient.TppApiObjectResponse<AuthenticateReply> CreateReply(string name = "Joanie", string suid = "dimsum",
            string onlineUserId = "123", string patientId = "123")
        {
            var response = new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
            {
                Body = new AuthenticateReply
                {
                    OnlineUserId = onlineUserId,
                    PatientId = patientId,
                    User = new User
                    {
                        Person = new Person
                        {
                            PatientId = patientId,
                            PersonName = new PersonName { Name = name }
                        }
                    }
                },
                Headers = new Dictionary<string, string>
                {
                    { ResponseSuidHeader, suid }
                }
            };

            return response;
        }

        private TppUserSession CreateUserSession( string odsCode, string sessionId = "dimsum", string onlineUserId = "123", 
            string patientId = "123", string nhsNumber = "123456789")
        {
            return new TppUserSession()
            {
                Suid = sessionId,
                OnlineUserId = onlineUserId,
                PatientId = patientId,
                OdsCode = odsCode,
                NhsNumber = nhsNumber
            };
        }

        private TppClient.TppApiObjectResponse<LogoffReply> LogoffReply()
        {
            var response = new TppClient.TppApiObjectResponse<LogoffReply>(HttpStatusCode.OK)
            {
                Body = new LogoffReply
                {
                    Uuid = Guid.NewGuid()
                },
                Headers = new Dictionary<string, string>
                {
                    { ResponseSuidHeader, "dimsum" }
                }
            };

            return response;
        }  
    }
}
