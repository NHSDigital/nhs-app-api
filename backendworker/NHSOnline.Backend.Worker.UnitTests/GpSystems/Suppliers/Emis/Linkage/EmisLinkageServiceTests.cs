using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Verifications;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Linkage
{
    [TestClass]
    public class EmisLinkageServiceTests
    {
        private EmisLinkageService _systemUnderTest;
        private Mock<IEmisClient> _emisClient;
        private Mock<IEmisLinkageMapper> _emisLinkageMapper;
        private Mock<IEmisSessionService> _emisSessionService;
        private IFixture _fixture;
        private Mock<IRegistrationGuidKeyGenerator> _mockRegistrationGuidKeyGenerator;
        private Mock<IRegistrationCacheService> _mockRegistrationCacheService;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _emisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _emisLinkageMapper = _fixture.Freeze<Mock<IEmisLinkageMapper>>();
            _emisSessionService = _fixture.Freeze<Mock<IEmisSessionService>>();
            _mockRegistrationGuidKeyGenerator = _fixture.Freeze<Mock<IRegistrationGuidKeyGenerator>>();
            _mockRegistrationCacheService = _fixture.Freeze<Mock<IRegistrationCacheService>>();
            _systemUnderTest = _fixture.Create<EmisLinkageService>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.Created)]
        public async Task GetLinkageKey_ReturnsSuccessfulResponse_WhenSuccessfulResponseFromEmis(HttpStatusCode httpStatusCode)
        {
            // Arrange
            var addVerificationResponse = _fixture.Create<AddVerificationResponse>();
            var nhsNumber = _fixture.Create<string>();
            var odsCode = _fixture.Create<string>();
            var identityToken = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();

            var endUserSessionResponse = new SessionsEndUserSessionPostResponse
            {
                EndUserSessionId = endUserSessionId,
            };

            _emisSessionService.Setup(x => x.SendSessionsEndUserSessionPost()).ReturnsAsync(endUserSessionResponse);

            _emisClient.Setup(x => x.VerificationPost(It.IsAny<EmisHeaderParameters>(), It.Is<AddVerificationRequest>(
                req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                req.NationalPracticeCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase) &&
                req.Token.Equals(identityToken, StringComparison.OrdinalIgnoreCase))))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<AddVerificationResponse>(httpStatusCode)
                    {
                        Body = addVerificationResponse,
                    }));

            // Act
            var result = await _systemUnderTest.GetLinkageKey(nhsNumber, odsCode, identityToken);

            // Assert
            _emisClient.Verify(x => x.VerificationPost(It.IsAny<EmisHeaderParameters>(), It.Is<AddVerificationRequest>(
                req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                req.NationalPracticeCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase) &&
                req.Token.Equals(identityToken, StringComparison.OrdinalIgnoreCase))));
            _emisLinkageMapper.Verify(x => x.Map(addVerificationResponse));
            result.Should().BeAssignableTo<LinkageResult.SuccessfullyRetrieved>();
            var successResult = (LinkageResult.SuccessfullyRetrieved)result;
            successResult.Response.Should().NotBeNull();
            successResult.Response.OdsCode.Should().Be(odsCode);
        }

        [TestMethod]
        public async Task GetLinkageKey_ReturnsSuccessfulResponse_WhenConflictResponseFromEmis()
        {
            // Arrange
            var addVerificationResponse = _fixture.Create<AddVerificationResponse>();
            var nhsNumber = _fixture.Create<string>();
            var odsCode = _fixture.Create<string>();
            var identityToken = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();

            var endUserSessionResponse = new SessionsEndUserSessionPostResponse
            {
                EndUserSessionId = endUserSessionId,
            };

            _emisSessionService.Setup(x => x.SendSessionsEndUserSessionPost()).ReturnsAsync(endUserSessionResponse);

            _emisClient.Setup(x => x.VerificationPost(It.IsAny<EmisHeaderParameters>(), It.Is<AddVerificationRequest>(
                    req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                           req.NationalPracticeCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase) &&
                           req.Token.Equals(identityToken, StringComparison.OrdinalIgnoreCase))))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<AddVerificationResponse>(HttpStatusCode.Conflict)
                    {
                        Body = addVerificationResponse,
                    }));

            // Act
            var result = await _systemUnderTest.GetLinkageKey(nhsNumber, odsCode, identityToken);

            // Assert
            _emisClient.Verify(x => x.VerificationPost(It.IsAny<EmisHeaderParameters>(), It.Is<AddVerificationRequest>(
                req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                       req.NationalPracticeCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase) &&
                       req.Token.Equals(identityToken, StringComparison.OrdinalIgnoreCase))));
            _emisLinkageMapper.Verify(x => x.Map(addVerificationResponse));
            result.Should().BeAssignableTo<LinkageResult.SuccessfullyRetrievedAlreadyExists>();
            var successResult = (LinkageResult.SuccessfullyRetrievedAlreadyExists) result;
            successResult.Response.Should().NotBeNull();
            successResult.Response.OdsCode.Should().Be(odsCode);
        }

        [TestMethod]
        [DataRow(EmisApiErrorCode.PatientNotRegisteredAtPractice, HttpStatusCode.NotFound, typeof(LinkageResult.PatientNotRegisteredAtPractice))]
        [DataRow(EmisApiErrorCode.NoRegisteredOnlineUserFound, HttpStatusCode.NotFound, typeof(LinkageResult.NoRegisteredOnlineUserFound))]
        [DataRow(null, HttpStatusCode.NotFound, typeof(LinkageResult.NotFoundErrorRetrievingNhsUser))]
        [DataRow(EmisApiErrorCode.PracticeNotLive, HttpStatusCode.BadRequest, typeof(LinkageResult.PracticeNotLive))]
        [DataRow(EmisApiErrorCode.PatientMarkedAsArchived, HttpStatusCode.BadRequest, typeof(LinkageResult.PatientMarkedAsArchived))]
        [DataRow(EmisApiErrorCode.PatientNonCompetentOrUnder16, HttpStatusCode.BadRequest, typeof(LinkageResult.PatientNonCompetentOrUnder16))]
        [DataRow(EmisApiErrorCode.AccountStatusInvalid, HttpStatusCode.BadRequest, typeof(LinkageResult.AccountStatusInvalid))]
        [DataRow(null, HttpStatusCode.BadRequest, typeof(LinkageResult.BadRequestErrorRetrievingNhsUser))]
        public async Task GetLinkageKey_ReturnsCorrectErrorResponse_WhenEmisRespondsWithError(EmisApiErrorCode? emisApiErrorCode, HttpStatusCode httpStatusCodeResponse, Type expectedResultType)
        {
            // Arrange
            var nhsNumber = _fixture.Create<string>();
            var odsCode = _fixture.Create<string>();
            var identityToken = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();

            var endUserSessionResponse = new SessionsEndUserSessionPostResponse
            {
                EndUserSessionId = endUserSessionId,
            };
            
            _emisSessionService.Setup(x => x.SendSessionsEndUserSessionPost()).ReturnsAsync(endUserSessionResponse);
            
            var mockResponse = new EmisClient.EmisApiObjectResponse<AddVerificationResponse>(httpStatusCodeResponse);

            // Add emis specific error code if unit test requires it.
            if (emisApiErrorCode.HasValue)
            {
                mockResponse.StandardErrorResponse = new StandardErrorResponse
                {
                    InternalResponseCode = (int)emisApiErrorCode,
                };
            }

            _emisClient.Setup(x => x.VerificationPost(
                It.Is<EmisHeaderParameters>(
                    header => string.Equals(header.EndUserSessionId, endUserSessionId, StringComparison.Ordinal)),
                It.Is<AddVerificationRequest>(
                    req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                    req.NationalPracticeCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase) &&
                    req.Token.Equals(identityToken, StringComparison.OrdinalIgnoreCase))))
                    .ReturnsAsync(mockResponse);

            // Act
            var result = await _systemUnderTest.GetLinkageKey(nhsNumber, odsCode, identityToken);

            // Assert
            _emisClient.Verify(x => x.VerificationPost(
                It.Is<EmisHeaderParameters>(
                    header => string.Equals(header.EndUserSessionId, endUserSessionId, StringComparison.Ordinal)),
                It.Is<AddVerificationRequest>(
                    req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                    req.NationalPracticeCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase) &&
                    req.Token.Equals(identityToken, StringComparison.OrdinalIgnoreCase))));

            result.GetType().Should().Be(expectedResultType);
        }

        [TestMethod]
        public async Task GetLinkageKey_ReturnsSupplierSystemUnavailable_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            var nhsNumber = _fixture.Create<string>();
            var odsCode = _fixture.Create<string>();
            var identityToken = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();

            var endUserSessionResponse = new SessionsEndUserSessionPostResponse
            {
                EndUserSessionId = endUserSessionId,
            };

            _emisSessionService.Setup(x => x.SendSessionsEndUserSessionPost()).ReturnsAsync(endUserSessionResponse);

            _emisClient.Setup(x => x.VerificationPost(It.IsAny<EmisHeaderParameters>(), It.Is<AddVerificationRequest>(
                req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                req.NationalPracticeCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase) &&
                req.Token.Equals(identityToken, StringComparison.OrdinalIgnoreCase))))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetLinkageKey(nhsNumber, odsCode, identityToken);

            // Assert
            result.Should().BeAssignableTo<LinkageResult.SupplierSystemUnavailable>();
            _emisClient.Verify();
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromEmis()
        {
            // Arrange
            var createLinkageRequest = _fixture.Create<CreateLinkageRequest>();
            var addNhsUserResponse = _fixture.Create<AddNhsUserResponse>();
            var addVerificationResponse = _fixture.Create<AddVerificationResponse>();
            var endUserSessionId = _fixture.Create<string>();
            var endUserSessionResponse = new SessionsEndUserSessionPostResponse
            {
                EndUserSessionId = endUserSessionId,
            };

            _emisSessionService.Setup(x => x.SendSessionsEndUserSessionPost()).ReturnsAsync(endUserSessionResponse);

            _emisClient.Setup(x => x.NhsUserPost(
                It.Is<EmisHeaderParameters>(
                    header => string.Equals(header.EndUserSessionId, endUserSessionId, StringComparison.Ordinal)),
                It.Is<AddNhsUserRequest>(
                    request => request.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal)
                    && request.NationalPracticeCode.Equals(createLinkageRequest.OdsCode, StringComparison.Ordinal))))
                .ReturnsAsync(
                    new EmisClient.EmisApiObjectResponse<AddNhsUserResponse>(HttpStatusCode.OK)
                    {
                        Body = addNhsUserResponse,
                    });

            _emisClient.Setup(x => x.VerificationPost(It.IsAny<EmisHeaderParameters>(), It.Is<AddVerificationRequest>(
                req => req.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.OrdinalIgnoreCase) &&
                req.NationalPracticeCode.Equals(createLinkageRequest.OdsCode, StringComparison.OrdinalIgnoreCase) &&
                req.Token.Equals(createLinkageRequest.IdentityToken, StringComparison.OrdinalIgnoreCase))))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<AddVerificationResponse>(HttpStatusCode.OK)
                    {
                        Body = addVerificationResponse,
                    }))
                    .Verifiable();
            
            const string key = "Key";
            _mockRegistrationGuidKeyGenerator.Setup(x => x.GenerateRegistrationKey(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(key).Verifiable();

            
            _mockRegistrationCacheService.Setup(x => x.CreateRegistrationGuid(key, addNhsUserResponse.AccessIdentityGuid))
                .Returns(Task.FromResult(
                    "Encrypted key"
                )).Verifiable();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(createLinkageRequest);

            // Assert
            _emisClient.Verify();
            _emisLinkageMapper.Verify(x => x.Map(addVerificationResponse));
            result.Should().BeAssignableTo<LinkageResult.SuccessfullyCreated>();
            var successResult = (LinkageResult.SuccessfullyCreated)result;
            successResult.Response.Should().NotBeNull();
            successResult.Response.OdsCode.Should().Be(createLinkageRequest.OdsCode);
            _mockRegistrationGuidKeyGenerator.Verify();
            _mockRegistrationCacheService.Verify(x => x.CreateRegistrationGuid(key, addNhsUserResponse.AccessIdentityGuid));
        }

        [TestMethod]
        [DataRow(EmisApiErrorCode.PatientNotRegisteredAtPractice, HttpStatusCode.NotFound, typeof(LinkageResult.PatientNotRegisteredAtPractice))]
        [DataRow(EmisApiErrorCode.NoRegisteredOnlineUserFound, HttpStatusCode.NotFound, typeof(LinkageResult.NoRegisteredOnlineUserFound))]
        [DataRow(null, HttpStatusCode.NotFound, typeof(LinkageResult.NotFoundErrorCreatingNhsUser))]
        [DataRow(EmisApiErrorCode.PracticeNotLive, HttpStatusCode.BadRequest, typeof(LinkageResult.PracticeNotLive))]
        [DataRow(EmisApiErrorCode.PatientMarkedAsArchived, HttpStatusCode.BadRequest, typeof(LinkageResult.PatientMarkedAsArchived))]
        [DataRow(EmisApiErrorCode.PatientNonCompetentOrUnder16, HttpStatusCode.BadRequest, typeof(LinkageResult.PatientNonCompetentOrUnder16))]
        [DataRow(null, HttpStatusCode.BadRequest, typeof(LinkageResult.BadRequestErrorCreatingNhsUser))]
        public async Task CreateLinkageKey_ReturnsCorrectErrorResponse_WhenEmisRespondsWithError(EmisApiErrorCode? emisApiErrorCode, HttpStatusCode httpStatusCodeResponse, Type expectedResultType)
        {
            // Arrange
            var createLinkageRequest = _fixture.Create<CreateLinkageRequest>();

            var endUserSessionId = _fixture.Create<string>();
            var endUserSessionResponse = new SessionsEndUserSessionPostResponse
            {
                EndUserSessionId = endUserSessionId,
            };

            _emisSessionService.Setup(x => x.SendSessionsEndUserSessionPost()).ReturnsAsync(endUserSessionResponse);

            var mockResponse = new EmisClient.EmisApiObjectResponse<AddNhsUserResponse>(httpStatusCodeResponse);

            // Add emis specific error code if unit test requires it.
            if (emisApiErrorCode.HasValue)
            {
                mockResponse.StandardErrorResponse = new StandardErrorResponse
                {
                    InternalResponseCode = (int)emisApiErrorCode,
                };
            }

            _emisClient.Setup(x => x.NhsUserPost(
                It.Is<EmisHeaderParameters>(
                    header => string.Equals(header.EndUserSessionId, endUserSessionId, StringComparison.Ordinal)),
                It.Is<AddNhsUserRequest>(
                    request => request.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal)
                    && request.NationalPracticeCode.Equals(createLinkageRequest.OdsCode, StringComparison.Ordinal))))
                    .ReturnsAsync(mockResponse)
                    .Verifiable();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(createLinkageRequest);

            // Assert
            _emisClient.Verify();
            result.GetType().Should().Be(expectedResultType);
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsLinkageKeyAlreadyExists_WhenEmisRespondsWith409()
        {
            // Arrange
            var createLinkageRequest = _fixture.Create<CreateLinkageRequest>();

            var endUserSessionId = _fixture.Create<string>();
            var endUserSessionResponse = new SessionsEndUserSessionPostResponse
            {
                EndUserSessionId = endUserSessionId,
            };

            _emisSessionService.Setup(x => x.SendSessionsEndUserSessionPost()).ReturnsAsync(endUserSessionResponse);

            _emisClient.Setup(x => x.NhsUserPost(
                It.Is<EmisHeaderParameters>(
                    header => string.Equals(header.EndUserSessionId, endUserSessionId, StringComparison.Ordinal)),
                It.Is<AddNhsUserRequest>(
                    request => request.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal)
                    && request.NationalPracticeCode.Equals(createLinkageRequest.OdsCode, StringComparison.Ordinal))))
                .ReturnsAsync(
                    new EmisClient.EmisApiObjectResponse<AddNhsUserResponse>(HttpStatusCode.Conflict))
                    .Verifiable();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(createLinkageRequest);

            // Assert
            _emisClient.Verify();
            result.Should().BeAssignableTo<LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount>();
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsSupplierSystemUnavailable_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            var createLinkageRequest = _fixture.Create<CreateLinkageRequest>();

            var endUserSessionId = _fixture.Create<string>();
            var endUserSessionResponse = new SessionsEndUserSessionPostResponse
            {
                EndUserSessionId = endUserSessionId,
            };

            _emisSessionService.Setup(x => x.SendSessionsEndUserSessionPost()).ReturnsAsync(endUserSessionResponse);

            _emisClient.Setup(x => x.NhsUserPost(
                It.Is<EmisHeaderParameters>(
                    header => string.Equals(header.EndUserSessionId, endUserSessionId, StringComparison.Ordinal)),
                It.Is<AddNhsUserRequest>(
                    request => request.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal)
                    && request.NationalPracticeCode.Equals(createLinkageRequest.OdsCode, StringComparison.Ordinal))))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(createLinkageRequest);

            // Assert
            result.Should().BeAssignableTo<LinkageResult.SupplierSystemUnavailable>();
            _emisClient.Verify();
        }
    }
}
