using System.Collections.Generic;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.PfsApi.UnitTests.ClinicalDecisionSupport.Utils
{
    [TestClass]
    public class FhirSanitizationHelperTests
    {
        private Mock<IHtmlSanitizer> _mockIHtmlSanitizer;
        private FhirSanitizationHelper _fhirSanitizationHelper;
        private const string GuidanceResponse = "{ \"resourceType\": \"GuidanceResponse\", \"contained\": [ { \"resourceType\": \"Questionnaire\", \"id\": \"CONDITION_LIST\", \"status\": \"active\", \"item\": [ { \"linkId\": \"ALG\", \"text\": \"Allergies &amp; Problems\", \"type\": \"group\", \"required\": false, \"item\": [ { \"linkId\": \"ALG_HFV\", \"text\": \"Hay fever\", \"type\": \"boolean\", \"required\": false }, { \"linkId\": \"ALG_URT_ALIAS_0\", \"text\": \"Hives\", \"type\": \"boolean\", \"required\": false }, { \"linkId\": \"ALG_URT\", \"text\": \"Urticaria\", \"type\": \"boolean\", \"required\": false } ] } ] } ], \"module\": { \"reference\": \"https://test/fhir/ServiceDefinition/CONDITION_LIST\" }, \"status\": \"data-required\", \"occurrenceDateTime\": \"2019-11-08T14:39:45.117\", \"dataRequirement\": [ { \"id\": \"PATIENT\", \"type\": \"Patient\", \"profile\": [ \"https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Patient-1\" ] }, { \"id\": \"ORG\", \"type\": \"Organization\", \"profile\": [ \"https://fhir.hl7.org.uk/STU3/StructureDefinition/CareConnect-Organization-1\" ], \"codeFilter\": [ { \"path\": \"identifier\", \"valueSetString\": \"odsOrganizationCode\" } ] }, { \"id\": \"CONDITION_LIST\", \"extension\": [ { \"url\": \"https://www.hl7.org/fhir/questionnaire.html\", \"valueReference\": { \"reference\": \"#CONDITION_LIST\" } } ], \"type\": \"QuestionnaireResponse\", \"profile\": [ \"https://www.hl7.org/fhir/questionnaireresponse.html\" ] } ] }";
        private const string GuidanceResponseWithResult = "{ \"resourceType\": \"GuidanceResponse\", \"contained\": [ { \"resourceType\": \"Parameters\", \"id\": \"outputParams\", \"parameter\": [ { \"name\": \"sessionId\", \"valueString\": \"6e59eadf-69cc-4dfd-a641-e082a0467a59\" } ] }, { \"resourceType\": \"CarePlan\", \"id\": \"careplan1\", \"status\": \"active\", \"intent\": \"option\", \"title\": \"What happens next &amp;?\", \"activity\": [ { \"detail\": { \"description\": \"Test &amp; text\" } } ] }, { \"resourceType\": \"ReferralRequest\", \"id\": \"rr1\", \"description\": \"Thanks &amp; your referred\" }, { \"resourceType\": \"RequestGroup\", \"id\": \"rg1\", \"action\": [ { \"resource\": { \"reference\": \"#careplan1\" } }, { \"resource\": { \"reference\": \"#rr1\" } } ] } ], \"module\": { \"reference\": \"https://test/fhir/ServiceDefinition/GEC_ADM\" }, \"status\": \"success\", \"occurrenceDateTime\": \"2019-11-11T09:11:04.653\", \"outputParameters\": { \"reference\": \"#outputParams\" }, \"result\": { \"reference\": \"#rg1\" } }";
        private const string GuidanceResponseOptions =
            "{ \"resourceType\": \"GuidanceResponse\", \"contained\": [ { \"resourceType\": \"Parameters\", \"id\": \"outputParams\", \"parameter\": [ { \"name\": \"sessionId\", \"valueString\": \"0b0ee9da-ec15-4e26-886b-536936edeb0e\" } ] }, { \"resourceType\": \"Questionnaire\", \"id\": \"PRE_STD_AD_EMERGENCY\", \"status\": \"active\", \"item\": [ { \"linkId\": \"PRE_STD_AD_EMERGENCY_GROUP\", \"text\": \"test &amp; testing\", \"type\": \"group\", \"item\": [ { \"extension\": [ { \"url\": \"http://hl7.org/fhir/StructureDefinition/questionnaire-itemControl\", \"valueCodeableConcept\": { \"id\": \"backButton\", \"coding\": [ { \"system\": \"http://hl7.org/fhir/ValueSet/questionnaire-item-control\", \"code\": \"button\", \"display\": \"back\" } ], \"text\": \"back\" } } ], \"linkId\": \"PRE_STD_AD_EMERGENCY_PREV\", \"type\": \"boolean\", \"required\": false }, { \"linkId\": \"PRE_STD_AD_EMERGENCY\", \"text\": \"test &amp; testing\", \"type\": \"choice\", \"required\": true, \"repeats\": false, \"option\": [ { \"valueCoding\": { \"code\": \"PRE_STD_EMERGENCY_NO\", \"display\": \"test &amp; testing\" } }, { \"valueCoding\": { \"code\": \"PRE_STD_EMERGENCY_YES\", \"display\": \"I am experiencing some of these\" } } ] } ] } ] } ], \"module\": { \"reference\": \"https://test/fhir/ServiceDefinition/GEC_ADM\" }, \"status\": \"data-required\", \"occurrenceDateTime\": \"2019-11-11T10:53:01.306\", \"outputParameters\": { \"reference\": \"#outputParams\" }, \"dataRequirement\": [ { \"id\": \"PRE_STD_AD_EMERGENCY\", \"extension\": [ { \"url\": \"https://www.hl7.org/fhir/questionnaire.html\", \"valueReference\": { \"reference\": \"#PRE_STD_AD_EMERGENCY\" } } ], \"type\": \"QuestionnaireResponse\", \"profile\": [ \"https://www.hl7.org/fhir/questionnaireresponse.html\" ] } ] }";
  
        [TestInitialize]
        public void TestInitialize()
        {
            _fhirSanitizationHelper = new FhirSanitizationHelper();
            _mockIHtmlSanitizer = new Mock<IHtmlSanitizer>();
        }

        [TestMethod]
        public void Validate_DecodeGuidanceResponse_QuestionnaireItemText()
        {
            // Arrange
            var fhirParser = new FhirJsonParser();
            var parsedGuidanceResponse = (GuidanceResponse) fhirParser.Parse(GuidanceResponse);
            _mockIHtmlSanitizer.Setup(fsh => fsh.SanitizeHtml("Allergies &amp; Problems"))
                .Returns("Allergies &amp; Problems");
            
            // Act
            _fhirSanitizationHelper.SanitizeGuidanceResponse(parsedGuidanceResponse, _mockIHtmlSanitizer.Object);

            // Assert
            var contained = parsedGuidanceResponse.Contained;
            var questionnaire = (Questionnaire) contained[0];
            var text = questionnaire.Item[0].Text;

            Assert.AreEqual("Allergies & Problems", text);
        }
        
        [TestMethod]
        public void Validate_DecodeGuidanceResponse_CarePlanText()
        {
            // Arrange
            var fhirParser = new FhirJsonParser();
            var parsedGuidanceResponse = (GuidanceResponse) fhirParser.Parse(GuidanceResponseWithResult);
            _mockIHtmlSanitizer.Setup(fsh => fsh.SanitizeHtml("What happens next &amp;?"))
                .Returns("What happens next &amp;?");
            
            // Act
            _fhirSanitizationHelper.SanitizeGuidanceResponse(parsedGuidanceResponse, _mockIHtmlSanitizer.Object);

            // Assert
            var contained = parsedGuidanceResponse.Contained;
            var carePlan = (CarePlan) contained[1];
            var text = carePlan.Title;

            Assert.AreEqual("What happens next &?", text);
        }
        
        [TestMethod]
        public void Validate_DecodeGuidanceResponse_ReferralRequestText()
        {
            // Arrange
            var fhirParser = new FhirJsonParser();
            var parsedGuidanceResponse = (GuidanceResponse) fhirParser.Parse(GuidanceResponseWithResult);
            _mockIHtmlSanitizer.Setup(fsh => fsh.SanitizeHtml("Thanks &amp; your referred"))
                .Returns("Thanks & your referred");
            
            // Act
            _fhirSanitizationHelper.SanitizeGuidanceResponse(parsedGuidanceResponse, _mockIHtmlSanitizer.Object);

            // Assert
            var contained = parsedGuidanceResponse.Contained;
            var referralRequest = (ReferralRequest) contained[2];
            var text = referralRequest.Description;

            Assert.AreEqual("Thanks & your referred", text);
        }
        
        [TestMethod]
        public void Validate_DecodeGuidanceResponse_OptionValue()
        {
            // Arrange
            var fhirParser = new FhirJsonParser();
            var parsedGuidanceResponse = (GuidanceResponse) fhirParser.Parse(GuidanceResponseOptions);
            _mockIHtmlSanitizer.Setup(fsh => fsh.SanitizeHtml("test &amp; testing"))
                .Returns("test & testing");
            
            // Act
            _fhirSanitizationHelper.SanitizeGuidanceResponse(parsedGuidanceResponse, _mockIHtmlSanitizer.Object);

            // Assert
            var contained = parsedGuidanceResponse.Contained;
            var questionnaire = (Questionnaire) contained[1];
            var item = questionnaire.Item[0].Item[1].Option[0];
            var itemValue = (Coding) item.Value;
            var valueDisplay = itemValue.Display;

            Assert.AreEqual("test & testing", valueDisplay);
        }
        
        [TestMethod]
        public void Validate_DecodeGuidanceResponse_CarePlanActivityDetail()
        {
            // Arrange
            var fhirParser = new FhirJsonParser();
            var parsedGuidanceResponse = (GuidanceResponse) fhirParser.Parse(GuidanceResponseWithResult);
            _mockIHtmlSanitizer.Setup(fsh => fsh.SanitizeHtml("Test &amp; text"))
                .Returns("Test & text");
            
            // Act
            _fhirSanitizationHelper.SanitizeGuidanceResponse(parsedGuidanceResponse, _mockIHtmlSanitizer.Object);

            // Assert
            var contained = parsedGuidanceResponse.Contained;
            var carePlan = (CarePlan) contained[1];
            var activity = carePlan.Activity[0];
            var text = activity.Detail.Description;

            Assert.AreEqual("Test & text", text);
        }
    }
}