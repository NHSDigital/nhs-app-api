
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

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp
{
    [TestClass]
    public class TppSessionServiceTests
    {
        private IFixture _fixture;
        private Mock<ITppClient> _mockTppClient;
        private Mock<IOptions<ConfigurationSettings>> _mockConfigurationSettings;
        private TppSessionService _systemUnderTest;
        private Authenticate _actual;
        private ConfigurationSettings _configurationSettings;
        private const string ResponseSuidHeader = "suid";

        [TestInitialize]
        public void TestInitialize()
        {
            _configurationSettings = new ConfigurationSettings
            {
                DefaultSessionExpiryMinutes = 20
            };
        
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockTppClient = _fixture.Freeze<Mock<ITppClient>>();
            _mockTppClient.Setup(x => x
                    .AuthenticatePost(It.IsAny<Authenticate>()))
                .Callback<Authenticate>(x => _actual = x);

            _mockConfigurationSettings = _fixture.Freeze<Mock<IOptions<ConfigurationSettings>>>();
            _mockConfigurationSettings.SetupGet(x => x.Value).Returns(_configurationSettings);
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
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), "bar");

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<SessionCreateResult.SupplierSystemUnavailable>();
        }


        [TestMethod]
        public void Create_WhenCalledWithIm1ConnectionToken_DeserializesFromJsonAndPassesItToTheTppClient()
        {
            // Arrange
            const string accountId = "boo";
            const string passphrase = "hoo";
            var tppConnectionToken = CreateConnectionTokenJson(accountId, passphrase);
            _systemUnderTest = _fixture.Create<TppSessionService>();
        
            // Act
            _systemUnderTest.Create(tppConnectionToken, "bar");
        
            // Assert
            _actual.AccountId.Should().Be(accountId);
            _actual.Passphrase.Should().Be(passphrase);
        }
    
        [TestMethod]
        public void Create_WhenCalledWithOdsCode_PassesItToTheTppClientAsUnitId()
        {
            // Arrange
            const string expected = "bar";
            _systemUnderTest = _fixture.Create<TppSessionService>();
        
            // Act
            _systemUnderTest.Create(CreateConnectionTokenJson(), expected);

            //Assert
            _actual.UnitId.Should().Be(expected);
        }

        [TestMethod]
        public async Task Create_WhenCalledSuccessfully_SetsTheName()
        {
            // Arrange
            const string expectedName = "Montel";
            var reply = CreateReply(expectedName);
        
            _mockTppClient.Setup(x => x
                    .AuthenticatePost(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);
        
            _systemUnderTest = _fixture.Create<TppSessionService>();
        
            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), "1234");
        
            // Assert
            var created = (SessionCreateResult.SuccessfullyCreated) result;
            created.Name.Should().Be(expectedName);
        }
    
        [TestMethod]
        public async Task Create_WhenCalledSuccessfully_SetsTheSessionTimeout()
        {
            // Arrange
            const int expectedTimeout = 20;
            var reply = CreateReply();
        
            _mockTppClient.Setup(x => x
                    .AuthenticatePost(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);
        
            _systemUnderTest = _fixture.Create<TppSessionService>();
        
            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), "1234");
        
            // Assert
            var created = (SessionCreateResult.SuccessfullyCreated) result;
            created.SessionTimeout.Should().Be(expectedTimeout);
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
                patiendId: expectedPatientId);
        
            _mockTppClient.Setup(x => x
                    .AuthenticatePost(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);
        
            _systemUnderTest = _fixture.Create<TppSessionService>();
        
            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), odsCode);
        
            // Assert
            var created = (SessionCreateResult.SuccessfullyCreated) result;
            created.UserSession.Should().BeOfType<TppUserSession>();
            ((TppUserSession) created.UserSession).Suid.Should().Be(expectedSessionId);
            ((TppUserSession)created.UserSession).OnlineUserId.Should().Be(expectedOnlineUserId);
            ((TppUserSession)created.UserSession).PatientId.Should().Be(expectedPatientId);
            ((TppUserSession)created.UserSession).UnitId.Should().Be(odsCode);
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
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), "1234");
            
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
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), "1234");

            // Assert 
            result.Should().BeAssignableTo<SessionCreateResult.SupplierSystemUnavailable>();
        }

        private string CreateConnectionTokenJson(string accountId = "", string passphrase = "") =>
            $"{{ \"accountId\": \"{accountId}\", \"passphrase\": \"{passphrase}\" }}";

        private TppClient.TppApiObjectResponse<AuthenticateReply> CreateReply(string name = "Joanie", string suid = "dimsum",
            string onlineUserId = "123", string patiendId = "234")
        {
            var response = new TppClient.TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK);

            response.Body = new AuthenticateReply
            {
                OnlineUserId = onlineUserId,
                PatientId = patiendId,
                User = new User
                {
                    Person = new Person
                    {
                        PersonName = new PersonName { Name = name }
                    }
                }
            };

            response.Headers = new Dictionary<string, string>
            {
                { ResponseSuidHeader, suid }
            };

            return response;
        }   
    }
}