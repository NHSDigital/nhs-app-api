using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Session;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Demographics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Session
{
    [TestClass]
    public class MicrotestSessionServiceTests
    {
        private const string ValidConnectionToken = @"{""NhsNumber"": ""123 456 7890""}";

        private IFixture _fixture;
        private ILogger<MicrotestSessionService> _logger;
        private Mock<IMicrotestDemographicsService> _microtestDemographicsService;
        private MicrotestSessionService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<MicrotestSessionService>>();
            _microtestDemographicsService = _fixture.Freeze<Mock<IMicrotestDemographicsService>>();

            _systemUnderTest = new MicrotestSessionService(
                _logger,
                _microtestDemographicsService.Object,
                new MicrotestTokenValidationService(new Mock<ILogger<MicrotestTokenValidationService>>().Object));
        }

        [TestMethod]
        public async Task Create_ValidRequest_ReturnsSuccessResult()
        {
            _microtestDemographicsService.Setup(x => x.GetDemographics(It.IsAny<GpLinkedAccountModel>()))
                .ReturnsAsync(new DemographicsResult.Success(new DemographicsResponse()));

            var result = await _systemUnderTest.Create(ValidConnectionToken, "odsCode", "nhsNumber");

            result
                .Should()
                .BeAssignableTo<GpSessionCreateResult.Success>()
                .Subject
                .UserSession
                .Should()
                .NotBeNull();
        }

        [TestMethod]
        public async Task Create_InvalidDemographicsResult_ReturnsBadGateway()
        {
            _microtestDemographicsService.Setup(x => x.GetDemographics(It.IsAny<GpLinkedAccountModel>()))
                .ReturnsAsync(new DemographicsResult.BadGateway());

            var result = await _systemUnderTest.Create(ValidConnectionToken, "odsCode", "nhsNumber");

            result.Should().BeAssignableTo<GpSessionCreateResult.BadGateway>();
        }
    }
}
