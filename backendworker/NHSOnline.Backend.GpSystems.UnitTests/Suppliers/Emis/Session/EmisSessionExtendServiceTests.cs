using System;
using System.Collections.Generic;
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
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Session
{
    [TestClass]
    public class EmisSessionExtendServiceTests
    {
        private IFixture _fixture;
        private EmisUserSession _emisUserSession;
        private GpLinkedAccountModel _gpLinkedAccountModel;
        private EmisRequestParameters _emisRequestParameters;
        private Mock<IEmisClient> _mockEmisClient;
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        private EmisSessionExtendService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _emisUserSession = _fixture.Create<EmisUserSession>();
            _gpLinkedAccountModel = new GpLinkedAccountModel(_emisUserSession);
            _emisRequestParameters = new EmisRequestParameters(_emisUserSession);

            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _sampleSuccessStatusCodes = new List<HttpStatusCode>
            {
                HttpStatusCode.OK
            };

            _systemUnderTest = _fixture.Create<EmisSessionExtendService>();
        }

        [TestMethod]
        public async Task Extend_WhenClientReturnsSuccess_ReturnsSuccess()
        {
            // Arrange
            var response = new EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.DemographicsGet, _sampleSuccessStatusCodes);

            _mockEmisClient
                .Setup(x => x.DemographicsGet(
                    It.Is<EmisRequestParameters>(p => MatchEmisRequestParameters(p))))
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.Success>();
            _mockEmisClient.Verify();
        }

        [TestMethod]
        public async Task Extend_WhenClientReturnsError_ReturnsBadGateway()
        {
            // Arrange
            var response = new EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.BadRequest, RequestsForSuccessOutcome.DemographicsGet, _sampleSuccessStatusCodes);

            _mockEmisClient
                .Setup(x => x.DemographicsGet(
                    It.Is<EmisRequestParameters>(p => MatchEmisRequestParameters(p))))
                .ReturnsAsync(response)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.BadGateway>();
            _mockEmisClient.Verify();
        }

        [TestMethod]
        public async Task Extend_WhenClientThrowsHttpRequestException_ReturnsBadGateway()
        {
            // Arrange
            _mockEmisClient
                .Setup(x => x.DemographicsGet(
                    It.Is<EmisRequestParameters>(p => MatchEmisRequestParameters(p))))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Extend(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<SessionExtendResult.BadGateway>();
            _mockEmisClient.Verify();
        }

        private bool MatchEmisRequestParameters(EmisRequestParameters p) =>
            p.SessionId.Equals(_emisRequestParameters.SessionId, StringComparison.Ordinal) &&
            p.EndUserSessionId.Equals(_emisRequestParameters.EndUserSessionId, StringComparison.Ordinal) &&
            p.UserPatientLinkToken.Equals(_emisRequestParameters.UserPatientLinkToken, StringComparison.Ordinal);
    }
}

