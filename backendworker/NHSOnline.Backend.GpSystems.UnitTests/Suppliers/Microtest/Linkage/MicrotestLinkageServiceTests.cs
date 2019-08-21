using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Im1Connection;

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
            
            _im1CacheService = _fixture.Freeze<Mock<IIm1CacheService>>();        }

        [TestMethod]
        public async Task GetLinkageKey_ReturnsSuccessfulResponse_WhenSuccessfullyRetrievedFromMicrotest()
        {
            // Arrange
            var request = _fixture.Create<GetLinkageRequest>();
            var response = CreateDemographicsResponse();

            _mockMicrotestClient.Setup(x => x.DemographicsGet(
                    request.OdsCode,
                    request.NhsNumber
                )
            ).Returns(
                Task.FromResult(
                    response
                )
            ).Verifiable();
            var cacheKey = _fixture.Create<string>();
            
            _im1CacheKeyGenerator.Setup(x => x.GenerateCacheKey(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(cacheKey);
            _im1CacheService.Setup(x =>
                x.SaveIm1ConnectionToken(It.IsAny<string>(), It.IsAny<MicrotestConnectionToken>()));

            // Act
            var result = await _systemUnderTest.GetLinkageKey(request);

            // Assert
            var successResult = result.Should().BeAssignableTo<LinkageResult.SuccessfullyRetrieved>().Subject;
            successResult.Response.Should().NotBeNull();
            successResult.Response.OdsCode.Should().Be(request.OdsCode);
        }

        [TestMethod]
        public async Task GetLinkageKey_ReturnsBadGatewayResponse_WhenErrorFromMicrotest()
        {
            // Arrange
            var request = _fixture.Create<GetLinkageRequest>();
            var response = new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.InternalServerError);

            _mockMicrotestClient.Setup(x => x.DemographicsGet(
                    request.OdsCode,
                    request.NhsNumber
                )
            ).Returns(
                Task.FromResult(
                    response
                )
            ).Verifiable();

            // Act
            var result = await _systemUnderTest.GetLinkageKey(request);

            // Assert
            result.Should().BeOfType<LinkageResult.InternalServerError>();
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
