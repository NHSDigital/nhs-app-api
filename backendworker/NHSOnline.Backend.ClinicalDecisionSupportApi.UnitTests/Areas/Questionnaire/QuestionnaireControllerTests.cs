using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Areas.Questionnaire;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir;
using UnitTestHelper;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.UnitTests.Areas.Questionnaire
{
    [TestClass]
    public class QuestionnaireControllerTests
    {
        private IFixture _fixture;
        private QuestionnaireController _controller;
        private Mock<IQuestionnaireService> _mockQuestionnaireService;
        private FhirQuestionnaire _questionnaireResponse;
        
        private const string TestId = "1";
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            
            // Disabling due to circular reference: FhirItem containing nested FhirItems
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _mockQuestionnaireService = _fixture.Freeze<Mock<IQuestionnaireService>>();
            
            _questionnaireResponse = _fixture.Create<FhirQuestionnaire>();
            var successfulResult = new QuestionnaireResult.SuccessfullyRetrieved(_questionnaireResponse);
            
            _mockQuestionnaireService.Setup(x => x.GetQuestionnaireById(TestId))
                .Returns(Task.FromResult((QuestionnaireResult)successfulResult));
            
            _controller = _fixture.Create<QuestionnaireController>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public async Task GetQuestionnaireById_WhenCalledWithNullOrEmptyId_ReturnsBadRequest(string id)
        {
            // Act
            var actualResponse = await _controller.GetQuestionnaireById(id);

            // Assert
            var value = actualResponse.Should().BeAssignableTo<BadRequestResult>();
            value.Subject.StatusCode.Should().Be(400);
        }

        [TestMethod]
        public async Task GetQuestionnaireById_WhenCalledWithValidId_ReturnsSuccess()
        {
            // Act
            var actualResponse = await _controller.GetQuestionnaireById(TestId);

            // Assert
            _mockQuestionnaireService.Verify();
            var value = actualResponse.Should().BeAssignableTo<OkObjectResult>();
            value.Subject.StatusCode.Should().Be(200);
        }
    }
}