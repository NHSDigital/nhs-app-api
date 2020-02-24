using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using UnitTestHelper;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Session
{
    [TestClass]
    public class TppSessionServiceTests
    {
        private const string ResponseSuidHeader = "suid";

        private Mock<ITppClient> _mockTppClient;
        private Mock<ITppClientRequest<Authenticate, AuthenticateReply>> _mockAuthenticate;
        private Mock<ITppClientRequest<TppUserSession, LogoffReply>> _mockLogOff;

        private Mock<IConfigurationSettings> _mockConfigurationSettings;
        private Mock<ITppSessionMapper> _mockTppSessionMapper;
        private Mock<ITppLogMessagingService> _mockTppLogMessagingService;

        private Authenticate _actual;
        private GpUserSession _tppUserSession;
        private int _sessionTimeoutMinutes;
        private string _nhsNumber;
        private TppApiObjectResponse<AuthenticateReply> _authenticatePostResult;

        private ServiceProvider _serviceProvider;

        private TppSessionService _systemUnderTest;

        private Mock<ILogger<TppSessionService>> TppSessionServiceLogger => _serviceProvider.MockLogger<TppSessionService>();

        [TestInitialize]
        public void TestInitialize()
        {
            var services = new ServiceCollection();
            services.RegisterTppSessionServices();

            _tppUserSession = new TppUserSession();
            _actual = null;
            _authenticatePostResult = null;

            _mockAuthenticate = services.AddMock<ITppClientRequest<Authenticate, AuthenticateReply>>();
            _mockAuthenticate
                .Setup(x => x.Post(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => _authenticatePostResult)
                .Callback<Authenticate>(x =>
                {
                    _actual = x;
                });

            _mockTppClient = services.AddMock<ITppClient>();
            _mockTppClient
                .Setup(x => x.PatientSelectedPost(It.IsAny<TppUserSession>()))
                .ReturnsAsync(() => null);

            _mockLogOff = services.AddMock<ITppClientRequest<TppUserSession, LogoffReply>>();

            _nhsNumber = "123456";
            _sessionTimeoutMinutes = 10;

            _mockConfigurationSettings = services.AddMock<IConfigurationSettings>();
            _mockConfigurationSettings.SetupGet(x => x.DefaultHttpTimeoutSeconds).Returns(_sessionTimeoutMinutes);

            _mockTppSessionMapper = services.AddMock<ITppSessionMapper>();
            _mockTppLogMessagingService = services.AddMock<ITppLogMessagingService>();

            services.AddMockLoggers();

            _serviceProvider = services.BuildServiceProvider();
            _systemUnderTest = _serviceProvider.GetRequiredService<TppSessionService>();
        }

        [TestMethod]
        public async Task
            Create_TppClientThrowsHttpRequestExceptionFromSessionCreate_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            // Tpp client throws HttpRequestException
            _mockAuthenticate
                .Setup(x => x.Post(It.IsAny<Authenticate>()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), "bar", _nhsNumber);

            // Assert
            _mockAuthenticate.Verify();
            result.Should().BeAssignableTo<GpSessionCreateResult.BadGateway>();
        }

        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 0)]
        [DataRow(1, 1)]
        [DataRow(3, 1)]
        [DataRow(3, 2)]
        [DataRow(3, 3)]
        public async Task Create_LogsCorrectNumberOfLinkedAccounts(int numberOfLinkedAccounts, int ofWhichHaveDifferentAddress)
        {
            // Arrange
            const string patientId = "abc";
            const string accountId = "boo";
            const string passphrase = "hoo";
            const string gpPracticeName = "gp practice 1";
            const string gpAddress = "1 street name, town name";
            var tppConnectionToken = CreateConnectionTokenJson(accountId, passphrase);
            _authenticatePostResult = new TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.Accepted)
            {
                Body = new AuthenticateReply
                {
                    User = new User
                    {
                        Person = new Person
                        {
                            PatientId = patientId,
                        },
                    },
                    Registration = new Registration
                    {
                        PatientAccess = new List<PatientAccess>
                        {
                            new PatientAccess
                            {
                                PatientId = patientId,
                                SiteDetails = new SiteDetails
                                {
                                    UnitName = gpPracticeName,
                                    Address = new TppAddress
                                    {
                                        Address = gpAddress,
                                    },
                                },
                            },
                        },
                    },
                },
            };

            int numberToSetupWithSameGpPractice = numberOfLinkedAccounts - ofWhichHaveDifferentAddress;

            for (var i = 0; i < numberOfLinkedAccounts; i++)
            {
                var linkedAccount = new PatientAccess { SiteDetails = new SiteDetails { Address = new TppAddress() } };
                if (i < numberToSetupWithSameGpPractice)
                {
                    linkedAccount.SiteDetails.UnitName = gpPracticeName;
                    linkedAccount.SiteDetails.Address.Address = gpAddress;
                }
                _authenticatePostResult.Body.Registration.PatientAccess.Add(linkedAccount);
            }

            // Act
            await _systemUnderTest.Create(tppConnectionToken, "bar", _nhsNumber);

            // Assert
            TppSessionServiceLogger.VerifyLogger(LogLevel.Information,
                $"User has linked_accounts={numberOfLinkedAccounts}, with different_ods_codes_to_user={ofWhichHaveDifferentAddress}", Times.Once());
        }

        [TestMethod]
        public async Task Create_WhenCalledWithIm1ConnectionToken_DeserializesFromJsonAndPassesItToTheTppClient()
        {
            // Arrange
            const string accountId = "boo";
            const string passphrase = "hoo";
            var tppConnectionToken = CreateConnectionTokenJson(accountId, passphrase);
            _authenticatePostResult = new TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.Accepted)
            {
                Body = new AuthenticateReply()
            };

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

            _authenticatePostResult = new TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.Accepted)
            {
                Body = new AuthenticateReply()
            };

            // Act
            await _systemUnderTest.Create(CreateConnectionTokenJson(), expected, _nhsNumber);

            // Assert
            _actual.UnitId.Should().Be(expected);
        }

        [TestMethod]
        public async Task Create_WhenResponseHasAtLeastOneLinkedAccount_CallsAuthenticate()
        {
            // Arrange
            const string expectedName = "Montel";
            const string odsCode = "1234";
            var proxyPatientIds = new List<string> { "282", "383" };
            var reply = CreateReply(expectedName, proxyPatientIds: proxyPatientIds);

            _authenticatePostResult = reply;

            _mockTppSessionMapper
                .Setup(x => x.Map(reply, odsCode, _nhsNumber))
                .Returns(Option.Some(CreateUserSession(expectedName, odsCode, nhsNumber: _nhsNumber, proxyPatientIds: proxyPatientIds)));
            
            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), odsCode, _nhsNumber);

            // Assert
            var created = (GpSessionCreateResult.Success) result;
            created.UserSession.Name.Should().Be(expectedName);
            _mockTppClient.Verify(x => x.PatientSelectedPost(It.IsAny<TppUserSession>()), Times.Once);
        }
        
        [TestMethod]
        public async Task Create_WhenResponseHasNoLinkedAccounts_DoesNotCallAuthenticate()
        {
            // Arrange
            const string expectedName = "Montel";
            const string odsCode = "1234";
            var reply = CreateReply(expectedName, proxyPatientIds: new List<string>());

            _authenticatePostResult = reply;

            _mockTppSessionMapper
                .Setup(x => x.Map(reply, odsCode, _nhsNumber))
                .Returns(Option.Some(CreateUserSession(expectedName, odsCode, _nhsNumber, proxyPatientIds: new List<string>())));
            
            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), odsCode, _nhsNumber);

            // Assert
            var created = (GpSessionCreateResult.Success) result;
            created.UserSession.Name.Should().Be(expectedName);
            _mockTppClient.Verify(x => x.PatientSelectedPost(It.IsAny<TppUserSession>()), Times.Never);
        }
        
        [TestMethod]
        public async Task Create_WhenCalledSuccessfully_SetsTheName()
        {
            // Arrange
            const string expectedName = "Montel";
            const string odsCode = "1234";
            var reply = CreateReply(expectedName);

            _mockAuthenticate
                .Setup(x => x.Post(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);

            _mockTppSessionMapper
                .Setup(x => x.Map(reply, odsCode, _nhsNumber))
                .Returns(Option.Some(CreateUserSession(expectedName, odsCode, _nhsNumber)));

            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), odsCode, _nhsNumber);

            // Assert
            var created = (GpSessionCreateResult.Success) result;
            created.UserSession.Name.Should().Be(expectedName);
        }

        [TestMethod]
        public async Task Create_WhenCalledSuccessfully_SetsTheUserSessionAsATppUserSession()
        {
            // Arrange
            const string expectedPatientName = "Mr Test User";
            const string expectedSessionId = "ff5246bc-ef03-458a-a1ff-4b6e80268671";
            const string expectedOnlineUserId = "abcde";
            const string expectedPatientId = "12345";
            const string odsCode = "1234";
            var reply = CreateReply(
                suid: expectedSessionId,
                onlineUserId: expectedOnlineUserId,
                patientId: expectedPatientId);

            var userSession = CreateUserSession(expectedPatientName, odsCode, expectedSessionId,
                expectedOnlineUserId, expectedPatientId, _nhsNumber);

            _mockAuthenticate.Setup(x => x
                .Post(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);

            _mockTppSessionMapper
                .Setup(x => x.Map(reply, odsCode, _nhsNumber))
                .Returns(Option.Some(userSession));

            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), odsCode, _nhsNumber);

            // Assert
            var createdResult = result.Should().BeAssignableTo<GpSessionCreateResult.Success>().Subject;
            var tppUserSession = createdResult.UserSession.Should().BeAssignableTo<TppUserSession>().Subject;

            tppUserSession.Name.Should().Be(expectedPatientName);
            tppUserSession.Suid.Should().Be(expectedSessionId);
            tppUserSession.OnlineUserId.Should().Be(expectedOnlineUserId);
            tppUserSession.PatientId.Should().Be(expectedPatientId);
            tppUserSession.OdsCode.Should().Be(odsCode);
            tppUserSession.NhsNumber.Should().Be(_nhsNumber);
        }
        
        [TestMethod]
        public async Task Create_WhenCalledSuccessfully_CallsFetchAndLogAccessInformationWithUserSession()
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

            _mockAuthenticate.Setup(x => x
                    .Post(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);

            _mockTppSessionMapper
                .Setup(x => x.Map(reply, odsCode, _nhsNumber))
                .Returns(Option.Some(userSession));

            // Act
            await _systemUnderTest.Create(CreateConnectionTokenJson(), odsCode, _nhsNumber);

            // Assert
            _mockTppLogMessagingService.Verify(x => x.FetchAndLogAccessInformation(userSession));
        }
        
        [TestMethod]
        public async Task Create_WhenCalledWithErrorResponseProblemLoggingOn_ReturnsForbidden()
        {
            // Arrange
            var reply = CreateReply();
            reply.ErrorResponse = new Error{ ErrorCode = "9" };

            _mockAuthenticate.Setup(x => x
                .Post(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);

            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), "1234", _nhsNumber);

            // Assert
            result.Should().BeAssignableTo<GpSessionCreateResult.Forbidden>();
        }

        [TestMethod]
        public async Task Create_WhenCalledWithErrorResponseProblemOtherThanLoggingOn_ReturnsBadGateway()
        {
            // Arrange
            var reply = CreateReply();
            reply.ErrorResponse = new Error();

            _mockAuthenticate.Setup(x => x
                    .Post(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);

            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), "1234", _nhsNumber);

            // Assert
            result.Should().BeAssignableTo<GpSessionCreateResult.BadGateway>();
        }

        [TestMethod]
        public async Task Create_WhenCalledWithHttpError_ReturnsBadGateway()
        {
            // Arrange
            var reply = CreateReply();
            reply.StatusCode = HttpStatusCode.BadGateway;

            _mockAuthenticate.Setup(x => x
                .Post(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);

            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), "1234", _nhsNumber);

            // Assert
            result.Should().BeAssignableTo<GpSessionCreateResult.BadGateway>();
        }

        [TestMethod]
        public async Task Create_WhenCalledWithMappingError_ReturnsSupplierSystemBadResponse()
        {
            // Arrange
            const string odsCode = "1234";
            var reply = CreateReply();
            reply.StatusCode = HttpStatusCode.BadGateway;

            _mockAuthenticate.Setup(x => x
                    .Post(It.IsAny<Authenticate>()))
                .ReturnsAsync(() => reply);

            _mockTppSessionMapper
                .Setup(x => x.Map(reply, odsCode, _nhsNumber))
                .Returns(Option.None<TppUserSession>());

            // Act
            var result = await _systemUnderTest.Create(CreateConnectionTokenJson(), odsCode, _nhsNumber);

            // Assert
            result.Should().BeAssignableTo<GpSessionCreateResult.BadGateway>();
        }

        [TestMethod]
        public async Task Logoff_WhenCalledWithAValidUserSession_ReturnsSuccessfullyDeleted()
        {
            // Arrange
            var reply = LogoffReply();

            _mockLogOff.Setup(x => x
                .Post(It.IsAny<TppUserSession>()))
                .ReturnsAsync(() => reply);

            // Act
            var result = await _systemUnderTest.Logoff(_tppUserSession);

            // Assert
            result.Should().BeOfType<SessionLogoffResult.Success>();
        }

        [TestMethod]
        public async Task Logoff_WhenCalledWithErrorResponse_ReturnsBadGateway()
        {
            // Arrange
            var reply = LogoffReply();
            reply.ErrorResponse = new Error();

            _mockLogOff
                .Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .ReturnsAsync(() => reply);

            // Act
            var result = await _systemUnderTest.Logoff(_tppUserSession);

            // Assert
            result.Should().BeAssignableTo<SessionLogoffResult.BadGateway>();
        }

        [TestMethod]
        public async Task Logoff_WhenCalledWithHttpError_ReturnsBadGateway()
        {
            // Arrange
            var reply = LogoffReply();
            reply.StatusCode = HttpStatusCode.BadGateway;

            _mockLogOff
                .Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Logoff(_tppUserSession);

            // Assert
            result.Should().BeAssignableTo<SessionLogoffResult.BadGateway>();
        }

        [TestMethod]
        public async Task Logoff_WhenCalledWithInvalidSession_ReturnsNotAuthenticated()
        {
            // Arrange
            _mockLogOff
                .Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .ThrowsAsync(new UnauthorisedGpSystemHttpRequestException());

            // Act
            var result = await _systemUnderTest.Logoff(_tppUserSession);

            // Assert
            result.Should().BeAssignableTo<SessionLogoffResult.Forbidden>();
        }

        private static string CreateConnectionTokenJson(string accountId = "", string passphrase = "") =>
            $"{{ \"accountId\": \"{accountId}\", \"passphrase\": \"{passphrase}\" }}";

        private TppApiObjectResponse<AuthenticateReply> CreateReply(
            string name = "Joanie",
            string suid = "dimsum",
            string onlineUserId = "123",
            string patientId = "123",
            List<string> proxyPatientIds = null)
        {
            var response = new TppApiObjectResponse<AuthenticateReply>(HttpStatusCode.OK)
            {
                Body = new AuthenticateReply
                {
                    OnlineUserId = onlineUserId,
                    User = new User
                    {
                        Person = new Person
                        {
                            PatientId = patientId,
                            PersonName = new PersonName { Name = name }
                        }
                    },
                    Registration = new Registration
                    {
                        PatientAccess = new List<PatientAccess>
                        {
                            new PatientAccess
                            {
                                PatientId = patientId,
                                SiteDetails = new SiteDetails()
                            }
                        }
                    },
                    People = new List<Person>
                    {
                        new Person
                        {
                            PatientId = patientId,
                        }
                    },
                },
                Headers = new Dictionary<string, string>
                {
                    { ResponseSuidHeader, suid }
                }
            };

            if (proxyPatientIds != null)
            {
                foreach (var proxyPatientId in proxyPatientIds)
                {
                    response.Body.People.Add(new Person
                    {
                        PatientId = proxyPatientId,
                    });
                }
            }

            return response;
        }

        private static TppUserSession CreateUserSession(
            string patientName,
            string odsCode,
            string sessionId = "dimsum",
            string onlineUserId = "123",
            string patientId = "123",
            string nhsNumber = "123456789",
            List<string> proxyPatientIds = null)
        {
            var tppUserSession = new TppUserSession
            {
                Name = patientName,
                Suid = sessionId,
                OnlineUserId = onlineUserId,
                PatientId = patientId,
                OdsCode = odsCode,
                NhsNumber = nhsNumber,
                ProxyPatients = new List<TppProxyUserSession>(),
            };

            if (proxyPatientIds != null)
            {
                tppUserSession.ProxyPatients = proxyPatientIds.Select(x => new TppProxyUserSession
                {
                    Id = Guid.NewGuid(),
                    PatientId = x,
                }).ToList();
            }

            return tppUserSession;
        }

        private static TppApiObjectResponse<LogoffReply> LogoffReply()
        {
            var response = new TppApiObjectResponse<LogoffReply>(HttpStatusCode.OK)
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
