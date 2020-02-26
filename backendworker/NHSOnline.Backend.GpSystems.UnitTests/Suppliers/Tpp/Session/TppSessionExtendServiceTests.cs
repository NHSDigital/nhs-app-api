using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Session
{
    [TestClass]
    public class TppSessionExtendServiceTests
    {
        private IFixture _fixture;
        private TppUserSession _tppUserSession;
        private TppSessionExtendService _systemUnderTest;
        private Mock<ITppClientRequest<TppUserSession, PatientSelectedReply>> _mockPatientSelected;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _tppUserSession = _fixture.Create<TppUserSession>();
            _mockPatientSelected = _fixture.Freeze<Mock<ITppClientRequest<TppUserSession, PatientSelectedReply>>>();
            _systemUnderTest = _fixture.Create<TppSessionExtendService>();
        }

        [TestMethod]
        public async Task Extend_WhenClientReturnsSuccess_ReturnsSuccess()
        {
            // Arrange
            var response = new TppApiObjectResponse<PatientSelectedReply>(HttpStatusCode.OK);

            _mockPatientSelected.Setup(x => x.Post(_tppUserSession))
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_tppUserSession);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.Success>();
            _mockPatientSelected.Verify();
        }

        [TestMethod]
        public async Task Extend_WhenClientReturnsError_ReturnsBadGateway()
        {
            // Arrange
            var response = new TppApiObjectResponse<PatientSelectedReply>(HttpStatusCode.BadRequest);

            _mockPatientSelected.Setup(x => x.Post(_tppUserSession))
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_tppUserSession);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.BadGateway>();
            _mockPatientSelected.Verify();
        }

        [TestMethod]
        public async Task Extend_WhenClientThrowsHttpRequestException_ReturnsBadGateway()
        {
            // Arrange
            _mockPatientSelected.Setup(x => x.Post(_tppUserSession))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_tppUserSession);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.BadGateway>();
            _mockPatientSelected.Verify();
        }
    }
}