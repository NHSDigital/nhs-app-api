using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
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
using NHSOnline.Backend.PfsApi.UnitTests.Audit;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
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
        private P9UserSession _p9UserSession;
        private P5UserSession _p5UserSession;
        private CreateJwtRequest _request;

        private const string RequestAuditType = "AssertedLoginIdentity_CreateJwt";

       [TestInitialize]
        public void TestInitialize()
        {
            _p9UserSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), new EmisUserSession(), "im1token");
            _p5UserSession = new P5UserSession("csrfToken", new CitizenIdUserSession());
            _request = new CreateJwtRequest();

            _mockAssertedLoginIdentityService = new Mock<IAssertedLoginIdentityService>();
            _mockAuditor = new Mock<IAuditor>();
            _mockLogger = new Mock<ILogger<AssertedLoginIdentityController>>();

            _systemUnderTest = new AssertedLoginIdentityController(_mockAuditor.Object, _mockLogger.Object, _mockAssertedLoginIdentityService.Object);
        }

        [TestMethod]
        public async Task Post_P9_ServiceReturnsInternalServerError_Returns500StatusCode()
        {
            // Arrange
            _mockAssertedLoginIdentityService
                .Setup(x => x.CreateJwtToken(_p9UserSession.CitizenIdUserSession.IdTokenJti))
                .Returns(new CreateJwtResult.InternalServerError());

            var auditStub = ArrangeAudit();

            // Act
            var result = await _systemUnderTest.Post(_request, _p9UserSession);

            // Assert
            _mockAssertedLoginIdentityService.Verify();

            result.Should().BeOfType<StatusCodeResult>().Subject
                .StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            using (new AssertionScope())
            {
                auditStub.Operation.Should().Be(RequestAuditType);
                auditStub.Details.Should().Be("Creating Asserted login Identity JWT for Provider ID '{0}', Provider Name '{1}', "
                                              + "Jump Off ID '{2}', intended relying party URL: {3}",
                    _request.ProviderId, _request.ProviderName, _request.JumpOffId,
                    _request.IntendedRelyingPartyUrl);
                auditStub.ResponseDetails.Should().Be("AssertedLoginIdentity Token creation failed.");
            }

            _mockLogger.VerifyLogger(LogLevel.Information, "Created Asserted Login Identity", Times.Never());
        }

        [TestMethod]
        public async Task Post_P5_ServiceReturnsInternalServerError_Returns500StatusCode()
        {
            // Arrange
            _mockAssertedLoginIdentityService
                .Setup(x => x.CreateJwtToken(_p5UserSession.CitizenIdUserSession.IdTokenJti))
                .Returns(new CreateJwtResult.InternalServerError());

            // Act
            var result = await _systemUnderTest.Post(_request, _p5UserSession);

            // Assert
            _mockAssertedLoginIdentityService.Verify();

            result.Should().BeOfType<StatusCodeResult>().Subject
                .StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            _mockLogger.VerifyLogger(LogLevel.Information, "Created Asserted Login Identity", Times.Never());
        }

        [TestMethod]
        public async Task Post_P9_ServiceSucceeds_ReturnsCreatedResult()
        {
            // Arrange
            var expectedResponse = new CreateJwtResponse();
            _mockAssertedLoginIdentityService
                .Setup(x => x.CreateJwtToken(_p9UserSession.CitizenIdUserSession.IdTokenJti))
                .Returns(new CreateJwtResult.Success(expectedResponse));

            var auditStub = ArrangeAudit();

            // Act
            var result = await _systemUnderTest.Post(_request, _p9UserSession);

            // Assert
            _mockAssertedLoginIdentityService.Verify();

            result.Should().BeOfType<CreatedResult>()
                .Subject.Value.Should().BeOfType<CreateJwtResponse>()
                .Subject.Should().Be(expectedResponse);

            using (new AssertionScope())
            {
                auditStub.Operation.Should().Be(RequestAuditType);
                auditStub.Details.Should().Be("Creating Asserted login Identity JWT for Provider ID '{0}', Provider Name '{1}', "
                                              + "Jump Off ID '{2}', intended relying party URL: {3}",
                    _request.ProviderId, _request.ProviderName, _request.JumpOffId,
                    _request.IntendedRelyingPartyUrl);
                auditStub.ResponseDetails.Should().Be("AssertedLoginIdentity Token creation succeeded.");
            }

            _mockLogger.VerifyLogger(LogLevel.Information,
                $"Created Asserted Login Identity: ProviderId={_request.ProviderId} " +
                    $"ProviderName={_request.ProviderName} " +
                    $"JumpOffId={_request.JumpOffId} " +
                    $"IntendedRelyingPartyUrl={_request.IntendedRelyingPartyUrl}", Times.Once());
        }

        [TestMethod]
        public async Task Post_P5_ServiceSucceeds_ReturnsCreatedResult()
        {
            // Arrange
            var expectedResponse = new CreateJwtResponse();
            _mockAssertedLoginIdentityService
                .Setup(x => x.CreateJwtToken(_p5UserSession.CitizenIdUserSession.IdTokenJti))
                .Returns(new CreateJwtResult.Success(expectedResponse));

            // Act
            var result = await _systemUnderTest.Post(_request, _p5UserSession);

            // Assert
            _mockAssertedLoginIdentityService.Verify();

            result.Should().BeOfType<CreatedResult>()
                .Subject.Value.Should().BeOfType<CreateJwtResponse>()
                .Subject.Should().Be(expectedResponse);

            _mockLogger.VerifyLogger(LogLevel.Information,
                $"Created Asserted Login Identity: ProviderId={_request.ProviderId} " +
                $"ProviderName={_request.ProviderName} " +
                $"JumpOffId={_request.JumpOffId} " +
                $"IntendedRelyingPartyUrl={_request.IntendedRelyingPartyUrl}", Times.Once());
        }

        private AuditBuilderStub ArrangeAudit()
        {
            var auditBuilderStub = new AuditBuilderStub();
            _mockAuditor.Setup(x => x.Audit()).Returns(auditBuilderStub);
            return auditBuilderStub;
        }

        public void Dispose() => _systemUnderTest?.Dispose();
    }
}