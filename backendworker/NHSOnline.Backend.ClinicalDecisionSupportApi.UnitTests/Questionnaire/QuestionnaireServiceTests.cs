using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.UnitTests.Questionnaire
{
    [TestClass]
    public class QuestionnaireServiceTests
    {
        private IQuestionnaireService _questionnaireService;
        private Mock<IQuestionnaireClient> _questionnaireClient;
        private Mock<IHtmlSanitizer> _htmlSanitizer;
        private IFixture _fixture;
        private ILogger<QuestionnaireService> _logger;
        
        private const string TestId = "1";
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<QuestionnaireService>>();
            _questionnaireClient = _fixture.Freeze<Mock<IQuestionnaireClient>>();
            _htmlSanitizer = new Mock<IHtmlSanitizer>();
            
            _questionnaireService = new QuestionnaireService(_questionnaireClient.Object, _logger, _htmlSanitizer.Object);
        }

        [TestMethod]
        public async Task GetQuestionnaireById_WhenClientGetQuestionnaireByIdRequestIsUnsuccessful_ReturnsUnsuccessfulResult()
        {
            // Arrange

            _questionnaireClient
                .Setup(qc => qc.GetQuestionnaireById(It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new FhirApiQuestionnaireResponse<FhirQuestionnaire>(HttpStatusCode.Forbidden)));
            
            // Act

            var result = await _questionnaireService.GetQuestionnaireById(TestId);
            
            // Assert

            result.Should().BeAssignableTo<QuestionnaireResult.Unsuccessful>();
        }

        [TestMethod]
        public async Task GetQuestionnaireById_WhenQuestionnaireResponseBodyIsNull_ReturnsUnsuccessfulResult()
        {
            // Arrange

            _questionnaireClient
                .Setup(qc => qc.GetQuestionnaireById(It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new FhirApiQuestionnaireResponse<FhirQuestionnaire>(HttpStatusCode.OK)));
            
            // Act

            var result = await _questionnaireService.GetQuestionnaireById(TestId);
            
            // Assert

            result.Should().BeAssignableTo<QuestionnaireResult.Unsuccessful>();
        }

        [TestMethod]
        public async Task GetQuestionnaireById_WhenHttpRequestExceptionIsThrown_ReturnsUnsuccessful()
        {
            // Arrange

            _questionnaireClient
                .Setup(qc => qc.GetQuestionnaireById(It.IsAny<string>()))
                .Throws<HttpRequestException>();

            // Act

            var result = await _questionnaireService.GetQuestionnaireById(TestId);
            
            // Assert

            result.Should().BeAssignableTo<QuestionnaireResult.Unsuccessful>();
        }

        [TestMethod]
        public async Task GetQuestionnaireById_WhenGenericExceptionIsThrown_ReturnsUnsuccessful()
        {
            // Arrange

            _questionnaireClient
                .Setup(qc => qc.GetQuestionnaireById(It.IsAny<string>()))
                .Throws<Exception>();

            // Act

            var result = await _questionnaireService.GetQuestionnaireById(TestId);
            
            // Assert

            result.Should().BeAssignableTo<QuestionnaireResult.Unsuccessful>();
        }

        [TestMethod]
        public async Task GetQuestionnaireById_WhenResponseContainsQuestionnaire_WillSanitizeQuestionnaireItemText()
        {
            // Arrange

            const string dirtyText = "This is a question with <a>a link</a>";
            const string sanitizedText = "This is a question with a link";

            var mockResponse = new FhirApiQuestionnaireResponse<FhirQuestionnaire>(HttpStatusCode.OK)
            {
                Body = new FhirQuestionnaire
                {
                    Item = new List<FhirItem>
                    {
                        new FhirItem
                        {
                            Text = dirtyText
                        }
                    }
                }
            };
            
            _questionnaireClient
                .Setup(qc => qc.GetQuestionnaireById(It.IsAny<string>()))
                .Returns(Task.FromResult(mockResponse));

            _htmlSanitizer
                .Setup(hs => hs.SanitizeHtml(It.Is<string>(v => v.Equals(dirtyText, StringComparison.Ordinal))))
                .Returns(sanitizedText);
            
            // Act

            var result = await _questionnaireService.GetQuestionnaireById(TestId);
            
            // Assert

            result.Should().BeAssignableTo<QuestionnaireResult.SuccessfullyRetrieved>();
            ((QuestionnaireResult.SuccessfullyRetrieved) result).Response.Item[0].Text.Should().Be(sanitizedText);
        }

        [TestMethod]
        public async Task GetQuestionnaireById_WhenResponseQuestionnaireContainsNoItems_WillNotCallSanitizeHtml()
        {
            // Arrange

            var mockResponse = new FhirApiQuestionnaireResponse<FhirQuestionnaire>(HttpStatusCode.OK)
            {
                Body = new FhirQuestionnaire
                {
                    Item = null
                }
            };
            
            _questionnaireClient
                .Setup(qc => qc.GetQuestionnaireById(It.IsAny<string>()))
                .Returns(Task.FromResult(mockResponse));
            
            // Act

            var result = await _questionnaireService.GetQuestionnaireById(TestId);
            
            // Assert

            result.Should().BeAssignableTo<QuestionnaireResult.SuccessfullyRetrieved>();
            _htmlSanitizer.Verify(hs => hs.SanitizeHtml(It.IsAny<string>()), Times.Never);
        }
    }
}