using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Session;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Session
{
    [TestClass]
    public class EmisSessionExtendServiceTests
    {
        private IFixture _fixture;
        private EmisSessionExtendService _systemUnderTest;
        private Mock<IEmisClient> _mockEmisClient;
        private GpUserSession _gpUserSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _gpUserSession = _fixture.Create<EmisUserSession>();
            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();

            _systemUnderTest = _fixture.Create<EmisSessionExtendService>();
        }

        [TestMethod]
        public async Task Extend_WhenClientReturnsSuccess_ReturnsSuccess()
        {    
            // Arrange
            var response = new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK);

            _mockEmisClient.Setup(x => x.DemographicsGet(It.IsAny<EmisRequestParameters>()))
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_gpUserSession);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.Success>();
            _mockEmisClient.Verify();
        }

        [TestMethod]
        public async Task Extend_WhenClientReturnsError_ReturnsBadGateway()
        {
            // Arrange
            var response = new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.BadRequest);

            _mockEmisClient.Setup(x => x.DemographicsGet(It.IsAny<EmisRequestParameters>()))
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_gpUserSession);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.BadGateway>();
            _mockEmisClient.Verify();
        }

        [TestMethod]
        public async Task Extend_WhenClientThrowsHttpRequestException_ReturnsBadGateway()
        {
            // Arrange
            _mockEmisClient.Setup(x => x.DemographicsGet(It.IsAny<EmisRequestParameters>()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_gpUserSession);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.BadGateway>();
            _mockEmisClient.Verify();
        }
    }
}

