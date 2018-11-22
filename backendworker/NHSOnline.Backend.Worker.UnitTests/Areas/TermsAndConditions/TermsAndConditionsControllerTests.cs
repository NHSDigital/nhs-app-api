using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.TermsAndConditions;
using NHSOnline.Backend.Worker.Areas.TermsAndConditions.Models;
using NHSOnline.Backend.Worker.TermsAndConditions;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.TermsAndConditions
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
            _userSession.NhsNumber = _fixture.Create<string>();
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
        public async Task Post_Returns_Success()
        {
            // Arrange
            var request = _fixture.Create<ConsentRequest>();
            var response = new TermsAndConditionsRecordConsentResult.ConsentRecorded();
            _termsAndConditionsService.Setup(x => x.RecordConsent(_userSession.NhsNumber, request, It.IsAny<DateTimeOffset>())).Returns(Task.FromResult((TermsAndConditionsRecordConsentResult)response));
            
            // Act
            var result = await _systemUnderTest.Post(request);

            // Assert
            _termsAndConditionsService.Verify(x => x.RecordConsent(_userSession.NhsNumber, request, It.IsAny<DateTimeOffset>()));
            var okObjectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            okObjectResult.Value.Should().BeAssignableTo<TermsAndConditionsRecordConsentResult.ConsentRecorded>();
        }

        [TestMethod]
        public async Task Post_Returns_Failure()
        {
            // Arrange
            var request = _fixture.Create<ConsentRequest>();
            var response = new TermsAndConditionsRecordConsentResult.FailureToRecordConsent();
            _termsAndConditionsService.Setup(x => x.RecordConsent(_userSession.NhsNumber, request, It.IsAny<DateTimeOffset>())).Returns(Task.FromResult((TermsAndConditionsRecordConsentResult)response));
            
            // Act
            var result = await _systemUnderTest.Post(request);

            // Assert
            _termsAndConditionsService.Verify(x => x.RecordConsent(_userSession.NhsNumber, request, It.IsAny<DateTimeOffset>()));
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(Constants.CustomHttpStatusCodes.Status462FailedToRecordConsent);
        }

        [TestMethod]
        public async Task Get_Returns_Success()
        {
            // Arrange
            var consentRecord = _fixture.Create<ConsentResponse>();
            var response = new TermsAndConditionsFetchConsentResult.Success(consentRecord);
            _termsAndConditionsService.Setup(x => x.FetchConsent(_userSession.NhsNumber)).Returns(Task.FromResult((TermsAndConditionsFetchConsentResult)response));
            
            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _termsAndConditionsService.Verify(x => x.FetchConsent(_userSession.NhsNumber));
            var okObjectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            var value = okObjectResult.Value.Should().BeAssignableTo<TermsAndConditionsFetchConsentResult.Success>()
                .Subject;
            value.Response.ConsentGiven.Should().Be(consentRecord.ConsentGiven);
        }

        [TestMethod]
        public async Task Get_Returns_NotFound()
        {
            // Arrange
            var response = new TermsAndConditionsFetchConsentResult.NoConsentFound();
            _termsAndConditionsService.Setup(x => x.FetchConsent(_userSession.NhsNumber)).Returns(Task.FromResult((TermsAndConditionsFetchConsentResult)response));
            
            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _termsAndConditionsService.Verify(x => x.FetchConsent(_userSession.NhsNumber));
            var okObjectResult = result.Should().BeAssignableTo<OkObjectResult>().Subject;
            okObjectResult.Value.Should().BeAssignableTo<TermsAndConditionsFetchConsentResult.NoConsentFound>();
        }

        [TestMethod]
        public async Task Get_Returns_Failure()
        {
            // Arrange
            var response = new TermsAndConditionsFetchConsentResult.FailureToFetchConsent();
            _termsAndConditionsService.Setup(x => x.FetchConsent(_userSession.NhsNumber)).Returns(Task.FromResult((TermsAndConditionsFetchConsentResult)response));
            
            // Act
            var result = await _systemUnderTest.Get();

            // Assert
            _termsAndConditionsService.Verify(x => x.FetchConsent(_userSession.NhsNumber));
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(Constants.CustomHttpStatusCodes.Status463FailedToFetchConsent);
        }
    }
}