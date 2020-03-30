using System;
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
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Session
{
    [TestClass]
    public class TppSessionExtendServiceTests
    {
        private IFixture _fixture;
        private TppUserSession _tppUserSession;
        private GpLinkedAccountModel _gpLinkedAccountModel;
        private TppRequestParameters _tppRequestParameters;
        private TppSessionExtendService _systemUnderTest;
        private Mock<ITppClientRequest<TppRequestParameters, PatientSelectedReply>> _mockPatientSelected;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _tppUserSession = _fixture.Create<TppUserSession>();
            _gpLinkedAccountModel = new GpLinkedAccountModel(_tppUserSession);
            _tppRequestParameters = new TppRequestParameters(_tppUserSession);

            _mockPatientSelected = _fixture.Freeze<Mock<ITppClientRequest<TppRequestParameters, PatientSelectedReply>>>();

            _systemUnderTest = _fixture.Create<TppSessionExtendService>();
        }

        [TestMethod]
        public async Task Extend_WhenClientReturnsSuccess_ReturnsSuccess()
        {
            // Arrange
            var response = new TppApiObjectResponse<PatientSelectedReply>(HttpStatusCode.OK);

            _mockPatientSelected
                .Setup(x => x.Post(
                    It.Is<TppRequestParameters>(p => MatchTppRequestParameters(p))))
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.Success>();
            _mockPatientSelected.Verify();
        }

        [TestMethod]
        public async Task Extend_WhenClientReturnsError_ReturnsBadGateway()
        {
            // Arrange
            var response = new TppApiObjectResponse<PatientSelectedReply>(HttpStatusCode.BadRequest);

            _mockPatientSelected
                .Setup(x => x.Post(
                    It.Is<TppRequestParameters>(p => MatchTppRequestParameters(p))))
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.BadGateway>();
            _mockPatientSelected.Verify();
        }

        [TestMethod]
        public async Task Extend_WhenClientThrowsHttpRequestException_ReturnsBadGateway()
        {
            // Arrange
            _mockPatientSelected
                .Setup(x => x.Post(
                    It.Is<TppRequestParameters>(p => MatchTppRequestParameters(p))))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.BadGateway>();
            _mockPatientSelected.Verify();
        }

        private bool MatchTppRequestParameters(TppRequestParameters p) =>
            p.OnlineUserId.Equals(_tppRequestParameters.OnlineUserId, StringComparison.Ordinal) &&
            p.Suid.Equals(_tppRequestParameters.Suid, StringComparison.Ordinal) &&
            p.PatientId.Equals(_tppRequestParameters.PatientId, StringComparison.Ordinal) &&
            p.OdsCode.Equals(_tppRequestParameters.OdsCode, StringComparison.Ordinal);
    }
}