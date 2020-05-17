using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Im1Connection;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Linkage
{
    [TestClass]
    public class MicrotestLinkageServiceTests
    {
        private MicrotestLinkageService _systemUnderTest;
        private IFixture _fixture;

        private Mock<IMicrotestClient> _mockMicrotestClient;
        private Mock<IIm1CacheKeyGenerator> _im1CacheKeyGenerator;
        private Mock<IIm1CacheService> _im1CacheService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockMicrotestClient = _fixture.Freeze<Mock<IMicrotestClient>>();
            _systemUnderTest = _fixture.Create<MicrotestLinkageService>();
            _im1CacheKeyGenerator = _fixture.Freeze<Mock<IIm1CacheKeyGenerator>>();

            _im1CacheService = _fixture.Freeze<Mock<IIm1CacheService>>();
        }

        [TestMethod]
        public async Task GetLinkageKey_ReturnsSuccessfulResponse_WhenSuccessfullyRetrievedFromMicrotest()
        {
            // Arrange
            var request = _fixture.Create<GetLinkageRequest>();
            var response = CreateDemographicsResponse();

            _mockMicrotestClient.Setup(x => x.DemographicsGet(request.OdsCode, request.NhsNumber))
                .Returns(Task.FromResult(response));
            var cacheKey = _fixture.Create<string>();

            _im1CacheKeyGenerator.Setup(x => x.GenerateCacheKey(
                    It.IsAny<string>(), request.OdsCode, It.IsAny<string>()))
                .Returns(cacheKey);
            _im1CacheService.Setup(x =>
                x.SaveIm1ConnectionToken(cacheKey, It.IsAny<MicrotestConnectionToken>()));

            // Act
            var result = await _systemUnderTest.GetLinkageKey(request);

            // Assert
            var successResult = result.Should().BeOfType<LinkageResult.SuccessfullyRetrieved>().Subject;
            successResult.Response.Should().NotBeNull();
            successResult.Response.OdsCode.Should().Be(request.OdsCode);
            successResult.Response.AccountId.Should().StartWith("microtest_");
            successResult.Response.LinkageKey.Should().StartWith("microtest_");
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.Forbidden)]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Conflict)]
        [DataRow(HttpStatusCode.Unauthorized)]
        public async Task GetLinkageKey_ReturnsLinkageNotSupportedResponse_WhenDemographicsFailsWithAnyErrorStatusCode(HttpStatusCode demographicsStatusCode)
        {
            // Arrange
            var request = _fixture.Create<GetLinkageRequest>();
            var response = new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(demographicsStatusCode);

            _mockMicrotestClient.Setup(x => x.DemographicsGet(request.OdsCode, request.NhsNumber))
                .Returns(Task.FromResult(response));

            // Act
            var result = await _systemUnderTest.GetLinkageKey(request);

            // Assert
            result.Should().BeOfType<LinkageResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(InternalCode.LinkageKeysNotSupportedBySupplier);
        }

        [DataTestMethod]
        [DataRow(typeof(OperationCanceledException))]
        [DataRow(typeof(TaskCanceledException))]
        [DataRow(typeof(HttpRequestException))]
        [DataRow(typeof(Exception))]
        public async Task GetLinkageKey_ReturnsLinkageNotSupportedResponse_WhenDemographicsFailsWithException(Type demographicsException)
        {
            // Arrange
            var request = _fixture.Create<GetLinkageRequest>();

            _mockMicrotestClient.Setup(x => x.DemographicsGet(request.OdsCode, request.NhsNumber))
                .Throws((Exception)Activator.CreateInstance(demographicsException));

            // Act
            var result = await _systemUnderTest.GetLinkageKey(request);

            // Assert
            result.Should().BeOfType<LinkageResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(InternalCode.LinkageKeysNotSupportedBySupplier);
        }

        [TestMethod]
        public async Task CreateLinkageKey_Returns404()
        {
            // Arrange
            var request = _fixture.Create<CreateLinkageRequest>();

            // Act
            var result = await _systemUnderTest.CreateLinkageKey(request);

            // Assert
            result.Should().BeAssignableTo<LinkageResult.NotFound>();
        }

        private MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse> CreateDemographicsResponse(
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return
                new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(statusCode)
                {
                    Body = _fixture.Create<DemographicsGetResponse>()
                };
        }
    }
}
