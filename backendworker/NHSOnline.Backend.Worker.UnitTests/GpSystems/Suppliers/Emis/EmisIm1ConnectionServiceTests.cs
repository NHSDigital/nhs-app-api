using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis
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

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();
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

            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object);

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
                patientIdentifiers: patientIdentifiers,
                userPatientLinkModels: userPatientLinkModels,
                userPatientLinkToken: "self"
            );

            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object);

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
                patientIdentifiers: patientIdentifiers,
                userPatientLinkModels: userPatientLinkModels
            );

            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SuccessfullyVerified>();

            var successResult = result as Im1ConnectionVerifyResult.SuccessfullyVerified;

            successResult.Response.NhsNumbers.Select(x => x.NhsNumber).Should().BeEquivalentTo(expectedNhsNumbers);
        }

        [TestMethod]
        public async Task Verify_ReturnsEmptyNhsNumbersWhenEmisReturnsEmptyPatientIdentifiers()
        {
            var emisClientMock = new Mock<IEmisClient>();
            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object);
            var userPatientLinkModels = new[] {CreateUserPatientLinkModel()};
            var patientIdentifiers = new PatientIdentifier[0];

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
            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object);
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
            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object);
            var userPatientLinkModels = new UserPatientLink[0];

            SetupEmisClientMock(emisClientMock, userPatientLinkModels: userPatientLinkModels);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.NotFound>();
        }

        [TestMethod]
        public async Task Verify_ReturnsNotFound_WhenEmisReturnsNullUserLinkModels()
        {
            var emisClientMock = new Mock<IEmisClient>();
            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object);
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

            var systemUnderTest = new EmisIm1ConnectionService(emisClientMock.Object);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SupplierSystemUnavailable>();
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

            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = alreadyLinkedErrorMessage;

            _mockEmisClient.Setup(x =>x.MeApplicationsPost(It.IsAny<string>(), It.IsAny<MeApplicationsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode
                        .InternalServerError) {ErrorResponse = errorResponse}));

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

            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = accountNotFoundErrorMessage;

            _mockEmisClient.Setup(x =>x.MeApplicationsPost(It.IsAny<string>(), It.IsAny<MeApplicationsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode
                        .InternalServerError) {ErrorResponse = errorResponse}));

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

            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = linkageKeyDoesNotMatchErrorMessage;

            _mockEmisClient.Setup(x =>x.MeApplicationsPost(It.IsAny<string>(), It.IsAny<MeApplicationsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode
                        .InternalServerError) {ErrorResponse = errorResponse}));

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

            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = incorrectSurnameOrDateOfBirthErrorMessage;

            _mockEmisClient.Setup(x =>x.MeApplicationsPost(It.IsAny<string>(), It.IsAny<MeApplicationsPostRequest>()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MeApplicationsPostResponse>(HttpStatusCode
                        .InternalServerError) {ErrorResponse = errorResponse}));

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

        private static void SetupEmisClientMock(
            Mock<IEmisClient> emisClientMock,
            string endUserSessionId = DefaultEndUserSessionId,
            string sessionId = DefaultSessionId,
            string connectionToken = DefaultConnectionToken,
            string odsCode = DefaultOdsCode,
            string userPatientLinkToken = null,
            IEnumerable<UserPatientLink> userPatientLinkModels = null,
            IEnumerable<PatientIdentifier> patientIdentifiers = null
        )
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