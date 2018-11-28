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
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Session
{
    [TestClass]
    public class TppSessionExtendServiceTests
    {

        private IFixture _fixture;
        private TppUserSession _userSession;
        private TppSessionExtendService _systemUnderTest;
        private Mock<ITppClient> _mockTppClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _userSession = _fixture.Create<TppUserSession>();

            _mockTppClient = _fixture.Freeze<Mock<ITppClient>>();

            _systemUnderTest = _fixture.Create<TppSessionExtendService>();
        }

        [TestMethod]
        public async Task Extend_WhenClientReturnsSuccess_ReturnsSuccessfullyExtended()
        {    
            // Arrange
            var response = new TppClient.TppApiObjectResponse<PatientSelectedReply>(HttpStatusCode.OK);

            _mockTppClient.Setup(x => x.PatientSelectedPost(_userSession))
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_userSession);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.SuccessfullyExtended>();
            _mockTppClient.Verify();
        }

        [TestMethod]
        public async Task Extend_WhenClientReturnsError_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<PatientSelectedReply>(HttpStatusCode.BadRequest);

            _mockTppClient.Setup(x => x.PatientSelectedPost(_userSession))
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_userSession);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.SupplierSystemUnavailable>();
            _mockTppClient.Verify();
        }

        [TestMethod]
        public async Task Extend_WhenClientThrowsHttpRequestException_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockTppClient.Setup(x => x.PatientSelectedPost(_userSession))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_userSession);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.SupplierSystemUnavailable>();
            _mockTppClient.Verify();
        }
    }
}

