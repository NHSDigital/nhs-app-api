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
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Linkage;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Linkage
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
        public async Task GetLinkageKey_ReturnsSuccessfulResponse_WhenSuccessfulResponseFromVision(HttpStatusCode httpStatusCode)
        {
            // Arrange
            var getLinkageKeyResponse = _fixture.Create<LinkageKeyGetResponse>();
            var nhsNumber = _fixture.Create<string>();
            var surname = _fixture.Create<string>();
            var dateOfBirth = _fixture.Create<DateTime>();
            var odsCode = _fixture.Create<string>();

            var mappedResult = new LinkageResponse
            {
                AccountId = "testAccountId",
                LinkageKey = "testLinkageKey",
                OdsCode = odsCode,
            };

            _visionClient
                .Setup(x => x.GetLinkageKey(It.Is<GetLinkageKey>(
                req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                req.OdsCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase))))
                .Returns(Task.FromResult(
                    new VisionLinkageClient.VisionApiObjectResponse<LinkageKeyGetResponse>(httpStatusCode)
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
            var successResult = (LinkageResult.SuccessfullyRetrieved)result;
            successResult.Response.Should().NotBeNull();
            successResult.Response.Should().BeEquivalentTo(mappedResult);
        }

        [TestMethod]
        [DataRow(VisionApiErrorCodes.InvalidNhsNumber, HttpStatusCode.BadRequest,
            typeof(LinkageResult.BadRequestErrorRetrievingNhsUser))]
        [DataRow(null, HttpStatusCode.BadRequest,
            typeof(LinkageResult.BadRequestErrorRetrievingNhsUser))]
        [DataRow(VisionApiErrorCodes.PatientRecordNotFound, HttpStatusCode.NotFound,
            typeof(LinkageResult.NotFoundErrorRetrievingNhsUser))]
        [DataRow(null, HttpStatusCode.NotFound,
            typeof(LinkageResult.NotFoundErrorRetrievingNhsUser))]
        [DataRow(VisionApiErrorCodes.LinkageKeyRevoked, HttpStatusCode.Forbidden,
            typeof(LinkageResult.LinkageKeyRevoked))]
        [DataRow(null, HttpStatusCode.Forbidden,
            typeof(LinkageResult.ForbiddenErrorRetrievingNhsUser))]
        [DataRow(null, HttpStatusCode.InternalServerError,
            typeof(LinkageResult.SupplierSystemUnavailable))]
        public async Task GetLinkageKey_ReturnsNotFoundResponse_WhenNotFoundResponseFromVision(
            string visionApiErrorCode, HttpStatusCode httpStatusCodeResponse, Type expectedResultType)
        {
            // Arrange
            var getLinkageKeyResponse = _fixture.Create<LinkageKeyGetResponse>();
            var nhsNumber = _fixture.Create<string>();
            var surname = _fixture.Create<string>();
            var dateOfBirth = _fixture.Create<DateTime>();
            var odsCode = _fixture.Create<string>();

            _visionClient
                .Setup(x => x.GetLinkageKey(It.Is<GetLinkageKey>(
                req => req.NhsNumber.Equals(nhsNumber, StringComparison.OrdinalIgnoreCase) &&
                req.OdsCode.Equals(odsCode, StringComparison.OrdinalIgnoreCase))))
                .Returns(Task.FromResult(
                    new VisionLinkageClient.VisionApiObjectResponse<LinkageKeyGetResponse>(httpStatusCodeResponse)
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
            // Arrange
            var createLinkageRequest = _fixture.Create<CreateLinkageRequest>();
            var linkageKeyPostResponse = _fixture.Create<LinkageKeyPostResponse>();
            
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
                    new VisionLinkageClient.VisionApiObjectResponse<LinkageKeyPostResponse>(HttpStatusCode.OK)
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
        [DataRow(VisionApiErrorCodes.InvalidNhsNumber, HttpStatusCode.BadRequest,
            typeof(LinkageResult.BadRequestErrorCreatingNhsUser))]
        [DataRow(VisionApiErrorCodes.LinkageKeyRevoked, HttpStatusCode.BadRequest,
            typeof(LinkageResult.AccountStatusInvalid))]
        [DataRow(VisionApiErrorCodes.InvalidNhsNumber, HttpStatusCode.NotFound,
            typeof(LinkageResult.NotFoundErrorCreatingNhsUser))]
        [DataRow(VisionApiErrorCodes.LinkageKeyAlreadyExists, HttpStatusCode.Conflict,
            typeof(LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount))]
        [DataRow(null, HttpStatusCode.BadRequest,
            typeof(LinkageResult.BadRequestErrorCreatingNhsUser))]
        [DataRow(null, HttpStatusCode.NotFound,
            typeof(LinkageResult.NotFoundErrorCreatingNhsUser))]
        [DataRow(null, HttpStatusCode.Conflict,
            typeof(LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount))]
        [DataRow(null, HttpStatusCode.BadGateway,
            typeof(LinkageResult.SupplierSystemUnavailable))]
        public async Task CreateLinkageKey_ReturnsCorrectErrorResponse_WhenvisionRespondsWithError(
            string visionApiErrorCode, HttpStatusCode httpStatusCodeResponse, Type expectedResultType)
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
                new VisionLinkageClient.VisionApiObjectResponse<LinkageKeyPostResponse>(httpStatusCodeResponse);

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
                    new VisionLinkageClient.VisionApiObjectResponse<LinkageKeyPostResponse>(HttpStatusCode.Conflict)))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(createLinkageRequest);

            // Assert
            _visionClient.Verify();
            result.Should().BeAssignableTo<LinkageResult.ErrorCreatingPatientWhoAlreadyHasAnOnlineAccount>();
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
}