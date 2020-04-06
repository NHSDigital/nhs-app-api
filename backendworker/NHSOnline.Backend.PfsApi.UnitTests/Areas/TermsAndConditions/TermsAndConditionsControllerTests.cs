using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.TermsAndConditions;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.TermsAndConditions
{
    [TestClass]
    public class TermsAndConditionsControllerTests
    {
        private TermsAndConditionsController _systemUnderTest;
        private IFixture _fixture;
        private Mock<ITermsAndConditionsService> _termsAndConditionsService;
        private P9UserSession _userSession;
        private string _nhsLoginId;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _termsAndConditionsService = _fixture.Freeze<Mock<ITermsAndConditionsService>>();

            _nhsLoginId = _fixture.Create<string>();

            _userSession = _fixture.Create<P9UserSession>();
            _userSession.CitizenIdUserSession.AccessToken = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _nhsLoginId),
                new Claim("nhs_number", _fixture.Create<string>())
            });

            _systemUnderTest = _fixture.Create<TermsAndConditionsController>();
        }

        [TestMethod]
        public async Task PostForInitialConsent_Returns_Success()
        {
            // Arrange
            var request = _fixture.Create<ConsentRequest>();
            var response = new TermsAndConditionsRecordConsentResult.InitialConsentRecorded();

            _termsAndConditionsService.Setup(x => x.RecordConsent(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((TermsAndConditionsRecordConsentResult) response));

            // Act
            var result = await _systemUnderTest.Post(request, _userSession);

            // Assert
            _termsAndConditionsService.VerifyAll();

            var okObjectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            okObjectResult.Value.Should()
                .BeAssignableTo<TermsAndConditionsRecordConsentResult.InitialConsentRecorded>();
        }

        [TestMethod]
        public async Task PostForUpdatedConsent_Returns_Success()
        {
            // Arrange
            var request = _fixture.Create<ConsentRequest>();
            request.UpdatingConsent = true;

            var response = new TermsAndConditionsRecordConsentResult.UpdateConsentRecorded();

            _termsAndConditionsService.Setup(x => x.RecordConsent(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((TermsAndConditionsRecordConsentResult) response));

            // Act
            var result = await _systemUnderTest.Post(request, _userSession);

            // Assert
            _termsAndConditionsService.VerifyAll();

            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<TermsAndConditionsRecordConsentResult.UpdateConsentRecorded>();
        }

        [TestMethod]
        public async Task Post_Returns_Failure()
        {
            // Arrange
            var request = _fixture.Create<ConsentRequest>();
            var response = new TermsAndConditionsRecordConsentResult.InternalServerError();
            _termsAndConditionsService.Setup(x => x.RecordConsent(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((TermsAndConditionsRecordConsentResult) response))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(request, _userSession);

            // Assert
            _termsAndConditionsService.VerifyAll();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_Returns_Success()
        {
            // Arrange
            var consentRecord = _fixture.Create<ConsentResponse>();
            var response = new TermsAndConditionsFetchConsentResult.Success(consentRecord);
            _termsAndConditionsService.Setup(x => x.FetchConsent(_nhsLoginId))
                .Returns(Task.FromResult((TermsAndConditionsFetchConsentResult) response));

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _termsAndConditionsService.VerifyAll();

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
            _termsAndConditionsService.Setup(x => x.FetchConsent(_nhsLoginId))
                .Returns(Task.FromResult((TermsAndConditionsFetchConsentResult) response));

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _termsAndConditionsService.VerifyAll();

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
            _termsAndConditionsService.Setup(x => x.FetchConsent(_nhsLoginId))
                .Returns(Task.FromResult((TermsAndConditionsFetchConsentResult) response));

            // Act
            var result = await _systemUnderTest.Get(_userSession);

            // Assert
            _termsAndConditionsService.VerifyAll();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task ToggleAnalyticsCookieAcceptance_Returns_Success()
        {
            // Arrange
            var request = _fixture.Create<AnalyticsCookieAcceptance>();
            var response = new ToggleAnalyticsCookieAcceptanceResult.Success();

            _termsAndConditionsService.Setup(x
                    => x.ToggleAnalyticsCookieAcceptance(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((ToggleAnalyticsCookieAcceptanceResult) response))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(request, _userSession);

            // Assert
            _termsAndConditionsService.VerifyAll();

            var noContentResult = result.Should().BeAssignableTo<NoContentResult>().Subject;
            noContentResult.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task ToggleAnalyticsCookieAcceptance_Returns_Failure()
        {
            // Arrange
            var request = _fixture.Create<AnalyticsCookieAcceptance>();
            var response = new ToggleAnalyticsCookieAcceptanceResult.Failure();
            _termsAndConditionsService.Setup(x
                    => x.ToggleAnalyticsCookieAcceptance(_nhsLoginId, request, It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((ToggleAnalyticsCookieAcceptanceResult) response))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(request, _userSession);

            // Assert
            _termsAndConditionsService.VerifyAll();

            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }
    }
}
