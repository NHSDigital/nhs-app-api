using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Im1Connection;
using NHSOnline.Backend.Support;
using System.Net;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;
using System.Net.Http;
using UnitTestHelper;
using static NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Im1Connection
{
    [TestClass]
    public class MicrotestIm1ConnectionServiceTests
    {
        private const string DefaultConnectionToken = "{\"Im1CacheKey\" : \" test\", \"NhsNumber\" : \"9689170406\"}";
        private const string DefaultOdsCode = "B81603";
        private Mock<IIm1CacheKeyGenerator> _im1CacheKeyGenerator;
        private Mock<IIm1CacheService> _im1CacheService;
        private Mock<ILogger<MicrotestIm1ConnectionService>> _mockLogger;
        private Mock<IMicrotestClient> _microtestClientInterface;

        private IFixture _fixture;
        private MicrotestIm1ConnectionService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _systemUnderTest = _fixture.Create<MicrotestIm1ConnectionService>();

            _im1CacheKeyGenerator = _fixture.Freeze<Mock<IIm1CacheKeyGenerator>>();

            _im1CacheService = _fixture.Freeze<Mock<IIm1CacheService>>();

            _mockLogger = _fixture.Freeze<Mock<ILogger<MicrotestIm1ConnectionService>>>();

            _microtestClientInterface = _fixture.Freeze<Mock<IMicrotestClient>>();
        }

        [TestMethod]
        public async Task Verify_ReturnsAConnection_WhenRequested()
        {
            // Arrange
            var response = new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK);
            _microtestClientInterface.Setup(x => x.DemographicsGet(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var systemUnderTest = CreateSystemUnderTest();
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            var successResult = result.Should().BeAssignableTo<Im1ConnectionVerifyResult.Success>().Subject;

            successResult.Response.ConnectionToken.Should().Be(DefaultConnectionToken);
            successResult.Response.NhsNumbers.Should().NotBeEmpty();
        }

        [TestMethod]
        public async Task Verify_LoggerReturnsCorrectNHSNumberCount()
        {
            // Arrange
            var response = new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK);
            _microtestClientInterface.Setup(x => x.DemographicsGet(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var systemUnderTest = CreateSystemUnderTest();
            await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            const string expectedLogMessage = "Microtest returned 1 NHS Numbers for the user";
            _mockLogger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.Forbidden)]
        [DataRow(HttpStatusCode.InternalServerError)]
        public async Task Verify_ReturnsForbidden_WhenDemographicsFailsWithExpectedStatusCode(
            HttpStatusCode demographicsStatusCode)
        {
            // Arrange
            var response =
                new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(demographicsStatusCode);
            _microtestClientInterface.Setup(x => x.DemographicsGet(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var systemUnderTest = CreateSystemUnderTest();
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.ErrorCase>().Subject.ErrorCode.Should()
                .Be(InternalCode.ErrorRetrievingGivenDemographics);
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.Forbidden)]
        [DataRow(HttpStatusCode.InternalServerError)]
        public async Task Verify_V2_ReturnsUpstreamConnectionError_WhenDemographicsFailsWithExpectedStatusCode(
            HttpStatusCode demographicsStatusCode)
        {
            // Arrange
            var response =
                new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(demographicsStatusCode);
            _microtestClientInterface.Setup(x => x.DemographicsGet(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var systemUnderTest = CreateSystemUnderTest();
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.ErrorCase>().Subject.ErrorCode.Should()
                .Be(InternalCode.ErrorRetrievingGivenDemographics);
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Conflict)]
        [DataRow(HttpStatusCode.Unauthorized)]
        public async Task Verify_ReturnsBadGateway_WhenDemographicsFailsWithUnexpectedErrorCode(
            HttpStatusCode demographicsStatusCode)
        {
            // Arrange
            var response =
                new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(demographicsStatusCode);
            _microtestClientInterface.Setup(x => x.DemographicsGet(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var systemUnderTest = CreateSystemUnderTest();
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.BadGateway>();
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Conflict)]
        [DataRow(HttpStatusCode.Unauthorized)]
        public async Task Verify_V2_ReturnsNotFound_WhenDemographicsFailsWithUnexpectedErrorCode(
            HttpStatusCode demographicsStatusCode)
        {
            // Arrange
            var response =
                new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(demographicsStatusCode);
            _microtestClientInterface.Setup(x => x.DemographicsGet(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var systemUnderTest = CreateSystemUnderTest();
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.BadGateway>();
        }

        [TestMethod]
        public async Task Verify_ReturnsBadGateway_WhenDemographicsResultsInHttpRequestException()
        {
            // Arrange
            var response =
                new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.Forbidden);
            _microtestClientInterface.Setup(x => x.DemographicsGet(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new HttpRequestException("This is a test exception"));

            // Act
            var systemUnderTest = CreateSystemUnderTest();
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.BadGateway>();
        }

        [TestMethod]
        public async Task Verify_ThrowsException_WhenDemographicsResultsInUnexpectedException()
        {
            // Arrange
            var response =
                new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.Forbidden);
            _microtestClientInterface.Setup(x => x.DemographicsGet(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new OperationCanceledException());

            // Act
            var systemUnderTest = CreateSystemUnderTest();
            Func<Task> verifyAction = async () => await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            await verifyAction.Should().ThrowAsync<OperationCanceledException>();
        }

        [TestMethod]
        public async Task Register_SuccessfullyRegistered_WhenDataAreCorrect()
        {
            // Arrange
            var request = _fixture.Create<PatientIm1ConnectionRequest>();
            var cacheKey = _fixture.Create<string>();

            _im1CacheKeyGenerator.Setup(x => x.GenerateCacheKey(request.AccountId, request.OdsCode, request.LinkageKey))
                .Returns(cacheKey);

            var connectionToken = _fixture.Create<MicrotestConnectionToken>();
            _im1CacheService.Setup(x => x.GetIm1ConnectionToken<MicrotestConnectionToken>(cacheKey))
                .Returns(Task.FromResult(Option.Some(connectionToken)));

            // Act
            var systemUnderTest = CreateSystemUnderTest();
            var result = await systemUnderTest.Register(request);

            // Assert
            var successResult = result.Should().BeAssignableTo<Im1ConnectionRegisterResult.Success>().Subject;
            successResult.Response.ConnectionToken.Should().Be(connectionToken.SerializeJson());
        }

        [TestMethod]
        public async Task Register_ReturnsError_WhenDataAreCorrect_CacheDoesntExist()
        {
            // Arrange
            var request = _fixture.Create<PatientIm1ConnectionRequest>();
            var cacheKey = _fixture.Create<string>();


            _im1CacheKeyGenerator.Setup(x => x.GenerateCacheKey(request.AccountId, request.OdsCode, request.LinkageKey))
                .Returns(cacheKey);
            var connectionToken = _fixture.Create<MicrotestConnectionToken>();
            _im1CacheService.Setup(x => x.GetIm1ConnectionToken<MicrotestConnectionToken>(cacheKey))
                .Returns(Task.FromResult(Option.None<MicrotestConnectionToken>()));

            // Act
            var systemUnderTest = CreateSystemUnderTest();
            var result = await systemUnderTest.Register(request);

            // Assert
            result.Should().BeOfType<Im1ConnectionRegisterResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(InternalCode.LinkageKeysNotSupportedBySupplier);
        }

        [TestMethod]
        public async Task Register_ThrowsException_WhenIm1CacheThrowsException()
        {
            // Arrange
            var request = _fixture.Create<PatientIm1ConnectionRequest>();
            var cacheKey = _fixture.Create<string>();

            _im1CacheKeyGenerator.Setup(x => x.GenerateCacheKey(request.AccountId, request.OdsCode, request.LinkageKey))
                .Returns(cacheKey);
            var connectionToken = _fixture.Create<MicrotestConnectionToken>();
            _im1CacheService.Setup(x => x.GetIm1ConnectionToken<MicrotestConnectionToken>(cacheKey))
                .Throws(new OperationCanceledException());

            // Act
            var systemUnderTest = CreateSystemUnderTest();
            Func<Task<Im1ConnectionRegisterResult>>
                registerAction = async () => await systemUnderTest.Register(request);

            // Assert
            await registerAction.Should().ThrowAsync<OperationCanceledException>();
        }

        private MicrotestIm1ConnectionService CreateSystemUnderTest()
        {
            return new MicrotestIm1ConnectionService(_microtestClientInterface.Object, _im1CacheKeyGenerator.Object,
                _im1CacheService.Object, _mockLogger.Object);
        }
    }
}