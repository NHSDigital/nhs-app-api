using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.PfsApi.Areas.AssertedLoginIdentity;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity.Models;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.AssertedLoginIdentity
{
    [TestClass]
    public sealed class AssertedLoginIdentityControllerTests: IDisposable
    {
        private AssertedLoginIdentityController _systemUnderTest;

        private Mock<IAssertedLoginIdentityService> _mockAssertedLoginIdentityService;
        private Mock<IAuditor> _mockAuditor;
        private Mock<ILogger<AssertedLoginIdentityController>> _mockLogger;
        private P9UserSession _userSession;
        private CreateJwtRequest _request;

        private const string RequestAuditType = "AssertedLoginIdentity_CreateJwt_Request";
        private const string ResponseAuditType = "AssertedLoginIdentity_CreateJwt_Response";

       [TestInitialize]
        public void TestInitialize()
        {
            _userSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), new EmisUserSession(), "im1token");
            _request = new CreateJwtRequest();

            _mockAssertedLoginIdentityService = new Mock<IAssertedLoginIdentityService>();
            _mockAuditor = new Mock<IAuditor>();
            _mockLogger = new Mock<ILogger<AssertedLoginIdentityController>>();

            _systemUnderTest = new AssertedLoginIdentityController(_mockAuditor.Object, _mockLogger.Object, _mockAssertedLoginIdentityService.Object);
        }

        [TestMethod]
        public async Task Post_ServiceReturnsInternalServerError_Returns500StatusCode()
        {
            // Arrange
            _mockAssertedLoginIdentityService
                .Setup(x => x.CreateJwtToken(_userSession.CitizenIdUserSession.IdTokenJti))
                .Returns(new CreateJwtResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post(_request, _userSession);

            // Assert
            _mockAssertedLoginIdentityService.Verify();

            result.Should().BeOfType<StatusCodeResult>().Subject
                .StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            _mockAuditor.Verify(x => x.Audit(RequestAuditType,
                "Creating Asserted login Identity JWT for Provider ID '{0}', Provider Name '{1}', "
                + "Jump Off ID '{2}', intended relying party URL: {3}",
                _request.ProviderId, _request.ProviderName, _request.JumpOffId,
                _request.IntendedRelyingPartyUrl));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "AssertedLoginIdentity Token creation failed."));

            _mockLogger.VerifyLogger(LogLevel.Information, "Created Asserted Login Identity", Times.Never());
        }

        [TestMethod]
        public async Task Post_ServiceSucceeds_ReturnsCreatedResult()
        {
            // Arrange
            var expectedResponse = new CreateJwtResponse();
            _mockAssertedLoginIdentityService
                .Setup(x => x.CreateJwtToken(_userSession.CitizenIdUserSession.IdTokenJti))
                .Returns(new CreateJwtResult.Success(expectedResponse));

            // Act
            var result = await _systemUnderTest.Post(_request, _userSession);

            // Assert
            _mockAssertedLoginIdentityService.Verify();

            result.Should().BeOfType<CreatedResult>()
                .Subject.Value.Should().BeOfType<CreateJwtResponse>()
                .Subject.Should().Be(expectedResponse);

            _mockAuditor.Verify(x => x.Audit(RequestAuditType,
                "Creating Asserted login Identity JWT for Provider ID '{0}', Provider Name '{1}', "
                + "Jump Off ID '{2}', intended relying party URL: {3}",
                _request.ProviderId, _request.ProviderName, _request.JumpOffId,
                _request.IntendedRelyingPartyUrl));
            _mockAuditor.Verify(x => x.Audit(ResponseAuditType,
                "AssertedLoginIdentity Token creation succeeded."));

            _mockLogger.VerifyLogger(LogLevel.Information,
                $"Created Asserted Login Identity: ProviderId={_request.ProviderId} " +
                    $"ProviderName={_request.ProviderName} " +
                    $"JumpOffId={_request.JumpOffId} " +
                    $"IntendedRelyingPartyUrl={_request.IntendedRelyingPartyUrl}", Times.Once());
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}