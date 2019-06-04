using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.CidApi.Areas.Linkage;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Linkage
{
    [TestClass]
    public class EmisLinkageServiceTests
    {
        private EmisLinkageService _systemUnderTest;
        private Mock<IEmisClient> _emisClient;
        private Mock<IEmisLinkageMapper> _emisLinkageMapper;
        private Mock<IEmisSessionService> _emisSessionService;
        private IFixture _fixture;
        private Mock<IIm1CacheKeyGenerator> _mockIm1CacheKeyGenerator;
        private Mock<IIm1CacheService> _mockIm1CacheService;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _emisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _emisLinkageMapper = _fixture.Freeze<Mock<IEmisLinkageMapper>>();
            _emisSessionService = _fixture.Freeze<Mock<IEmisSessionService>>();
            _mockIm1CacheKeyGenerator = _fixture.Freeze<Mock<IIm1CacheKeyGenerator>>();
            _mockIm1CacheService = _fixture.Freeze<Mock<IIm1CacheService>>();
            _systemUnderTest = _fixture.Create<EmisLinkageService>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.Created)]
        public async Task GetLinkageKey_ReturnsSuccessfulResponse_WhenSuccessfulResponseFromEmis(HttpStatusCode httpStatusCode)
        {
            // Arrange
            var addVerificationResponse = _fixture.Create<AddVerificationResponse>();
            var nhsNumber = _fixture.Create<string>();
            var surname = _fixture.Create<string>();
            var dateOfBirth = _fixture.Create<DateTime>();
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

            var request = CreateGetLinkageRequest(nhsNumber, surname, dateOfBirth, odsCode,identityToken);

            // Act
            var result = await _systemUnderTest.GetLinkageKey(request);

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
            var surname = _fixture.Create<string>();
            var dateOfBirth = _fixture.Create<DateTime>();
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
            
            var request = CreateGetLinkageRequest(nhsNumber, surname, dateOfBirth, odsCode,identityToken);

            // Act
            var result = await _systemUnderTest.GetLinkageKey(request);

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
        [DataRow(-1551, HttpStatusCode.NotFound,
            Im1ConnectionErrorCodes.Code.PatientNotRegisteredAtThisPractice)]
        [DataRow(-1104, HttpStatusCode.NotFound,
            Im1ConnectionErrorCodes.Code.NoSelfAssociatedUserExistWithThisPatient)]
        [DataRow(1401, HttpStatusCode.BadRequest, Im1ConnectionErrorCodes.Code.PracticeNotLive)]
        [DataRow(1552, HttpStatusCode.BadRequest, Im1ConnectionErrorCodes.Code.PatientArchived)]
        [DataRow(1553, HttpStatusCode.BadRequest,
            Im1ConnectionErrorCodes.Code.UnderMinimumAgeOrNonCompetent)]
        [DataRow(1107, HttpStatusCode.BadRequest,
            Im1ConnectionErrorCodes.Code.UserSelfAssociatedAccountIsArchived)]
        public async Task GetLinkageKey_ReturnsCorrectErrorResponse_WhenEmisRespondsWithError(int? emisApiErrorCode,
            HttpStatusCode httpStatusCodeResponse, Im1ConnectionErrorCodes.Code code)
        {
            // Act
            var result = await GetLinkageKey(emisApiErrorCode, httpStatusCodeResponse);

            // Assert
            result.GetType().Should().Be(typeof(LinkageResult.ErrorCase));
            var errorResult = (LinkageResult.ErrorCase) result;
            errorResult.ErrorCode.Should().Be(code);
        }


        [TestMethod]
        public async Task GetLinkageKey_ReturnsCorrectErrorResponse_WhenEmisRespondsWithBadRequestErrorCode()
        {
            // Act
            var result = await GetLinkageKey(null, HttpStatusCode.BadRequest);

            // Assert
            result.GetType().Should().Be(typeof(LinkageResult.BadRequest));
            var errorResult = (LinkageResult.BadRequest)result;
            errorResult.ErrorCode.Should().Be(Im1ConnectionErrorCodes.Code.UnknownError);
        }

        [TestMethod]
        public async Task GetLinkageKey_ReturnsCorrectErrorResponse_WhenEmisRespondsWithNotFoundErrorCode()
        {
            // Act
            var result = await GetLinkageKey(null, HttpStatusCode.NotFound);

            // Assert
            result.GetType().Should().Be(typeof(LinkageResult.NotFound));
            var errorResult = (LinkageResult.NotFound)result;
            errorResult.ErrorCode.Should().Be(Im1ConnectionErrorCodes.Code.UnknownError);
        }

        private async Task<LinkageResult> GetLinkageKey(int? emisApiErrorCode, HttpStatusCode httpStatusCodeResponse)
        {
            // Arrange
            var nhsNumber = _fixture.Create<string>();
            var odsCode = _fixture.Create<string>();
            var surname = _fixture.Create<string>();
            var dateOfBirth = _fixture.Create<DateTime>();
            var identityToken = _fixture.Create<string>();
            var endUserSessionId = _fixture.Create<string>();

            var endUserSessionResponse = new SessionsEndUserSessionPostResponse
            {
                EndUserSessionId = endUserSessionId,
            };

            _emisSessionService.Setup(x => x.SendSessionsEndUserSessionPost()).ReturnsAsync(endUserSessionResponse);

            var mockResponse = new EmisClient.EmisApiObjectResponse<AddVerificationResponse>(httpStatusCodeResponse);

            mockResponse.StandardErrorResponse = new StandardErrorResponse
            {
                InternalResponseCode = emisApiErrorCode ?? 0,
            };


            _emisClient.Setup(x => x.VerificationPost(
                    It.Is<EmisHeaderParameters>(
                        header => string.Equals(header.EndUserSessionId, endUserSessionId, StringComparison.Ordinal)),
                    It.Is<AddVerificationRequest>(
                        req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                               req.NationalPracticeCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase) &&
                               req.Token.Equals(identityToken, StringComparison.OrdinalIgnoreCase))))
                .ReturnsAsync(mockResponse);

            var request = CreateGetLinkageRequest(nhsNumber, surname, dateOfBirth, odsCode, identityToken);

            // Act
            var result = await _systemUnderTest.GetLinkageKey(request);

            // Assert
            _emisClient.Verify(x => x.VerificationPost(
                It.Is<EmisHeaderParameters>(
                    header => string.Equals(header.EndUserSessionId, endUserSessionId, StringComparison.Ordinal)),
                It.Is<AddVerificationRequest>(
                    req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                           req.NationalPracticeCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase) &&
                           req.Token.Equals(identityToken, StringComparison.OrdinalIgnoreCase))));

            return result;
        }

        [TestMethod]
        public async Task GetLinkageKey_ReturnsSupplierSystemUnavailable_WhenHttpExceptionOccursCallingEmis()
        {
            // Arrange
            var nhsNumber = _fixture.Create<string>();
            var surname = _fixture.Create<string>();
            var dateOfBirth = _fixture.Create<DateTime>();
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
            
            var request = CreateGetLinkageRequest(nhsNumber, surname, dateOfBirth, odsCode,identityToken);

            // Act
            var result = await _systemUnderTest.GetLinkageKey(request);

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
            _mockIm1CacheKeyGenerator.Setup(x => x.GenerateCacheKey(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(key).Verifiable();

            _mockIm1CacheService
                .Setup(x => x.SaveIm1ConnectionToken(key,
                    It.Is<EmisConnectionToken>(ct => ct.Im1CacheKey.Equals(key,
                                                         StringComparison.Ordinal) &&
                                                     ct.AccessIdentityGuid.Equals(
                                                         addNhsUserResponse.AccessIdentityGuid.ToString(),
                                                         StringComparison.Ordinal))))
                .Returns(Task.FromResult("Encrypted key"))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(createLinkageRequest);

            // Assert
            _emisClient.Verify();
            _emisLinkageMapper.Verify(x => x.Map(addVerificationResponse));
            result.Should().BeAssignableTo<LinkageResult.SuccessfullyCreated>();
            var successResult = (LinkageResult.SuccessfullyCreated)result;
            successResult.Response.Should().NotBeNull();
            successResult.Response.OdsCode.Should().Be(createLinkageRequest.OdsCode);
            _mockIm1CacheKeyGenerator.Verify();
            _mockIm1CacheService.Verify();
        }



       [TestMethod]
       [DataRow(1551, HttpStatusCode.NotFound, Im1ConnectionErrorCodes.Code.PatientNotRegisteredAtThisPractice)]
       [DataRow(1553, HttpStatusCode.BadRequest, Im1ConnectionErrorCodes.Code.UnderMinimumAgeOrNonCompetent)]
       public async Task CreateLinkageKey_ReturnsCorrectErrorResponse_WhenEmisRespondsWithError(int emisApiErrorCode,
           HttpStatusCode httpStatusCodeResponse,
           Im1ConnectionErrorCodes.Code expectedCode)
       {
           // Act
           var result = await CreateLinkageKey(emisApiErrorCode, httpStatusCodeResponse);

           // Assert
            _emisClient.Verify();
           result.GetType().Should().Be(typeof(LinkageResult.ErrorCase));
           var errorCase = (LinkageResult.ErrorCase) result;
           errorCase.ErrorCode.Should().Be(expectedCode);
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsCorrectErrorResponse_WhenEmisRespondsWithNotFoundError()
        {
            // Act
            var result = await CreateLinkageKey(0, HttpStatusCode.NotFound);

            // Assert
            _emisClient.Verify();
            result.GetType().Should().Be(typeof(LinkageResult.NotFound));
            var errorCase = (LinkageResult.NotFound)result;
            errorCase.ErrorCode.Should().Be(Im1ConnectionErrorCodes.Code.UnknownError);
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsCorrectErrorResponse_WhenEmisRespondsWithBadRequestError()
        {
            // Act
            var result = await CreateLinkageKey(0, HttpStatusCode.BadRequest);

            // Assert
            _emisClient.Verify();
            result.GetType().Should().Be(typeof(LinkageResult.BadRequest));
            var errorCase = (LinkageResult.BadRequest)result;
            errorCase.ErrorCode.Should().Be(Im1ConnectionErrorCodes.Code.UnknownError);
        }

        public async Task<LinkageResult> CreateLinkageKey(int emisApiErrorCode,HttpStatusCode httpStatusCodeResponse)
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
            mockResponse.StandardErrorResponse = new StandardErrorResponse
            {
                InternalResponseCode = (int)emisApiErrorCode,
            };


            _emisClient.Setup(x => x.NhsUserPost(
                It.Is<EmisHeaderParameters>(
                    header => string.Equals(header.EndUserSessionId, endUserSessionId, StringComparison.Ordinal)),
                It.Is<AddNhsUserRequest>(
                    request => request.NhsNumber.Equals(createLinkageRequest.NhsNumber, StringComparison.Ordinal)
                    && request.NationalPracticeCode.Equals(createLinkageRequest.OdsCode, StringComparison.Ordinal))))
                    .ReturnsAsync(mockResponse)
                    .Verifiable();
            
            return await _systemUnderTest.CreateLinkageKey(createLinkageRequest);
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
           result.Should().BeAssignableTo<LinkageResult.Conflict>();
           var errorCode = (LinkageResult.Conflict) result;
           errorCode.ErrorCode.Should().Be(Im1ConnectionErrorCodes.Code.UnknownError);
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

        private GetLinkageRequest CreateGetLinkageRequest(string nhsNumber, string surname, DateTime dateOfBirth,
            string odsCode, string identityToken)
        {
            var getLinkageRequest = new GetLinkageRequest()
            {
                NhsNumber = nhsNumber,
                Surname = surname,
                DateOfBirth = dateOfBirth,
                OdsCode = odsCode,
                IdentityToken = identityToken
            };

            return getLinkageRequest;
        }
    }
}
