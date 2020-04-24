using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Session
{
    [TestClass]
    public class EmisSessionServiceTests
    {
        private IFixture _fixture;
        private Mock<IEmisClient> _mockEmisClient;
        private Mock<ILogger<EmisSessionService>> _logger;
        private EmisSessionService _systemUnderTest;
        private SessionsEndUserSessionPostResponse _endUserSessionResponse;
        private SessionsPostResponse _sessionsResponse;

        private string _connectionToken;
        private string _odsCode;
        private string _nhsNumber;
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = new Mock<ILogger<EmisSessionService>>();
            _fixture.Inject(_logger);
            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _endUserSessionResponse = _fixture.Create<SessionsEndUserSessionPostResponse>();
            _connectionToken = _fixture.Create<string>();
            _odsCode = _fixture.Create<string>();
            _nhsNumber = _fixture.Create<string>();
            
            _sampleSuccessStatusCodes = new List<HttpStatusCode>()
            {
                HttpStatusCode.OK
            };

            _mockEmisClient.Setup(x => x.SessionsEndUserSessionPost()).Returns(
                Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<SessionsEndUserSessionPostResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.SessionsEndUserSessionPost, _sampleSuccessStatusCodes)
                    {
                        Body = _endUserSessionResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            _sessionsResponse = _fixture.Create<SessionsPostResponse>();

            _mockEmisClient
                .Setup(x => x.SessionsPost(_endUserSessionResponse.EndUserSessionId,
                    It.Is<SessionsPostRequest>(y =>
                        y.AccessIdentityGuid.Equals(_connectionToken, StringComparison.Ordinal)
                        && y.NationalPracticeCode.Equals(_odsCode, StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<SessionsPostResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.SessionsPost, _sampleSuccessStatusCodes)
                    {
                        Body = _sessionsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            _mockEmisClient.Setup(x => x.PracticeSettingsGet(
                    It.IsAny<EmisRequestParameters>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.PracticeSettingsGet, _sampleSuccessStatusCodes)
                {
                    Body = new PracticeSettingsGetResponse()
                    {
                        InputRequirements = new PracticeSettingsInputRequirements()
                        {
                            AppointmentBookingReason = "RequestedOptional",
                            PrescribingComment = "RequestedOptional"
                        }
                    }
                });

            _systemUnderTest = _fixture.Create<EmisSessionService>();
        }

        [TestMethod]
        public async Task
            Create_EmisClientThrowsHttpRequestExceptionFromEndUserSession_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            // emis client throws HttpRequestException
            _mockEmisClient
                .Setup(x => x.SessionsEndUserSessionPost())
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<GpSessionCreateResult.BadGateway>();
        }

        [TestMethod]
        public async Task Create_EmisClientThrowsHttpRequestExceptionFromSession_ReturnsBadGateway()
        {
            // Arrange
            // emis client throws HttpRequestException
            _mockEmisClient
                .Setup(x => x.SessionsPost(It.IsAny<string>(), It.IsAny<SessionsPostRequest>()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<GpSessionCreateResult.BadGateway>();
        }

        [TestMethod]
        public async Task Create_EmisClientEndUserSessionPostUnsuccessful_ReturnsBadGateway()
        {
            // Arrange
            var unsuccessfulEndUserSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<SessionsEndUserSessionPostResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, () => null)
                .Create();

            _mockEmisClient
                .Setup(x => x.SessionsEndUserSessionPost())
                .Returns(Task.FromResult(unsuccessfulEndUserSession))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<GpSessionCreateResult.BadGateway>();
        }

        [TestMethod]
        public async Task Create_EmisClientSessionsPostReturnsForbidden_ReturnsForbidden()
        {
            // Arrange
            var forbiddenSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<SessionsPostResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.Forbidden)
                .With(x => x.Body, () => null)
                .With(x => x.ErrorResponseBadRequest, () => null)
                .Create();

            _mockEmisClient
                .Setup(x => x.SessionsPost(_endUserSessionResponse.EndUserSessionId,
                    It.Is<SessionsPostRequest>(y =>
                        y.AccessIdentityGuid.Equals(_connectionToken, StringComparison.Ordinal)
                        && y.NationalPracticeCode.Equals(_odsCode, StringComparison.Ordinal))))
                .Returns(Task.FromResult(forbiddenSession))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<GpSessionCreateResult.Forbidden>();
        }

        [TestMethod]
        public async Task Create_EmisClientSessionsPostReturnsBadRequest_ReturnsForbidden()
        {
            // Arrange
            var badRequestSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<SessionsPostResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.BadRequest)
                .With(x => x.Body, () => null)
                .With(x => x.ExceptionErrorResponse, () => null)
                .Create();

            _mockEmisClient
                .Setup(x => x.SessionsPost(_endUserSessionResponse.EndUserSessionId,
                    It.Is<SessionsPostRequest>(y =>
                        y.AccessIdentityGuid.Equals(_connectionToken, StringComparison.Ordinal)
                        && y.NationalPracticeCode.Equals(_odsCode, StringComparison.Ordinal))))
                .Returns(Task.FromResult(badRequestSession))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<GpSessionCreateResult.Forbidden>();
        }

        [TestMethod]
        public async Task Create_EmisClientSessionsPostReturnsInternalServerError_ReturnsBadGateway()
        {
            // Arrange
            var forbiddenSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<SessionsPostResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, () => null)
                .Create();

            _mockEmisClient
                .Setup(x => x.SessionsPost(_endUserSessionResponse.EndUserSessionId,
                    It.Is<SessionsPostRequest>(y =>
                        y.AccessIdentityGuid.Equals(_connectionToken, StringComparison.Ordinal)
                        && y.NationalPracticeCode.Equals(_odsCode, StringComparison.Ordinal))))
                .Returns(Task.FromResult(forbiddenSession))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<GpSessionCreateResult.BadGateway>();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task Create_EmptyUserPatientLinkToken_ReturnsForbidden(string userPatientLinkToken)
        {
            //Arrange
            var sessionPostResponse = CreateSessionsPostResponse(userPatientLinkToken);

            _mockEmisClient.Setup(
                ec => ec.SessionsPost(
                    It.IsAny<string>(),
                    It.IsAny<SessionsPostRequest>()))
                .Returns(sessionPostResponse);

            //Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            //Assert
            result.Should().BeAssignableTo<GpSessionCreateResult.Forbidden>();
        }


        [DataTestMethod]
        [DataRow("Mr", "Fred", "Blogs", "Mr Fred Blogs")]
        [DataRow("", "Fred", "Blogs", "Fred Blogs")]
        [DataRow("Mr", "", "Blogs", "Mr Blogs")]
        [DataRow("Mr", "Fred", "", "Mr Fred")]
        public async Task Create_HappyPath_ReturnsSuccessfullyCreatedWithExpectedUserData(string title, string firstname, string surname, string expected)
        {
            // Arrange
            _sessionsResponse.Title = title;
            _sessionsResponse.FirstName = firstname;
            _sessionsResponse.Surname = surname;

            var enumMapperLogger = _fixture.Create<ILoggerFactory>().CreateLogger<EmisEnumMapper>();
            var enumMapper = new EmisEnumMapper(enumMapperLogger);

            var systemUnderTest = CreateEmisSessionService(enumMapper);

            // Act
            var result = await systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            _mockEmisClient.VerifyAll();
            var createdResult = result.Should().BeAssignableTo<GpSessionCreateResult.Success>().Subject;

            createdResult.UserSession.Name.Should().Be(expected);
            createdResult.UserSession.Id.Should().NotBeEmpty();
            createdResult.UserSession.NhsNumber.Should().Be(_nhsNumber);
            createdResult.UserSession.OdsCode.Should().Be(_odsCode);
        }

        [TestMethod]
        public async Task Create_NoLinkedAccounts_LogsSuccessfully()
        {
            // Arrange
            var enumMapperLogger = _fixture.Create<ILoggerFactory>().CreateLogger<EmisEnumMapper>();
            var enumMapper = new EmisEnumMapper(enumMapperLogger);

            var systemUnderTest = CreateEmisSessionService(enumMapper);

            // only self patient link
            _sessionsResponse.UserPatientLinks = new List<UserPatientLink>
            {
                new UserPatientLink
                {
                    UserPatientLinkToken = _fixture.Create<string>(),
                    AssociationType = AssociationType.Self,
                    NationalPracticeCode = "ABC12",
                }
            };

            // Act
            var result = await systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            _mockEmisClient.VerifyAll();
            var createdResult = result.Should().BeAssignableTo<GpSessionCreateResult.Success>().Subject;

            createdResult.Should().BeOfType<GpSessionCreateResult.Success>();
            _logger.VerifyLogger(LogLevel.Information,
                $"User has linked_accounts=0, with different_ods_codes_to_user=0", Times.Once());
        }

        [TestMethod]
        public async Task Create_LinkedAccountFromDifferentPractice_LogsSuccessfully()
        {
            // Arrange
            var enumMapperLogger = _fixture.Create<ILoggerFactory>().CreateLogger<EmisEnumMapper>();
            var enumMapper = new EmisEnumMapper(enumMapperLogger);

            var systemUnderTest = CreateEmisSessionService(enumMapper);

            // only self patient link
            _sessionsResponse.UserPatientLinks = new List<UserPatientLink>
            {
                new UserPatientLink
                {
                    UserPatientLinkToken = _fixture.Create<string>(),
                    AssociationType = AssociationType.Self,
                    NationalPracticeCode = "ABC12",
                },
                new UserPatientLink
                {
                    UserPatientLinkToken = _fixture.Create<string>(),
                    AssociationType = AssociationType.Proxy,
                    NationalPracticeCode = "ABC13",
                },
            };

            // Act
            var result = await systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            _mockEmisClient.VerifyAll();
            var createdResult = result.Should().BeAssignableTo<GpSessionCreateResult.Success>().Subject;

            createdResult.Should().BeOfType<GpSessionCreateResult.Success>();
            _logger.VerifyLogger(LogLevel.Information,
                "User has linked_accounts=1, with different_ods_codes_to_user=1", Times.Once());
        }

        [TestMethod]
        public async Task Create_LinkedAccountFromSamePractice_LogsSuccessfully()
        {
            // Arrange
            var enumMapperLogger = _fixture.Create<ILoggerFactory>().CreateLogger<EmisEnumMapper>();
            var enumMapper = new EmisEnumMapper(enumMapperLogger);

            var systemUnderTest = CreateEmisSessionService(enumMapper);

            // only self patient link
            _sessionsResponse.UserPatientLinks = new List<UserPatientLink>
            {
                new UserPatientLink
                {
                    UserPatientLinkToken = _fixture.Create<string>(),
                    AssociationType = AssociationType.Self,
                    NationalPracticeCode = "ABC12",
                },
                new UserPatientLink
                {
                    UserPatientLinkToken = _fixture.Create<string>(),
                    AssociationType = AssociationType.Proxy,
                    NationalPracticeCode = "ABC12",
                },
            };

            // Act
            var result = await systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            _mockEmisClient.VerifyAll();
            var createdResult = result.Should().BeAssignableTo<GpSessionCreateResult.Success>().Subject;

            createdResult.Should().BeOfType<GpSessionCreateResult.Success>();
            _logger.VerifyLogger(LogLevel.Information,
                "User has linked_accounts=1, with different_ods_codes_to_user=0", Times.Once());
        }

        [TestMethod]
        public async Task Create_HappyPath_ReturnsSuccessfullyCreatedWithExpectedUserData_IsProxySetToFalseWhenProxyPatientsIsEmpty()
        {
            // Arrange
            var expectedName =  $"{_sessionsResponse.Title} {_sessionsResponse.FirstName} {_sessionsResponse.Surname}";

            var enumMapperLogger = _fixture.Create<ILoggerFactory>().CreateLogger<EmisEnumMapper>();
            var enumMapper = new EmisEnumMapper(enumMapperLogger);

            var systemUnderTest = CreateEmisSessionService(enumMapper);

            _sessionsResponse.UserPatientLinks.ToList()[0].AssociationType = AssociationType.None;
            _sessionsResponse.UserPatientLinks.ToList()[1].AssociationType = AssociationType.Self;
            _sessionsResponse.UserPatientLinks.ToList()[2].AssociationType = AssociationType.None;

            // Act
            var result = await systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            _mockEmisClient.VerifyAll();
            var createdResult = result.Should().BeAssignableTo<GpSessionCreateResult.Success>().Subject;

            var expectedResult = new GpSessionCreateResult.Success(new EmisUserSession
            {
                Name = expectedName,
                Id = createdResult.UserSession.Id, NhsNumber = _nhsNumber, OdsCode = _odsCode
            });

            createdResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task Create_HappyPath_ReturnsSuccessfullyCreatedWithExpectedUserData_IsProxySetToTrueWhenProxyPatientsIsPopulated()
        {
            // Arrange
            var expectedName =  $"{_sessionsResponse.Title} {_sessionsResponse.FirstName} {_sessionsResponse.Surname}";

            var enumMapperLogger = _fixture.Create<ILoggerFactory>().CreateLogger<EmisEnumMapper>();
            var enumMapper = new EmisEnumMapper(enumMapperLogger);

            var systemUnderTest = CreateEmisSessionService(enumMapper);

            _sessionsResponse.UserPatientLinks.ToList()[0].AssociationType = AssociationType.Proxy;
            _sessionsResponse.UserPatientLinks.ToList()[1].AssociationType = AssociationType.Self;
            _sessionsResponse.UserPatientLinks.ToList()[2].AssociationType = AssociationType.Proxy;

            // Act
            var result = await systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            _mockEmisClient.VerifyAll();
            var createdResult = result.Should().BeAssignableTo<GpSessionCreateResult.Success>().Subject;
            
            createdResult.UserSession.Name.Should().Be(expectedName);
            createdResult.UserSession.Id.Should().NotBeEmpty();
            createdResult.UserSession.NhsNumber.Should().Be(_nhsNumber);
            createdResult.UserSession.OdsCode.Should().Be(_odsCode);
            createdResult.UserSession.HasLinkedAccounts.Should().BeTrue();
        }


        [TestMethod]
        public async Task Create_HappyPath_ReturnsAUserSessionInTheResult()
        {
            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            result.Should().BeAssignableTo<GpSessionCreateResult.Success>()
                .Subject.UserSession.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Create_HappyPath_ReturnsAUserSessionWithTheEmisSupplier()
        {
            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            result.Should().BeAssignableTo<GpSessionCreateResult.Success>()
                .Subject.UserSession.Supplier.Should().Be(Supplier.Emis);
        }

        [TestMethod]
        public async Task Create_HappyPath_ReturnsAEmisUserSession()
        {
            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            result
                .Should().BeAssignableTo<GpSessionCreateResult.Success>()
                .Subject.UserSession.Should().BeAssignableTo<EmisUserSession>();
        }

        private EmisSessionService CreateEmisSessionService(EmisEnumMapper enumMapper)
        {
            return new EmisSessionService(
                _mockEmisClient.Object,
                _logger.Object,
                enumMapper,
                new EmisTokenValidationService(new Mock<ILogger<EmisTokenValidationService>>().Object));
        }

        private Task<EmisClient.EmisApiObjectResponse<SessionsPostResponse>> CreateSessionsPostResponse(string userPatientLinkToken)
        {
            return Task.FromResult(
                new EmisClient.EmisApiObjectResponse<SessionsPostResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.SessionsPost, _sampleSuccessStatusCodes)
                {
                    Body = new SessionsPostResponse
                    {
                        SessionId = "1234567890",
                        UserPatientLinks = new List<UserPatientLink>
                        {
                            new UserPatientLink
                            {
                                UserPatientLinkToken = userPatientLinkToken
                            }
                        },
                        FirstName = "Kanye",
                        Surname = "West"
                    },
                    ExceptionErrorResponse = null,
                    ErrorResponseBadRequest = null,
                    StatusCode = HttpStatusCode.OK
                });
        }
    }
}