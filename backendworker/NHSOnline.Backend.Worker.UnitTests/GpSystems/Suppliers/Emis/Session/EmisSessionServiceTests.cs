using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Session
{
    [TestClass]
    public class EmisSessionServiceTests
    {
        private IFixture _fixture;
        private Mock<IEmisClient> _mockEmisClient;
        private ILogger<EmisSessionService> _logger;
        private EmisSessionService _systemUnderTest;
        private SessionsEndUserSessionPostResponse _endUserSessionResponse;
        private SessionsPostResponse _sessionsResponse;

        private string _connectionToken;
        private string _odsCode;
        private string _nhsNumber;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<EmisSessionService>>();
            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _endUserSessionResponse = _fixture.Create<SessionsEndUserSessionPostResponse>();
            _connectionToken = _fixture.Create<string>();
            _odsCode = _fixture.Create<string>();
            _nhsNumber = _fixture.Create<string>();
            
            _mockEmisClient.Setup(x => x.SessionsEndUserSessionPost()).Returns(
                Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<SessionsEndUserSessionPostResponse>(HttpStatusCode.OK)
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
                    new EmisClient.EmisApiObjectResponse<SessionsPostResponse>(HttpStatusCode.OK)
                    {
                        Body = _sessionsResponse,
                        ExceptionErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));

            _mockEmisClient.Setup(x => x.PracticeSettingsGet(
                    It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>(HttpStatusCode.OK)
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
            result.Should().BeAssignableTo<GpSessionCreateResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Create_EmisClientThrowsHttpRequestExceptionFromSession_ReturnsSupplierSystemUnavailable()
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
            result.Should().BeAssignableTo<GpSessionCreateResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Create_EmisClientEndUserSessionPostUnsuccessful_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var unsuccessfulEndUserSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<SessionsEndUserSessionPostResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
                .Create();

            _mockEmisClient
                .Setup(x => x.SessionsEndUserSessionPost())
                .Returns(Task.FromResult(unsuccessfulEndUserSession))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<GpSessionCreateResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Create_EmisClientSessionsPostReturnsForbidden_ReturnsInvalidIm1ConnectionToken()
        {
            // Arrange
            var forbiddenSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<SessionsPostResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.Forbidden)
                .With(x => x.Body, null)
                .With(x => x.ErrorResponseBadRequest, null)
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
            result.Should().BeAssignableTo<GpSessionCreateResult.InvalidIm1ConnectionToken>();
        }

        [TestMethod]
        public async Task Create_EmisClientSessionsPostReturnsBadRequest_ReturnsInvalidIm1ConnectionToken()
        {
            // Arrange
            var badRequestSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<SessionsPostResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.BadRequest)
                .With(x => x.Body, null)
                .With(x => x.ExceptionErrorResponse, null)
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
            result.Should().BeAssignableTo<GpSessionCreateResult.InvalidIm1ConnectionToken>();
        }

        [TestMethod]
        public async Task Create_EmisClientSessionsPostReturnsInternalServerError_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var forbiddenSession = _fixture
                .Build<EmisClient.EmisApiObjectResponse<SessionsPostResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
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
            result.Should().BeAssignableTo<GpSessionCreateResult.SupplierSystemUnavailable>();
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

            
            var systemUnderTest = new EmisSessionService(_mockEmisClient.Object, _logger, enumMapper);
            
            // Act
            var result = await systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);

            // Assert
            _mockEmisClient.VerifyAll();
            var createdResult = result.Should().BeAssignableTo<GpSessionCreateResult.SuccessfullyCreated>().Subject;

            var expectedResult = new GpSessionCreateResult.SuccessfullyCreated(expected, new EmisUserSession { NhsNumber = _nhsNumber, OdsCode = _odsCode});

            createdResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task Create_HappyPath_ReturnsAUserSessionInTheResult()
        {
            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);
            
            // Assert
            result
                .Should()
                .BeAssignableTo<GpSessionCreateResult.SuccessfullyCreated>()
                .Subject
                .UserSession
                .Should()
                .NotBeNull();
        }

        [TestMethod] 
        public async Task Create_HappyPath_ReturnsAUserSessionWithTheEmisSupplier()
        {
            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);
            
            // Assert
            result
                .Should()
                .BeAssignableTo<GpSessionCreateResult.SuccessfullyCreated>()
                .Subject
                .UserSession
                .Supplier
                .Should()
                .Be(Supplier.Emis);
        }
        
        [TestMethod]
        public async Task Create_HappyPath_ReturnsAEmisUserSession()
        {
            // Act
            var result = await _systemUnderTest.Create(_connectionToken, _odsCode, _nhsNumber);
            
            // Assert
            result
                .Should()
                .BeAssignableTo<GpSessionCreateResult.SuccessfullyCreated>()
                .Subject
                .UserSession
                .Should()
                .BeAssignableTo<EmisUserSession>();
        }
    }
}
