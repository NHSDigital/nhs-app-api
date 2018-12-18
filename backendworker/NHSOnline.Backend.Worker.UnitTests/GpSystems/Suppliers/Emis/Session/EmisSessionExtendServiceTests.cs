using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Session
{
    [TestClass]
    public class EmisSessionExtendServiceTests
    {
        private IFixture _fixture;
        private UserSession _userSession;
        private EmisSessionExtendService _systemUnderTest;
        private Mock<IEmisClient> _mockEmisClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _fixture.Create<EmisUserSession>()));

            _userSession = _fixture.Create<UserSession>();

            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();

            _systemUnderTest = _fixture.Create<EmisSessionExtendService>();
        }

        [TestMethod]
        public async Task Extend_WhenClientReturnsSuccess_ReturnsSuccessfullyExtended()
        {    
            // Arrange
            var response = new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK);

            _mockEmisClient.Setup(x => x.DemographicsGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_userSession);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.SuccessfullyExtended>();
            _mockEmisClient.Verify();
        }

        [TestMethod]
        public async Task Extend_WhenClientReturnsError_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var response = new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.BadRequest);

            _mockEmisClient.Setup(x => x.DemographicsGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_userSession);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.SupplierSystemUnavailable>();
            _mockEmisClient.Verify();
        }

        [TestMethod]
        public async Task Extend_WhenClientThrowsHttpRequestException_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockEmisClient.Setup(x => x.DemographicsGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_userSession);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.SupplierSystemUnavailable>();
            _mockEmisClient.Verify();
        }
    }
}

