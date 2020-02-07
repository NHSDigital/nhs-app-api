using System;
using System.Collections.Generic;
using System.Net;
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
        private UserSession _userSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _termsAndConditionsService = _fixture.Freeze<Mock<ITermsAndConditionsService>>();

            _userSession = _fixture.Create<UserSession>();
            _userSession.GpUserSession.NhsNumber = _fixture.Create<string>();
            _userSession.GpUserSession.OdsCode = _fixture.Create<string>();
            var httpContextItems = new Dictionary<object, object>
            {
                { Constants.HttpContextItems.UserSession, _userSession }
            };
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(x => x.Items).Returns(httpContextItems);

            _systemUnderTest = _fixture.Create<TermsAndConditionsController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task PostForInitialConsent_Returns_Success()
        {
            // Arrange
            var request = _fixture.Create<ConsentRequest>();
            var response = new TermsAndConditionsRecordConsentResult.InitialConsentRecorded();

            _termsAndConditionsService.Setup(x => x.RecordConsent(
                    _userSession.GpUserSession.NhsNumber, _userSession.GpUserSession.OdsCode, request,
                    It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((TermsAndConditionsRecordConsentResult) response))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(request);

            // Assert
            _termsAndConditionsService.Verify(x => x.RecordConsent(_userSession.GpUserSession.NhsNumber,
                _userSession.GpUserSession.OdsCode, request, It.IsAny<DateTimeOffset>()));
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

            _termsAndConditionsService.Setup(x => x.RecordConsent(
                    _userSession.GpUserSession.NhsNumber, _userSession.GpUserSession.OdsCode, request,
                    It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((TermsAndConditionsRecordConsentResult) response))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(request);

            // Assert
            _termsAndConditionsService.Verify(x => x.RecordConsent(_userSession.GpUserSession.NhsNumber,
                _userSession.GpUserSession.OdsCode, request, It.IsAny<DateTimeOffset>()));
            result.Should().BeAssignableTo<OkObjectResult>()
                .Subject.Value.Should().BeAssignableTo<TermsAndConditionsRecordConsentResult.UpdateConsentRecorded>();
        }

        [TestMethod]
        public async Task Post_Returns_Failure()
        {
            // Arrange
            var request = _fixture.Create<ConsentRequest>();
            var response = new TermsAndConditionsRecordConsentResult.InternalServerError();
            _termsAndConditionsService.Setup(x => x.RecordConsent(
                    _userSession.GpUserSession.NhsNumber, _userSession.GpUserSession.OdsCode, request,
                    It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((TermsAndConditionsRecordConsentResult) response))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(request);

            // Assert
            _termsAndConditionsService.Verify(x => x.RecordConsent(_userSession.GpUserSession.NhsNumber,
                _userSession.GpUserSession.OdsCode, request, It.IsAny<DateTimeOffset>()));
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task Get_Returns_Success()
        {
            // Arrange
            var consentRecord = _fixture.Create<ConsentResponse>();
            var response = new TermsAndConditionsFetchConsentResult.Success(consentRecord);
            _termsAndConditionsService.Setup(x => x.FetchConsent(_userSession.GpUserSession.NhsNumber))
                .Returns(Task.FromResult((TermsAndConditionsFetchConsentResult) response));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _termsAndConditionsService.Verify(x => x.FetchConsent(_userSession.GpUserSession.NhsNumber));
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
            _termsAndConditionsService.Setup(x => x.FetchConsent(_userSession.GpUserSession.NhsNumber))
                .Returns(Task.FromResult((TermsAndConditionsFetchConsentResult) response));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _termsAndConditionsService.Verify(x => x.FetchConsent(_userSession.GpUserSession.NhsNumber));
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
            _termsAndConditionsService.Setup(x => x.FetchConsent(_userSession.GpUserSession.NhsNumber))
                .Returns(Task.FromResult((TermsAndConditionsFetchConsentResult) response));

            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _termsAndConditionsService.Verify(x => x.FetchConsent(_userSession.GpUserSession.NhsNumber));
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [TestMethod]
        public async Task ToggleAnalyticsCookieAcceptance_Returns_Success()
        {
            // Arrange
            var request = _fixture.Create<AnalyticsCookieAcceptance>();
            var response = new ToggleAnalyticsCookieAcceptanceResult.Success();

            _termsAndConditionsService.Setup(x => x.ToggleAnalyticsCookieAcceptance(
                    _userSession.GpUserSession.NhsNumber, request,
                    It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((ToggleAnalyticsCookieAcceptanceResult) response))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(request);

            // Assert
            _termsAndConditionsService.Verify(x =>
                x.ToggleAnalyticsCookieAcceptance(_userSession.GpUserSession.NhsNumber, request,
                    It.IsAny<DateTimeOffset>()));
            var noContentResult = result.Should().BeAssignableTo<NoContentResult>().Subject;
            noContentResult.Should().BeAssignableTo<NoContentResult>();
        }

        [TestMethod]
        public async Task ToggleAnalyticsCookieAcceptance_Returns_Failure()
        {
            // Arrange
            var request = _fixture.Create<AnalyticsCookieAcceptance>();
            var response = new ToggleAnalyticsCookieAcceptanceResult.Failure();
            _termsAndConditionsService.Setup(x => x.ToggleAnalyticsCookieAcceptance(
                    _userSession.GpUserSession.NhsNumber, request,
                    It.IsAny<DateTimeOffset>()))
                .Returns(Task.FromResult((ToggleAnalyticsCookieAcceptanceResult) response))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.ToggleAnalyticsCookieAcceptance(request);

            // Assert
            _termsAndConditionsService.Verify(x =>
                x.ToggleAnalyticsCookieAcceptance(_userSession.GpUserSession.NhsNumber, request,
                    It.IsAny<DateTimeOffset>()));
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }
    }
}