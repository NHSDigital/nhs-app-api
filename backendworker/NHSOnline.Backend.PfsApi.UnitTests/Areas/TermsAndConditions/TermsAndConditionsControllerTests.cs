using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
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
using NHSOnline.Backend.PfsApi.Areas.TermsAndConditions;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.TermsAndConditions
{
    [TestClass]
    public sealed class TermsAndConditionsControllerTests: IDisposable
    {
        private P9UserSession _userSession;
        private string _nhsLoginId;

        private Mock<ITermsAndConditionsService> _mockTermsAndConditionsService;
        private Mock<IAuditor> _mockAuditor;

        private TermsAndConditionsController _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _nhsLoginId = "NHS login id";

            _userSession = new P9UserSession("csrfToken", new CitizenIdUserSession(), new EmisUserSession(), "im1token")
            {
                CitizenIdUserSession =
                {
                    AccessToken = JwtToken.Generate(new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _nhsLoginId),
                        new Claim("nhs_number", "NHS Number")
                    })
                }
            };

            _mockTermsAndConditionsService = new Mock<ITermsAndConditionsService>();
            _mockAuditor = new Mock<IAuditor>();

            _systemUnderTest = new TermsAndConditionsController(
                _mockTermsAndConditionsService.Object,
                new Mock<ILogger<TermsAndConditionsController>>().Object,
                _mockAuditor.Object);
        }

        [TestMethod]
        public async Task PostForInitialConsent_Returns_Success()
        {
            // Arrange
            var request = new ConsentRequest();
            TermsAndConditionsRecordConsentResult response = new TermsAndConditionsRecordConsentResult.InitialConsentRecorded();

            _mockTermsAndConditionsService
                .Setup(x => x.RecordConsent(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult(response));
            ArrangeAudit();

            // Act
            var result = await _systemUnderTest.Post(request, _userSession);

            // Assert
            _mockTermsAndConditionsService.VerifyAll();

            var okObjectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            okObjectResult.Value.Should()
                .BeAssignableTo<TermsAndConditionsRecordConsentResult.InitialConsentRecorded>();
        }

        [TestMethod]
        [DataRow(true, true, DisplayName = "Audit Initial Success, Consent: true, Cookies: true")]
        [DataRow(true, false, DisplayName = "Audit Initial Success, Consent: true, Cookies: false")]
        [DataRow(false, true, DisplayName = "Audit Initial Success, Consent: false, Cookies: true")]
        [DataRow(false, false, DisplayName = "Audit Initial Success, Consent: false, Cookies: false")]
        public async Task PostForInitialConsent_Audits_Success(bool consent, bool analyticsCookieAccepted)
        {
            // Arrange
            var request = new ConsentRequest { AnalyticsCookieAccepted = analyticsCookieAccepted, ConsentGiven = consent, UpdatingConsent = false };
            TermsAndConditionsRecordConsentResult response = new TermsAndConditionsRecordConsentResult.InitialConsentRecorded();

            _mockTermsAndConditionsService
                .Setup(x => x.RecordConsent(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult(response));
            var auditStub = ArrangeAudit();

            // Act
            await _systemUnderTest.Post(request, _userSession);

            // Assert
            using (new AssertionScope())
            {
                auditStub.Operation.Should().Be("TermsAndConditions_RecordConsent");
                auditStub.Details.Should().Be("Attempting to record patient consent - ConsentGiven={0}, AnalyticsCookieAccepted={1} at DateOfConsent={2}");
                auditStub.Parameters[0].Should().Be(consent);
                auditStub.Parameters[1].Should().Be(analyticsCookieAccepted);
                auditStub.ResponseDetails.Should().Be("Initial Consent Successfully recorded");
            }
        }

        [TestMethod]
        public async Task PostForUpdatedConsent_Returns_Success()
        {
            // Arrange
            var request = new ConsentRequest();
            request.UpdatingConsent = true;

            TermsAndConditionsRecordConsentResult response = new TermsAndConditionsRecordConsentResult.UpdateConsentRecorded();

            _mockTermsAndConditionsService
                .Setup(x => x.RecordConsent(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult(response));
            ArrangeAudit();

            // Act
            var result = await _systemUnderTest.Post(request, _userSession);

            // Assert
            _mockTermsAndConditionsService.VerifyAll();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<TermsAndConditionsRecordConsentResult.UpdateConsentRecorded>();
        }

        [TestMethod]
        [DataRow(true, true, DisplayName = "Audit Update Success, Consent: true, Cookies: true")]
        [DataRow(true, false, DisplayName = "Audit Update Success, Consent: true, Cookies: false")]
        [DataRow(false, true, DisplayName = "Audit Update Success, Consent: false, Cookies: true")]
        [DataRow(false, false, DisplayName = "Audit Update Success, Consent: false, Cookies: false")]
        public async Task PostForUpdateConsent_Audits_Success(bool consent, bool analyticsCookieAccepted)
        {
            // Arrange
            var request = new ConsentRequest { AnalyticsCookieAccepted = analyticsCookieAccepted, ConsentGiven = consent, UpdatingConsent = true };
            TermsAndConditionsRecordConsentResult response = new TermsAndConditionsRecordConsentResult.UpdateConsentRecorded();

            _mockTermsAndConditionsService
                .Setup(x => x.RecordConsent(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult(response));
            var auditStub = ArrangeAudit();

            // Act
            await _systemUnderTest.Post(request, _userSession);

            // Assert
            using (new AssertionScope())
            {
                auditStub.Operation.Should().Be("TermsAndConditions_RecordConsent");
                auditStub.Details.Should().Be("Attempting to record patient consent - ConsentGiven={0}, AnalyticsCookieAccepted={1} at DateOfConsent={2}");
                auditStub.Parameters[0].Should().Be(consent);
                auditStub.Parameters[1].Should().Be(analyticsCookieAccepted);
                auditStub.ResponseDetails.Should().Be("Updated Consent Successfully recorded");
            }
        }

        [TestMethod]
        public async Task Post_Returns_Failure()
        {
            // Arrange
            var request = new ConsentRequest();
            TermsAndConditionsRecordConsentResult response = new TermsAndConditionsRecordConsentResult.InternalServerError();
            _mockTermsAndConditionsService
                .Setup(x => x.RecordConsent(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult(response))
                .Verifiable();
            ArrangeAudit();

            // Act
            var result = await _systemUnderTest.Post(request, _userSession);

            // Assert
            _mockTermsAndConditionsService.VerifyAll();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        [DataRow(true, true, DisplayName = "Audit Failure, Consent: true, Cookies: true")]
        [DataRow(true, false, DisplayName = "Audit Failure, Consent: true, Cookies: false")]
        [DataRow(false, true, DisplayName = "Audit Failure, Consent: false, Cookies: true")]
        [DataRow(false, false, DisplayName = "Audit Failure, Consent: false, Cookies: false")]
        public async Task Post_Audits_Failure(bool consent, bool analyticsCookieAccepted)
        {
            // Arrange
            var request = new ConsentRequest { AnalyticsCookieAccepted = analyticsCookieAccepted, ConsentGiven = consent, UpdatingConsent = true };
            TermsAndConditionsRecordConsentResult response = new TermsAndConditionsRecordConsentResult.InternalServerError();

            _mockTermsAndConditionsService
                .Setup(x => x.RecordConsent(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult(response));
            var auditStub = ArrangeAudit();

            // Act
            await _systemUnderTest.Post(request, _userSession);

            // Assert
            using (new AssertionScope())
            {
                auditStub.Operation.Should().Be("TermsAndConditions_RecordConsent");
                auditStub.Details.Should().Be("Attempting to record patient consent - ConsentGiven={0}, AnalyticsCookieAccepted={1} at DateOfConsent={2}");
                auditStub.Parameters[0].Should().Be(consent);
                auditStub.Parameters[1].Should().Be(analyticsCookieAccepted);
                auditStub.ResponseDetails.Should().Be("Failed to record");
            }
        }

        [TestMethod]
        public async Task Get_Returns_Success()
        {
            // Arrange
            var consentRecord = new ConsentResponse();
            var response = new TermsAndConditionsFetchConsentResult.Success(consentRecord);
            _mockTermsAndConditionsService.Setup(x => x.FetchConsent(_nhsLoginId))
                .Returns(Task.FromResult((TermsAndConditionsFetchConsentResult) response));

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _mockTermsAndConditionsService.VerifyAll();

            var value = result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<TermsAndConditionsFetchConsentResult.Success>()
                .Subject;
            using (new AssertionScope())
            {
                value.Response.ConsentGiven.Should().Be(consentRecord.ConsentGiven);
                value.Response.UpdatedConsentRequired.Should().Be(consentRecord.UpdatedConsentRequired);
                value.Response.AnalyticsCookieAccepted.Should().Be(consentRecord.AnalyticsCookieAccepted);
            }
        }

        [TestMethod]
        public async Task Get_Returns_NotFound()
        {
            // Arrange
            var response = new TermsAndConditionsFetchConsentResult.NoConsentFound(new ConsentResponse());
            _mockTermsAndConditionsService.Setup(x => x.FetchConsent(_nhsLoginId))
                .Returns(Task.FromResult((TermsAndConditionsFetchConsentResult) response));

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _mockTermsAndConditionsService.VerifyAll();

            var okObjectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            var value = okObjectResult.Value.Should()
                .BeAssignableTo<TermsAndConditionsFetchConsentResult.NoConsentFound>().Subject;
            using (new AssertionScope())
            {
                value.Response.ConsentGiven.Should().BeFalse();
                value.Response.UpdatedConsentRequired.Should().BeFalse();
                value.Response.AnalyticsCookieAccepted.Should().BeFalse();
            }
        }

        [TestMethod]
        public async Task Get_Returns_Failure()
        {
            // Arrange
            var response = new TermsAndConditionsFetchConsentResult.InternalServerError();
            _mockTermsAndConditionsService.Setup(x => x.FetchConsent(_nhsLoginId))
                .Returns(Task.FromResult((TermsAndConditionsFetchConsentResult) response));

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _mockTermsAndConditionsService.VerifyAll();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task ToggleAnalyticsCookieAcceptance_Returns_Success()
        {
            // Arrange
            var request = new AnalyticsCookieAcceptance();
            var response = new ToggleAnalyticsCookieAcceptanceResult.Success();

            _mockTermsAndConditionsService.Setup(x
                    => x.ToggleAnalyticsCookieAcceptance(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((ToggleAnalyticsCookieAcceptanceResult) response))
                .Verifiable();
            ArrangeAudit();

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(request, _userSession);

            // Assert
            _mockTermsAndConditionsService.VerifyAll();

            var noContentResult = result.Should().BeAssignableTo<NoContentResult>().Subject;
            noContentResult.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task ToggleAnalyticsCookieAcceptance_Audits_Success(bool analyticsCookieAccepted)
        {
            // Arrange
            var request = new AnalyticsCookieAcceptance { AnalyticsCookieAccepted = analyticsCookieAccepted };
            var response = new ToggleAnalyticsCookieAcceptanceResult.Success();

            _mockTermsAndConditionsService.Setup(x
                    => x.ToggleAnalyticsCookieAcceptance(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((ToggleAnalyticsCookieAcceptanceResult)response))
                .Verifiable();
            var auditStub = ArrangeAudit();

            // Act
            await _systemUnderTest.ToggleAnalyticsCookieAcceptance(request, _userSession);

            // Assert
            using (new AssertionScope())
            {
                auditStub.Operation.Should().Be("TermsAndConditions_ToggleAnalyticsCookieAcceptance");
                auditStub.Details.Should().Be("Attempting to toggle analytics cookie acceptance - AnalyticsCookieAccepted={0} at DateOfAnalyticsCookieToggle={1}");
                auditStub.Parameters[0].Should().Be(analyticsCookieAccepted);
                auditStub.ResponseDetails.Should().Be("Analytics Cookie Consent toggled successfully");
            }
        }

        [TestMethod]
        public async Task ToggleAnalyticsCookieAcceptance_Returns_Failure()
        {
            // Arrange
            var request = new AnalyticsCookieAcceptance();
            var response = new ToggleAnalyticsCookieAcceptanceResult.Failure();
            _mockTermsAndConditionsService.Setup(x
                    => x.ToggleAnalyticsCookieAcceptance(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((ToggleAnalyticsCookieAcceptanceResult) response))
                .Verifiable();
            ArrangeAudit();

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(request, _userSession);

            // Assert
            _mockTermsAndConditionsService.VerifyAll();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task ToggleAnalyticsCookieAcceptance_Audits_Failure(bool analyticsCookieAccepted)
        {
            // Arrange
            var request = new AnalyticsCookieAcceptance { AnalyticsCookieAccepted = analyticsCookieAccepted };
            var response = new ToggleAnalyticsCookieAcceptanceResult.Failure();
            _mockTermsAndConditionsService.Setup(x
                    => x.ToggleAnalyticsCookieAcceptance(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((ToggleAnalyticsCookieAcceptanceResult)response))
                .Verifiable();
            var auditStub = ArrangeAudit();

            // Act
            await _systemUnderTest.ToggleAnalyticsCookieAcceptance(request, _userSession);

            // Assert
            using (new AssertionScope())
            {
                auditStub.Operation.Should().Be("TermsAndConditions_ToggleAnalyticsCookieAcceptance");
                auditStub.Details.Should().Be("Attempting to toggle analytics cookie acceptance - AnalyticsCookieAccepted={0} at DateOfAnalyticsCookieToggle={1}");
                auditStub.Parameters[0].Should().Be(analyticsCookieAccepted);
                auditStub.ResponseDetails.Should().Be("Failed to toggle analytics cookie");
            }
        }

        public void Dispose() => _systemUnderTest?.Dispose();

        private AuditBuilderStub ArrangeAudit()
        {
            var auditBuilderStub = new AuditBuilderStub();
            _mockAuditor.Setup(x => x.Audit()).Returns(auditBuilderStub);
            return auditBuilderStub;
        }
    }
}
