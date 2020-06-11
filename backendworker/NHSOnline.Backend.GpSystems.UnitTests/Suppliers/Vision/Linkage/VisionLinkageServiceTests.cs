using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Castle.Core.Internal;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Linkage
{
    [TestClass]
    public class VisionLinkageServiceTests
    {
        private VisionLinkageService _systemUnderTest;
        private Mock<IVisionClient> _visionClient;
        private Mock<IVisionLinkageMapper> _visionLinkageMapper;
        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _visionClient = _fixture.Freeze<Mock<IVisionClient>>();
            _visionLinkageMapper = _fixture.Freeze<Mock<IVisionLinkageMapper>>();
            _systemUnderTest = _fixture.Create<VisionLinkageService>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.OK)]
        public async Task GetLinkageKey_ReturnsSuccessfulResponse_WhenSuccessfulResponseFromVision(
            HttpStatusCode httpStatusCode)
        {
            // Arrange
            var accountId = _fixture.Create<string>();
            var nhsNumber = _fixture.Create<string>();
            var surname = _fixture.Create<string>();
            var dateOfBirth = _fixture.Create<DateTime>();
            var odsCode = _fixture.Create<string>();
            var apiKey = _fixture.Create<string>();
            var linkageKey = _fixture.Create<string>();

            var getLinkageKeyResponse = new LinkageKeyGetResponse
            {
                AccountId = accountId,
                ApiKey = apiKey,
                DateOfBirth = DateTimeExtensions.FormatToYYYYMMDD(dateOfBirth),
                LinkageKey = linkageKey,
                OdsCode = odsCode,
                Surname = surname,
            };

            var mappedResult = new LinkageResponse
            {
                AccountId = accountId,
                LinkageKey = linkageKey,
                OdsCode = odsCode,
            };

            _visionClient
                .Setup(x => x.GetLinkageKey(It.Is<GetLinkageKey>(
                    req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                           req.OdsCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase))))
                .Returns(Task.FromResult(
                    new VisionLinkageApiObjectResponse<LinkageKeyGetResponse>(httpStatusCode)
                    {
                        Body = getLinkageKeyResponse,
                    }));

            var request = CreateGetLinkageRequest(nhsNumber, surname, dateOfBirth, odsCode);
            _visionLinkageMapper.Setup(x => x.Map(getLinkageKeyResponse)).Returns(mappedResult);

            // Act
            var result = await _systemUnderTest.GetLinkageKey(request);

            // Assert
            _visionClient.Verify(x => x.GetLinkageKey(It.Is<GetLinkageKey>(
                req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                       req.OdsCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase))));
            _visionLinkageMapper.Verify(x => x.Map(getLinkageKeyResponse));
            result.Should().BeAssignableTo<LinkageResult.SuccessfullyRetrieved>();
            var successResult = (LinkageResult.SuccessfullyRetrieved) result;
            successResult.Response.Should().NotBeNull();
            successResult.Response.Should().BeEquivalentTo(mappedResult);
        }

        [TestMethod]
        [DataRow("V4205", HttpStatusCode.BadRequest, Im1ConnectionErrorCodes.InternalCode.InvalidNhsNumber)]
        [DataRow("VY806", HttpStatusCode.NotFound, Im1ConnectionErrorCodes.InternalCode.PatientRecordNotFound)]
        public async Task GetLinkageKey_ReturnsNotFoundResponse_WhenNotFoundResponseFromVision(
            string visionApiErrorCode, HttpStatusCode httpStatusCodeResponse,
            Im1ConnectionErrorCodes.InternalCode expectedCode)
        {
            var result = await GetLinkageKey(visionApiErrorCode, httpStatusCodeResponse,
                typeof(LinkageResult.ErrorCase));

            var errorCase = (LinkageResult.ErrorCase) result;
            errorCase.ErrorCode.Should().Be(expectedCode);
        }

        [TestMethod]
        [DataRow(null, HttpStatusCode.BadRequest)]
        [DataRow(null, HttpStatusCode.NotFound)]
        [DataRow(null, HttpStatusCode.Forbidden)]
        [DataRow(null, HttpStatusCode.InternalServerError)]
        public async Task GetLinkageKey_ReturnCorrectError_WhenNoVisionErrorCode(
            string visionApiErrorCode, HttpStatusCode httpStatusCodeResponse)
        {
            await GetLinkageKey(visionApiErrorCode, httpStatusCodeResponse, typeof(LinkageResult.UnmappedErrorWithStatusCode));
        }

        public async Task<LinkageResult> GetLinkageKey(
            string visionApiErrorCode, HttpStatusCode httpStatusCodeResponse, Type expectedResultType)
        {
            // Arrange
            var nhsNumber = _fixture.Create<string>();
            var surname = _fixture.Create<string>();
            var dateOfBirth = _fixture.Create<DateTime>();
            var odsCode = _fixture.Create<string>();

            _visionClient
                .Setup(x => x.GetLinkageKey(It.Is<GetLinkageKey>(
                    req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                           req.OdsCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase))))
                .Returns(Task.FromResult(
                    new VisionLinkageApiObjectResponse<LinkageKeyGetResponse>(httpStatusCodeResponse)
                    {
                        ErrorResponse = new ErrorResponse
                        {
                            Code = visionApiErrorCode,
                        },
                    }))
                .Verifiable();

            var request = CreateGetLinkageRequest(nhsNumber, surname, dateOfBirth, odsCode);

            // Act
            var result = await _systemUnderTest.GetLinkageKey(request);

            // Assert
            _visionClient.Verify(x => x.GetLinkageKey(It.Is<GetLinkageKey>(
                req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                       req.OdsCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase))));
            _visionLinkageMapper.VerifyNoOtherCalls();
            result.GetType().Should().Be(expectedResultType);
            result.Should().NotBeNull();
            return result;
        }

        private GetLinkageRequest CreateGetLinkageRequest(
            string nhsNumber,
            string surname,
            DateTime dateOfBirth,
            string odsCode)
        {
            var getLinkageRequest = new GetLinkageRequest
            {
                NhsNumber = nhsNumber,
                Surname = surname,
                DateOfBirth = dateOfBirth,
                OdsCode = odsCode,
            };

            return getLinkageRequest;
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsSuccessfulResponseForHappyPath_WhenSuccessfulResponseFromVision()
        {
            var accountId = _fixture.Create<string>();
            var nhsNumber = _fixture.Create<string>();
            var surname = _fixture.Create<string>();
            var dateOfBirth = _fixture.Create<DateTime>();
            var odsCode = _fixture.Create<string>();
            var apiKey = _fixture.Create<string>();
            var linkageKey = _fixture.Create<string>();
            var email = _fixture.Create<string>();
            var identityToken = _fixture.Create<string>();

            // Arrange
            var createLinkageRequest = new CreateLinkageRequest
            {
                OdsCode = odsCode,
                Surname = surname,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                IdentityToken = identityToken,
                NhsNumber = nhsNumber,
            };

            var linkageKeyPostResponse = new LinkageKeyPostResponse
            {
                AccountId = accountId,
                ApiKey = apiKey,
                LinkageKey = linkageKey,
                OdsCode = odsCode,
            };

            _visionLinkageMapper
                .Setup(x =>
                    x.Map(It.IsAny<LinkageKeyPostResponse>()))
                .Returns(new LinkageResponse
                {
                    OdsCode = odsCode,
                    AccountId = accountId,
                    LinkageKey = linkageKey,
                });

            _visionClient
                .Setup(x => x.CreateLinkageKey(
                    It.Is<CreateLinkageKey>(
                        request => request.LinkageKeyPostRequest.NhsNumber.Equals(nhsNumber,
                                       StringComparison.Ordinal)
                                   && request.OdsCode.Equals(odsCode, StringComparison.Ordinal)
                                   && request.LinkageKeyPostRequest.DateOfBirth
                                       .Equals(DateTimeExtensions.FormatToYYYYMMDD(dateOfBirth),
                                           StringComparison.Ordinal)
                                   && request.LinkageKeyPostRequest.LastName.Equals(surname,
                                       StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new VisionLinkageApiObjectResponse<LinkageKeyPostResponse>(HttpStatusCode.OK)
                    {
                        Body = linkageKeyPostResponse,
                    }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(createLinkageRequest);

            // Assert
            _visionClient.Verify();
            _visionLinkageMapper.Verify(x => x.Map(linkageKeyPostResponse));
            result.Should().BeAssignableTo<LinkageResult.SuccessfullyCreated>();
            var successResult = (LinkageResult.SuccessfullyCreated) result;
            successResult.Response.Should().NotBeNull();
            successResult.Response.OdsCode.Should().Be(createLinkageRequest.OdsCode);
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsCorrectErrorResponse_WhenvisionRespondsWithConflict()
        {
            var result = await CreateLinkageKey("V2214", HttpStatusCode.Conflict,
                typeof(LinkageResult.ErrorCase));
            var errorCaseResult = (LinkageResult.ErrorCase) result;
            errorCaseResult.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.LinkageKeyAlreadyExists);
        }

        [TestMethod]
        [DataRow(null, HttpStatusCode.BadRequest)]
        [DataRow(null, HttpStatusCode.NotFound)]
        [DataRow(null, HttpStatusCode.BadGateway)]
        [DataRow(null, HttpStatusCode.Conflict)]
        [DataRow("test", HttpStatusCode.BadRequest)]
        public async Task CreateLinkageKey_ReturnsCorrectErrorCaseResponse_WhenVisionRespondsWithUnknownError(
            string visionApiErrorCode, HttpStatusCode httpStatusCodeResponse)
        {
            await CreateLinkageKey(visionApiErrorCode, httpStatusCodeResponse,
                typeof(LinkageResult.UnmappedErrorWithStatusCode));
        }

        private async Task<LinkageResult> CreateLinkageKey(string visionApiErrorCode,
            HttpStatusCode httpStatusCodeResponse, Type expectedResultType)
        {
            // Arrange
            var createLinkageRequest = _fixture.Create<CreateLinkageRequest>();

            _visionLinkageMapper
                .Setup(x =>
                    x.Map(It.IsAny<LinkageKeyPostResponse>()))
                .Returns(new LinkageResponse
                {
                    OdsCode = createLinkageRequest.OdsCode,

                });

            var mockResponse =
                new VisionLinkageApiObjectResponse<LinkageKeyPostResponse>(httpStatusCodeResponse);

            // Add vision specific error code if unit test requires it.
            if (!visionApiErrorCode.IsNullOrEmpty())
            {
                mockResponse.ErrorResponse = new ErrorResponse
                {
                    Code = visionApiErrorCode,
                };
            }

            _visionClient
                .Setup(x => x.CreateLinkageKey(
                    It.Is<CreateLinkageKey>(
                        request => request.LinkageKeyPostRequest.NhsNumber.Equals(createLinkageRequest.NhsNumber,
                                       StringComparison.Ordinal)
                                   && request.OdsCode.Equals(createLinkageRequest.OdsCode, StringComparison.Ordinal)
                                   && request.LinkageKeyPostRequest.DateOfBirth
                                       .Equals(createLinkageRequest.DateOfBirth.FormatToYYYYMMDD(),
                                           StringComparison.Ordinal)
                                   && request.LinkageKeyPostRequest.LastName.Equals(createLinkageRequest.Surname,
                                       StringComparison.Ordinal))))
                .Returns(Task.FromResult(mockResponse))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(createLinkageRequest);

            // Assert
            _visionClient.Verify();
            result.GetType().Should().Be(expectedResultType);
            
            return result;
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsLinkageKeyAlreadyExists_WhenVisionRespondsWith409()
        {
            // Arrange
            var createLinkageRequest = _fixture.Create<CreateLinkageRequest>();

            _visionLinkageMapper
                .Setup(x =>
                    x.Map(It.IsAny<LinkageKeyPostResponse>()))
                .Returns(new LinkageResponse
                {
                    OdsCode = createLinkageRequest.OdsCode,

                });

            _visionClient
                .Setup(x => x.CreateLinkageKey(
                    It.Is<CreateLinkageKey>(
                        request => request.LinkageKeyPostRequest.NhsNumber.Equals(createLinkageRequest.NhsNumber,
                                       StringComparison.Ordinal)
                                   && request.OdsCode.Equals(createLinkageRequest.OdsCode, StringComparison.Ordinal)
                                   && request.LinkageKeyPostRequest.DateOfBirth
                                       .Equals(createLinkageRequest.DateOfBirth.FormatToYYYYMMDD(),
                                           StringComparison.Ordinal)
                                   && request.LinkageKeyPostRequest.LastName.Equals(createLinkageRequest.Surname,
                                       StringComparison.Ordinal))))
                .Returns(Task.FromResult(
                    new VisionLinkageApiObjectResponse<LinkageKeyPostResponse>(HttpStatusCode.Conflict)))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(createLinkageRequest);

            // Assert
            _visionClient.Verify();
            result.Should().BeAssignableTo<LinkageResult.UnmappedErrorWithStatusCode>();
            var errorResult = (LinkageResult.UnmappedErrorWithStatusCode) result;
            errorResult.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UnknownError);
        }

        [TestMethod]
        public async Task CreateLinkageKey_ReturnsSupplierSystemUnavailable_WhenHttpExceptionOccursCallingVision()
        {
            // Arrange
            var createLinkageRequest = _fixture.Create<CreateLinkageRequest>();

            _visionLinkageMapper
                .Setup(x =>
                    x.Map(It.IsAny<LinkageKeyPostResponse>()))
                .Returns(new LinkageResponse
                {
                    OdsCode = createLinkageRequest.OdsCode,

                });

            _visionClient
                .Setup(x => x.CreateLinkageKey(
                    It.Is<CreateLinkageKey>(
                        request => request.LinkageKeyPostRequest.NhsNumber.Equals(createLinkageRequest.NhsNumber,
                                       StringComparison.Ordinal)
                                   && request.OdsCode.Equals(createLinkageRequest.OdsCode, StringComparison.Ordinal)
                                   && request.LinkageKeyPostRequest.DateOfBirth
                                       .Equals(createLinkageRequest.DateOfBirth.FormatToYYYYMMDD(),
                                           StringComparison.Ordinal)
                                   && request.LinkageKeyPostRequest.LastName.Equals(createLinkageRequest.Surname,
                                       StringComparison.Ordinal))))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(createLinkageRequest);

            // Assert
            result.Should().BeAssignableTo<LinkageResult.SupplierSystemUnavailable>();
            _visionClient.Verify();
        }
    }
};