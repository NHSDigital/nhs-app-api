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
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Im1Connection
{
    [TestClass]
    public class EmisIm1ConnectionServiceTests
    {
        private const string DefaultEndUserSessionId = "DW3EUerDy8VEZi2gvJ5esg";
        private const string DefaultConnectionToken = "token";
        private const string DefaultSessionId = "session id";
        private const string DefaultOdsCode = "ods code";
        private const string DefaultUserPatientLinkToken = "link token";
        private const string DefaultIdentifierValue = "identifier";

        private IFixture _fixture;
        private Mock<IEmisClient> _mockEmisClient;
        private EmisIm1ConnectionService _systemUnderTest;
        private ILogger<EmisIm1ConnectionService> _logger;
        private Mock<IRegistrationGuidKeyGenerator> _mockRegistrationGuidKeyGenerator;
        private Mock<IRegistrationCacheService> _mockRegistrationCacheService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _logger = _fixture.Freeze<ILogger<EmisIm1ConnectionService>>();
            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _mockRegistrationGuidKeyGenerator = _fixture.Freeze<Mock<IRegistrationGuidKeyGenerator>>();
            _mockRegistrationCacheService = _fixture.Freeze<Mock<IRegistrationCacheService>>();
            _mockEmisClient.Setup(x => x.SessionsEndUserSessionPost()).Returns(
                Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<SessionsEndUserSessionPostResponse>(HttpStatusCode.OK)
                    {
                        Body = _fixture.Create<SessionsEndUserSessionPostResponse>()
                    }));

            _systemUnderTest = _fixture.Create<EmisIm1ConnectionService>();
        }

        [TestMethod]
        public async Task Verify_ReturnsAConnection_WhenRequested()
        {
            const string expectedNhsNumber = "AB123";

            var expectedNhsNumbers = new[] {new PatientNhsNumber {NhsNumber = expectedNhsNumber}};
            var userPatientLinkModels = new[] {CreateUserPatientLinkModel()};
            var patientIdentifiers = new[] {CreatePatientIdentifier(expectedNhsNumber)};
            var emisClientMock = new Mock<IEmisClient>();

            SetupEmisClientMock(
                emisClientMock,
                userPatientLinkModels: userPatientLinkModels,
                patientIdentifiers: patientIdentifiers
            );

            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object, _logger,
                _mockRegistrationGuidKeyGenerator.Object, _mockRegistrationCacheService.Object);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SuccessfullyVerified>();

            var successResult = result as Im1ConnectionVerifyResult.SuccessfullyVerified;

            successResult.Response.ConnectionToken.Should().Be(DefaultConnectionToken);
            successResult.Response.NhsNumbers.Should().BeEquivalentTo(expectedNhsNumbers);
        }

        [TestMethod]
        public async Task
            Verify_ReturnsAnNhsNumberUsingTheSelfUserPatientLinkToken_WhenTheMultipleUserPatientLinkTokensAreReturnedFromEmis()
        {
            const string expectedNhsNumber = "345";
            var emisClientMock = new Mock<IEmisClient>();
            var userPatientLinkModels = new[]
            {
                CreateUserPatientLinkModel("proxy", AssociationType.Proxy),
                CreateUserPatientLinkModel("self", AssociationType.Self)
            };
            var patientIdentifiers = new[] {CreatePatientIdentifier(expectedNhsNumber)};

            SetupEmisClientMock(
                emisClientMock,
                userPatientLinkToken: "self", userPatientLinkModels: userPatientLinkModels, patientIdentifiers: patientIdentifiers);

            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object, _logger,
                _mockRegistrationGuidKeyGenerator.Object, _mockRegistrationCacheService.Object);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SuccessfullyVerified>();

            var successResult = result as Im1ConnectionVerifyResult.SuccessfullyVerified;

            successResult.Response.NhsNumbers.Single().NhsNumber.Should().Be(expectedNhsNumber);
        }

        [TestMethod]
        public async Task
            Verify_ReturnsThePatientNhsNumbersOfTypeNhsNumber_WhenTheMultipleNhsNumbersAreReturnedFromEmis()
        {
            var expectedNhsNumbers = new[] {"1234", "345"};
            var emisClientMock = new Mock<IEmisClient>();
            var userPatientLinkModels = new[] {CreateUserPatientLinkModel()};

            var patientIdentifiers = new[]
            {
                CreatePatientIdentifier("boo", IdentifierType.ChiNumber),
                CreatePatientIdentifier("hoo", IdentifierType.Unknown),
                CreatePatientIdentifier(expectedNhsNumbers[0]),
                CreatePatientIdentifier(expectedNhsNumbers[1])
            };

            SetupEmisClientMock(
                emisClientMock,
                userPatientLinkModels: userPatientLinkModels, patientIdentifiers: patientIdentifiers);

            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object, _logger,
                _mockRegistrationGuidKeyGenerator.Object, _mockRegistrationCacheService.Object);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SuccessfullyVerified>();

            var successResult = result as Im1ConnectionVerifyResult.SuccessfullyVerified;

            successResult.Response.NhsNumbers.Select(x => x.NhsNumber).Should().BeEquivalentTo(expectedNhsNumbers);
        }

        [TestMethod]
        public async Task Verify_ReturnsEmptyNhsNumbersWhenEmisReturnsEmptyPatientIdentifiers()
        {
            var emisClientMock = new Mock<IEmisClient>();
            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object, _logger,
                _mockRegistrationGuidKeyGenerator.Object, _mockRegistrationCacheService.Object);
            var userPatientLinkModels = new[] {CreateUserPatientLinkModel()};
            var patientIdentifiers = Array.Empty<PatientIdentifier>();

            SetupEmisClientMock(emisClientMock, userPatientLinkModels: userPatientLinkModels,
                patientIdentifiers: patientIdentifiers);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SuccessfullyVerified>();

            var successResult = result as Im1ConnectionVerifyResult.SuccessfullyVerified;

            successResult.Response.NhsNumbers.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Verify_ReturnsEmptyNhsNumbers_WhenEmisReturnsNullPatientIdentifiers()
        {
            var emisClientMock = new Mock<IEmisClient>();
            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object, _logger,
                _mockRegistrationGuidKeyGenerator.Object, _mockRegistrationCacheService.Object);
            var userPatientLinkModels = new[] {CreateUserPatientLinkModel()};
            var patientIdentifiers = (PatientIdentifier[]) null;

            SetupEmisClientMock(emisClientMock, userPatientLinkModels: userPatientLinkModels,
                patientIdentifiers: patientIdentifiers);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SuccessfullyVerified>();
            var successResult = result as Im1ConnectionVerifyResult.SuccessfullyVerified;

            successResult.Response.NhsNumbers.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Verify_ReturnsNotFound_WhenEmisReturnsEmptyUserLinkModels()
        {
            var emisClientMock = new Mock<IEmisClient>();
            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object, _logger,
                _mockRegistrationGuidKeyGenerator.Object, _mockRegistrationCacheService.Object);
            var userPatientLinkModels = Array.Empty<UserPatientLink>();

            SetupEmisClientMock(emisClientMock, userPatientLinkModels: userPatientLinkModels);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.NotFound>();
        }

        [TestMethod]
        public async Task Verify_ReturnsNotFound_WhenEmisReturnsNullUserLinkModels()
        {
            var emisClientMock = new Mock<IEmisClient>();
            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object, _logger,
                _mockRegistrationGuidKeyGenerator.Object, _mockRegistrationCacheService.Object);
            var userPatientLinkModels = (UserPatientLink[]) null;

            SetupEmisClientMock(emisClientMock, userPatientLinkModels: userPatientLinkModels);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.NotFound>();
        }

        [TestMethod]
        public async Task Verify_ReturnsSupplierSystemUnavailable_WhenEmisClientThrowsHttpRequestException()
        {
            var emisClientMock = new Mock<IEmisClient>();
            emisClientMock
                .Setup(x => x.SessionsEndUserSessionPost())
                .Throws<HttpRequestException>();

            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object, _logger,
                _mockRegistrationGuidKeyGenerator.Object, _mockRegistrationCacheService.Object);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Register_SuccessfullyRegistered_WhenAccessGuidIsCached()
        {
            // Arrange
            // emis client returns expected responses
            var endUserSessionResponse = _fixture.Create<SessionsEndUserSessionPostResponse>();
            _mockEmisClient.Setup(x => x.SessionsEndUserSessionPost()).Returns(
                Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<SessionsEndUserSessionPostResponse>(HttpStatusCode.OK)
                    {
                        Body = endUserSessionResponse
                    }));

            const string key = "Key";
            _mockRegistrationGuidKeyGenerator.Setup(x => x.GenerateRegistrationKey(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(key);

            const string registrationGuid = "ACCESS_GUID";
            _mockRegistrationCacheService.Setup(x => x.GetRegistrationGuid(key))
                .Returns(Task.FromResult(
                    Option.Some(registrationGuid)
                    ));
            
            
            var sessionResponse = _fixture.Create<SessionsPostResponse>();
            _mockEmisClient.Setup(x => x.SessionsPost(It.IsAny<string>(), It.IsAny<SessionsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<SessionsPostResponse>(HttpStatusCode.OK)
                    {
                        Body = sessionResponse
                    }));
            
            var demographicsResponse = _fixture.Create<DemographicsGetResponse>();
            _mockEmisClient.Setup(x => x.DemographicsGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = demographicsResponse
                    }));
            
            var request = _fixture.Create<PatientIm1ConnectionRequest>();
            
            // Act
            var result = await _systemUnderTest.Register(request);
            
            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.SuccessfullyRegistered>();
                       
            _mockEmisClient.Verify(
                x => x.MeApplicationsPost(
                    It.IsAny<string>(), It.IsAny<MeApplicationsPostRequest>()),
                Times.Never());
        }

        [TestMethod]
        public async Task Register_SuccessfullyRegistered_WhenDataAreCorrect()
        {
            // Arrange
            // emis client returns expected responses
            var endUserSessionResponse = _fixture.Create<SessionsEndUserSessionPostResponse>();
            _mockEmisClient.Setup(x => x.SessionsEndUserSessionPost()).Returns(
                Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<SessionsEndUserSessionPostResponse>(HttpStatusCode.OK)
                    {
                        Body = endUserSessionResponse
                    }));
            
            var meApplicationsResponse = _fixture.Create<MeApplicationsPostResponse>();
            _mockEmisClient.Setup(x => x.MeApplicationsPost(It.IsAny<string>(), It.IsAny<MeApplicationsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode.OK)
                    {
                        Body = meApplicationsResponse
                    }));

            var sessionResponse = _fixture.Create<SessionsPostResponse>();
            _mockEmisClient.Setup(x => x.SessionsPost(It.IsAny<string>(), It.IsAny<SessionsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<SessionsPostResponse>(HttpStatusCode.OK)
                    {
                        Body = sessionResponse
                    }));
            
            var demographicsResponse = _fixture.Create<DemographicsGetResponse>();
            _mockEmisClient.Setup(x => x.DemographicsGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = demographicsResponse
                    }));
            
            var request = _fixture.Create<PatientIm1ConnectionRequest>();
            
            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.SuccessfullyRegistered>();
        }
        
        [TestMethod]
        public async Task Register_ReturnsSupplierSystemUnavailable_WhenEmisClientThrowsHttpRequestException()
        {
            // Arrange
            // emis client throws HttpRequestException
            _mockEmisClient.Setup(x => x.SessionsEndUserSessionPost())
                .Throws<HttpRequestException>();

            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Register_ReturnsAccountAlreadyExists_WhenEmisClientMeApplicationsReturnsConflict()
        {
            // Arrange
            // emis client MeApplications POST returns 409
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            _mockEmisClient.Setup(x =>
                    x.MeApplicationsPost(It.IsAny<string>(), It.IsAny<MeApplicationsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode.Conflict)));

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.AccountAlreadyExists>();
        }

        [TestMethod]
        public async Task Register_ReturnsAccountAlreadyExists_WhenEmisClientMeApplicationsReturnsSpecificErrorMessage()
        {
            const string alreadyLinkedErrorMessage = "Registered online user is already linked";

            // Arrange
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;

            _mockEmisClient.Setup(x =>x.MeApplicationsPost(It.IsAny<string>(), It.IsAny<MeApplicationsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode
                        .InternalServerError) {ExceptionErrorResponse = errorResponse}));

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.AccountAlreadyExists>();
        }
        
        [TestMethod]
        public async Task Register_ReturnsNotFound_WhenEmisClientMeApplicationsReturnsAccountIdNotFoundErrorMessage()
        {
            const string accountNotFoundErrorMessage = "No registered online user found for given linkage details";

            // Arrange
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = accountNotFoundErrorMessage;

            _mockEmisClient.Setup(x =>x.MeApplicationsPost(It.IsAny<string>(), It.IsAny<MeApplicationsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode
                        .InternalServerError) {ExceptionErrorResponse = errorResponse}));

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.NotFound>();
        }
        
        [TestMethod]
        public async Task Register_ReturnsNotFound_WhenEmisClientMeApplicationsReturnsLinkageKeyDoesNotMatchErrorMessage()
        {
            const string linkageKeyDoesNotMatchErrorMessage = "Invalid linkage details";

            // Arrange
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = linkageKeyDoesNotMatchErrorMessage;

            _mockEmisClient.Setup(x =>x.MeApplicationsPost(It.IsAny<string>(), It.IsAny<MeApplicationsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode
                        .InternalServerError) {ExceptionErrorResponse = errorResponse}));

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.NotFound>();
        }
        
        [TestMethod]
        public async Task Register_ReturnsNotFound_WhenEmisClientMeApplicationsReturnsIncorrectSurnameOrDateOfBirthErrorMessage()
        {
            const string incorrectSurnameOrDateOfBirthErrorMessage = "No match found for given demographics";

            // Arrange
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = incorrectSurnameOrDateOfBirthErrorMessage;

            _mockEmisClient.Setup(x =>x.MeApplicationsPost(It.IsAny<string>(), It.IsAny<MeApplicationsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode
                        .InternalServerError) {ExceptionErrorResponse = errorResponse}));

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.NotFound>();
        }
        
        [TestMethod]
        public async Task Register_ReturnsBadRequest_WhenEmisClientMeApplicationsReturnsBadRequest()
        {
            // Arrange
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            _mockEmisClient.Setup(x =>x.MeApplicationsPost(It.IsAny<string>(), It.IsAny<MeApplicationsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode
                        .BadRequest)));

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.BadRequest>();
        }
        
        [TestMethod]
        public async Task Register_ReturnsBadRequest_WhenEmisClientMeApplicationsReturnsBadRequest_NullMessage()
        {
            // Arrange
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = null;

            _mockEmisClient.Setup(x =>x.MeApplicationsPost(It.IsAny<string>(), It.IsAny<MeApplicationsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode
                        .BadRequest) {ExceptionErrorResponse = errorResponse}));

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.BadRequest>();
        }
        
        [TestMethod]
        public async Task Register_ReturnsBadRequest_WhenEmisClientMeApplicationsReturnsBadRequest_NullExceptions()
        {
            // Arrange
            var request = _fixture.Create<PatientIm1ConnectionRequest>();

            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions = null;

            _mockEmisClient.Setup(x =>x.MeApplicationsPost(It.IsAny<string>(), It.IsAny<MeApplicationsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode
                        .BadRequest) {ExceptionErrorResponse = errorResponse}));

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.BadRequest>();
        }

        private static PatientIdentifier CreatePatientIdentifier(
            string identifierValue = DefaultIdentifierValue,
            IdentifierType identifierType = IdentifierType.NhsNumber
        )
        {
            return new PatientIdentifier
            {
                IdentifierType = identifierType,
                IdentifierValue = identifierValue
            };
        }

        private static UserPatientLink CreateUserPatientLinkModel(
            string userPatientLinkToken = DefaultUserPatientLinkToken,
            AssociationType associationType = AssociationType.Self
        )
        {
            return new UserPatientLink
            {
                UserPatientLinkToken = userPatientLinkToken,
                AssociationType = associationType
            };
        }

        private static void SetupEmisClientMock(Mock<IEmisClient> emisClientMock,
            string endUserSessionId = DefaultEndUserSessionId,
            string sessionId = DefaultSessionId,
            string userPatientLinkToken = null,
            IEnumerable<UserPatientLink> userPatientLinkModels = null,
            IEnumerable<PatientIdentifier> patientIdentifiers = null)
        {
            userPatientLinkToken =
                userPatientLinkToken ?? userPatientLinkModels?.FirstOrDefault()?.UserPatientLinkToken;

            var endUserSessionResponse = new SessionsEndUserSessionPostResponse
            {
                EndUserSessionId = endUserSessionId
            };

            var sessionResponse = new SessionsPostResponse
            {
                SessionId = sessionId,
                UserPatientLinks = userPatientLinkModels
            };

            var demographicsResponse = new DemographicsGetResponse
            {
                PatientIdentifiers = patientIdentifiers
            };

            emisClientMock
                .Setup(x => x.SessionsEndUserSessionPost())
                .ReturnsAsync(
                    new EmisClient.EmisApiObjectResponse<SessionsEndUserSessionPostResponse>(HttpStatusCode.OK)
                    {
                        Body = endUserSessionResponse
                    });

            emisClientMock
                .Setup(x => x.SessionsPost(endUserSessionId, It.IsAny<SessionsPostRequest>()))
                .ReturnsAsync(
                    new EmisClient.EmisApiObjectResponse<SessionsPostResponse>(HttpStatusCode.OK)
                    {
                        Body = sessionResponse
                    });

            emisClientMock
                .Setup(x => x.DemographicsGet(userPatientLinkToken, sessionResponse.SessionId,
                    endUserSessionResponse.EndUserSessionId))
                .ReturnsAsync(
                    new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = demographicsResponse
                    });
        }
    }
}
