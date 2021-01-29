using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auditing.UnitTestsSupport;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.Areas.AssertedLoginIdentity;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity;
using NHSOnline.Backend.PfsApi.AssertedLoginIdentity.Models;
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
        private Mock<IMetricLogger> _mockMetricLogger;
        private P9UserSession _p9UserSession;
        private P5UserSession _p5UserSession;

        private const string RequestAuditType = "AssertedLoginIdentity_CreateJwt";

       [TestInitialize]
        public void TestInitialize()
        {
            _p9UserSession = new P9UserSession(
                "csrfToken",
                "nhsNumber",
                new CitizenIdUserSession{ OdsCode = "A12345" },
                new EmisUserSession(),
                "im1token");

            _p5UserSession = new P5UserSession(
                "csrfToken",
                new CitizenIdUserSession{ OdsCode = "B23456"});

            _mockAssertedLoginIdentityService = new Mock<IAssertedLoginIdentityService>();
            _mockAuditor = new Mock<IAuditor>();
            _mockLogger = new Mock<ILogger<AssertedLoginIdentityController>>();
            _mockMetricLogger = new Mock<IMetricLogger>();

            _systemUnderTest = new AssertedLoginIdentityController(
                _mockAuditor.Object,
                _mockLogger.Object,
                _mockMetricLogger.Object,
                _mockAssertedLoginIdentityService.Object);
        }

        [TestMethod]
        public async Task Post_P9_ServiceReturnsInternalServerError_Returns500StatusCode()
        {
            // Arrange
            _mockAssertedLoginIdentityService
                .Setup(x => x.CreateJwtToken(_p9UserSession.CitizenIdUserSession.IdTokenJti))
                .Returns(new CreateJwtResult.InternalServerError());

            var auditStub = ArrangeAudit();
            var request = new CreateJwtRequest();

            // Act
            var result = await _systemUnderTest.Post(request, _p9UserSession);

            // Assert
            _mockAssertedLoginIdentityService.Verify();

            result.Should().BeOfType<StatusCodeResult>().Subject
                .StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            using (new AssertionScope())
            {
                auditStub.Operation.Should().Be(RequestAuditType);
                auditStub.Details.Should().Be("Creating Asserted login Identity JWT for Provider ID '{0}', Provider Name '{1}', "
                                              + "Jump Off ID '{2}', intended relying party URL: {3}",
                    request.ProviderId, request.ProviderName, request.JumpOffId,
                    request.IntendedRelyingPartyUrl);
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
            var request = new CreateJwtRequest();

            // Act
            var result = await _systemUnderTest.Post(request, _p5UserSession);

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
            var request = new CreateJwtRequest
            {
                Action = "",
                ProviderId = "pkb",
                ProviderName = "Patients Know Best",
                JumpOffId = "appointments",
                IntendedRelyingPartyUrl = "https://www.patientknowbest.com/foo"
            };

            // Act
            var result = await _systemUnderTest.Post(request, _p9UserSession);

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
                    request.ProviderId, request.ProviderName, request.JumpOffId,
                    request.IntendedRelyingPartyUrl);
                auditStub.ResponseDetails.Should().Be("AssertedLoginIdentity Token creation succeeded.");
            }

            _mockLogger.VerifyLogger(LogLevel.Information,
                $"Created Asserted Login Identity: " +
                    $"OdsCode={_p9UserSession.OdsCode} " +
                    $"ProviderId={request.ProviderId} " +
                    $"ProviderName=\"{request.ProviderName}\" " +
                    $"JumpOffId={request.JumpOffId} " +
                    $"IntendedRelyingPartyUrl={request.IntendedRelyingPartyUrl}", Times.Once());
        }

        [TestMethod]
        public async Task Post_P5_ServiceSucceeds_ReturnsCreatedResult()
        {
            // Arrange
            var expectedResponse = new CreateJwtResponse();
            _mockAssertedLoginIdentityService
                .Setup(x => x.CreateJwtToken(_p5UserSession.CitizenIdUserSession.IdTokenJti))
                .Returns(new CreateJwtResult.Success(expectedResponse));
            var request = new CreateJwtRequest
            {
                Action = "",
                ProviderId = "pkb",
                ProviderName = "Patients Know Best",
                JumpOffId = "appointments",
                IntendedRelyingPartyUrl = "https://www.patientknowbest.com/foo"
            };

            // Act
            var result = await _systemUnderTest.Post(request, _p5UserSession);

            // Assert
            _mockAssertedLoginIdentityService.Verify();

            result.Should().BeOfType<CreatedResult>()
                .Subject.Value.Should().BeOfType<CreateJwtResponse>()
                .Subject.Should().Be(expectedResponse);

            _mockLogger.VerifyLogger(LogLevel.Information,
                $"Created Asserted Login Identity: " +
                $"OdsCode={_p5UserSession.OdsCode} " +
                $"ProviderId={request.ProviderId} " +
                $"ProviderName=\"{request.ProviderName}\" " +
                $"JumpOffId={request.JumpOffId} " +
                $"IntendedRelyingPartyUrl={request.IntendedRelyingPartyUrl}", Times.Once());
        }

        [TestMethod]
        public async Task Post_UpliftStartedAction_ServiceSucceeds_CallMetricLoggerUpliftStarted()
        {
            // Arrange
            var expectedResponse = new CreateJwtResponse();
            _mockAssertedLoginIdentityService
                .Setup(x => x.CreateJwtToken(_p5UserSession.CitizenIdUserSession.IdTokenJti))
                .Returns(new CreateJwtResult.Success(expectedResponse));
            var request = new CreateJwtRequest { Action = "UpliftStarted" };

            // Act
            await _systemUnderTest.Post(request, _p5UserSession);

            // Assert
            _mockMetricLogger.Verify(
                x => x.UpliftStarted(
                    It.Is<UpliftStartedData>(
                        data => MatchesSessionId(data, _p5UserSession.Key))), Times.Once);
        }

        private bool MatchesSessionId(UpliftStartedData actual, string expectedSessionId)
        {
            var sessionId = actual.ToKeyValuePairs().Single();
            return sessionId.Key == "SessionId" && sessionId.Value == expectedSessionId;
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("Hello")]
        public async Task Post_OtherAction_ServiceSucceeds_DoesNotCallMetricLogger(string action)
        {
            // Arrange
            var expectedResponse = new CreateJwtResponse();
            _mockAssertedLoginIdentityService
                .Setup(x => x.CreateJwtToken(_p5UserSession.CitizenIdUserSession.IdTokenJti))
                .Returns(new CreateJwtResult.Success(expectedResponse));
            var request = new CreateJwtRequest { Action = action };

            // Act
            await _systemUnderTest.Post(request, _p5UserSession);

            // Assert
            _mockMetricLogger.VerifyNoOtherCalls();
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("Hello")]
        [DataRow("UpliftStarted")]
        public async Task Post_ServiceFails_DoesNotCallMetricLogger(string action)
        {
            // Arrange
            _mockAssertedLoginIdentityService
                .Setup(x => x.CreateJwtToken(_p5UserSession.CitizenIdUserSession.IdTokenJti))
                .Returns(new CreateJwtResult.InternalServerError());
            var request = new CreateJwtRequest { Action = action };

            // Act
            await _systemUnderTest.Post(request, _p5UserSession);

            // Assert
            _mockMetricLogger.VerifyNoOtherCalls();
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