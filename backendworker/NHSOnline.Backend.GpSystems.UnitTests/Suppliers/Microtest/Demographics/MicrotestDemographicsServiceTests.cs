using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Demographics
{
    [TestClass]
    public class MicrotestDemographicsServiceTests
    {
        private IFixture _fixture;
        private IDemographicsService _systemUnderTest;
        private Mock<IMicrotestClient> _mockMicrotestClient;
        private MicrotestUserSession _microtestUserSession;
        private Mock<IMicrotestDemographicsMapper> _mockMicrotestDemographicsMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _microtestUserSession = _fixture.Create<MicrotestUserSession>();
            _mockMicrotestClient = _fixture.Freeze<Mock<IMicrotestClient>>();
            _mockMicrotestDemographicsMapper = _fixture.Freeze<Mock<IMicrotestDemographicsMapper>>();
            _systemUnderTest = _fixture.Create<MicrotestDemographicsService>();
        }

        [TestMethod]
        public async Task GetDemographics_Successful()
        {
            // Arrange
            var response = CreateResponse();
            var mappedResponse = _fixture.Create<DemographicsResponse>();

            SetupDemographicsMapper(response, mappedResponse);
            SetupDemographicsGet()
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await GetDemographics();

            // Assert
            _mockMicrotestClient.Verify();
            _mockMicrotestDemographicsMapper.Verify();

            var demographicsResult = result.Should().BeAssignableTo<DemographicsResult.Success>().Subject;
            demographicsResult.Response.Should().Be(mappedResponse);
        }

        [TestMethod]
        public async Task GetDemographics_UnsuccessfulResponse_ReturnsBadGateway()
        {
            // Arrange
            var response = CreateResponse(HttpStatusCode.BadGateway);
            var mappedResponse = _fixture.Create<DemographicsResponse>();

            SetupDemographicsMapper(response, mappedResponse);
            SetupDemographicsGet()
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await GetDemographics();

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<DemographicsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetDemographics_ForbiddenResponse_ReturnsForbidden()
        {
            // Arrange
            var response = CreateResponse(HttpStatusCode.Forbidden);
            var mappedResponse = _fixture.Create<DemographicsResponse>();

            SetupDemographicsMapper(response, mappedResponse);
            SetupDemographicsGet()
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await GetDemographics();

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<DemographicsResult.Forbidden>();
        }


        [TestMethod]
        public async Task GetDemographics_Exception_ReturnsBadGateway()
        {
            // Arrange
            SetupDemographicsGet()
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await GetDemographics();

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<DemographicsResult.BadGateway>();
        }

        private Task<DemographicsResult> GetDemographics()
        {
            return _systemUnderTest.GetDemographics(_microtestUserSession);
        }

        private void SetupDemographicsMapper(
            MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse> response,
            DemographicsResponse mappedResponse)
        {
            _mockMicrotestDemographicsMapper.Setup(x => x.Map(response.Body))
                .Returns(mappedResponse)
                .Verifiable();
        }

        private ISetup<IMicrotestClient, Task<MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>>>
            SetupDemographicsGet()
        {
            return _mockMicrotestClient.Setup(x =>
                x.DemographicsGet(_microtestUserSession.OdsCode, _microtestUserSession.NhsNumber));
        }

        private MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse> CreateResponse(
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
