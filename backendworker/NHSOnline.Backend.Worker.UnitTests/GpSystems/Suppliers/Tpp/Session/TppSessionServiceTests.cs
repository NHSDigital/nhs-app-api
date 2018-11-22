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
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Session
{
    [TestClass]
    public class TppSessionServiceTests
    {
        private IFixture _fixture;
        private Mock<ITppClient> _mockTppClient;
        private Mock<IOptions<ConfigurationSettings>> _mockConfigurationSettings;
        private TppSessionService _systemUnderTest;
        private Authenticate _actual;
        private TppUserSession _userSession;
        private int _sessionTimeoutMinutes;
        private string _nhsNumber;
        private const string ResponseSuidHeader = "suid";
        private TppClient.TppApiObjectResponse<AuthenticateReply> _authenticatePostResult;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
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

            _nhsNumber = _fixture.Create<string>();
            _sessionTimeoutMinutes = _fixture.Create<int>();
            _userSession = _fixture.Create<TppUserSession>();

            _mockConfigurationSettings = _fixture.Freeze<Mock<IOptions<ConfigurationSettings>>>();
            _mockConfigurationSettings
                .Setup(x => x.Value)
                .Returns(new ConfigurationSettings()
                {
                    DefaultSessionExpiryMinutes = _sessionTimeoutMinutes
                });
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
            result.Should().BeAssignableTo<SessionCreateResult.SupplierSystemUnavailable>();
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
            var reply = CreateReply(expectedName);
        
            _mockTppClient
                .Setup(x => x.AuthenticatePost(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);
        
            _systemUnderTest = _fixture.Create<TppSessionService>();
        
            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), "1234", _nhsNumber);
        
            // Assert
            var created = (SessionCreateResult.SuccessfullyCreated) result;
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
            const string expectedNhsNumber = "65786857978978";
            var reply = CreateReply(
                suid: expectedSessionId, 
                onlineUserId: expectedOnlineUserId, 
                patiendId: expectedPatientId,
                nhsNumber: expectedNhsNumber);
        
            _mockTppClient.Setup(x => x
                .AuthenticatePost(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);
        
            _systemUnderTest = _fixture.Create<TppSessionService>();
        
            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), odsCode, _nhsNumber);
        
            // Assert
            var createdResult = result.Should().BeAssignableTo<SessionCreateResult.SuccessfullyCreated>().Subject;
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
            result.Should().BeAssignableTo<SessionCreateResult.SupplierSystemUnavailable>();
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
            result.Should().BeAssignableTo<SessionCreateResult.SupplierSystemUnavailable>();
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
            var result = await _systemUnderTest.Logoff(_userSession);
        
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
            var result = await _systemUnderTest.Logoff(_userSession);
            
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
            var result = await _systemUnderTest.Logoff(_userSession);

            // Assert
            result.Should().BeAssignableTo<SessionLogoffResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Logoff_WhenCalledWithInvalidSession_ReturnsNotAuthenticated()
        {
            // Arrange
            var notAuthenticatedResponse = _fixture.Build<Error>()
                .With(x => x.ErrorCode, TppApiErrorCodes.NotAuthenticated)
                .Create();

            _mockTppClient
                .Setup(x => x.LogoffPost(It.IsAny<TppUserSession>()))
                .Returns(
                    Task.FromResult(
                        new TppClient.TppApiObjectResponse<LogoffReply>(HttpStatusCode.OK)
                        {
                            ErrorResponse = notAuthenticatedResponse
                        }
                    )
                );

            _systemUnderTest = _fixture.Create<TppSessionService>();

            // Act
            var result = await _systemUnderTest.Logoff(_userSession);

            // Assert
            result.Should().BeAssignableTo<SessionLogoffResult.NotAuthenticated>();
        }
        
        private static string CreateConnectionTokenJson(string accountId = "", string passphrase = "") =>
            $"{{ \"accountId\": \"{accountId}\", \"passphrase\": \"{passphrase}\" }}";

        private TppClient.TppApiObjectResponse<AuthenticateReply> CreateReply(string name = "Joanie", string suid = "dimsum",
            string onlineUserId = "123", string patiendId = "234", string nhsNumber = "123456789")
        {
            var response = new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
            {
                Body = new AuthenticateReply
                {
                    OnlineUserId = onlineUserId,
                    PatientId = patiendId,
                    User = new User
                    {
                        Person = new Person
                        {
                            PersonName = new PersonName { Name = name },
                            NationalId = new NationalId { Type = "NHS", Value = nhsNumber }
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
