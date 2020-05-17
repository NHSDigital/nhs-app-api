using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils;
using NhsAppFhir = NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models.Fhir;

namespace NHSOnline.Backend.PfsApi.UnitTests.ClinicalDecisionSupport.Utils
{
    [TestClass]
    public class FhirParameterHelpersTests
    {
        private Mock<ILogger<IFhirParameterHelpers>> _mockLogger;

        private FhirParameterHelpers _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<IFhirParameterHelpers>>();

            _systemUnderTest = new FhirParameterHelpers(_mockLogger.Object);
        }

        [DataTestMethod]
        [DataRow("Unknown", ServiceDefinitionType.Unknown)]
        [DataRow("AdminHelp", ServiceDefinitionType.AdminHelp)]
        [DataRow("ConditionList", ServiceDefinitionType.ConditionList)]
        [DataRow("GeneralAdvice", ServiceDefinitionType.GeneralAdvice)]
        [DataRow("ConditionAdvice", ServiceDefinitionType.ConditionAdvice)]
        [DataRow("AnotherUnknownType", ServiceDefinitionType.Unknown)]
        public void RemoveServiceDefinitionMetadataFromParameters_WhenBothNhsAppParametersAreProvided_ThenNhsAppParametersAreMappedCorrectlyIntoMetaData(
            string parameterServiceDefinitionType, ServiceDefinitionType expectedMetaDataType)
        {
            var parameters = GetDefaultParameters()
                .Add("nhsAppServiceDefinitionId", new FhirString("test-id"))
                .Add("nhsAppServiceDefinitionType", new FhirString(parameterServiceDefinitionType));

            _systemUnderTest.RemoveServiceDefinitionMetadataFromParameters(parameters, out var metaData);

            Assert.AreEqual("test-id", metaData.Id);
            Assert.AreEqual(expectedMetaDataType, metaData.Type);
        }

        [DataTestMethod]
        [DataRow("Unknown")]
        [DataRow("AdminHelp")]
        [DataRow("ConditionList")]
        [DataRow("GeneralAdvice")]
        [DataRow("ConditionAdvice")]
        [DataRow("AnotherUnknownType")]
        public void RemoveServiceDefinitionMetadataFromParameters_WhenBothNhsAppParametersAreProvided_ThenNhsAppParametersAreRemovedFromParameters(
            string parameterServiceDefinitionType)
        {
            var parameters = GetDefaultParameters()
                .Add("nhsAppServiceDefinitionId", new FhirString("test-id"))
                .Add("nhsAppServiceDefinitionType", new FhirString(parameterServiceDefinitionType));

            parameters = _systemUnderTest.RemoveServiceDefinitionMetadataFromParameters(parameters, out _);

            AssertNonNhsAppParametersRemain(parameters);
        }

        [DataTestMethod]
        [DataRow("nhsAppServiceDefinitionId", "test-id")]
        [DataRow("nhsAppServiceDefinitionType", "AdminHelp")]
        public void RemoveServiceDefinitionMetadataFromParameters_WhenAnNhsAppParameterIsMissing_ThenMetaDataIsNull(
            string parameterName, string parameterValue)
        {
            var parameters = GetDefaultParameters()
                .Add(parameterName, new FhirString(parameterValue));

            _systemUnderTest.RemoveServiceDefinitionMetadataFromParameters(parameters, out var metaData);

            Assert.IsNull(metaData);
        }

        [DataTestMethod]
        [DataRow("nhsAppServiceDefinitionId", "test-id")]
        [DataRow("nhsAppServiceDefinitionType", "AdminHelp")]
        public void RemoveServiceDefinitionMetadataFromParameters_WhenAnNhsAppParameterIsMissing_ThenAnyNhsAppParametersAreRemovedFromParameters(
            string parameterName, string parameterValue)
        {
            var parameters = GetDefaultParameters()
                .Add(parameterName, new FhirString(parameterValue));

            parameters = _systemUnderTest.RemoveServiceDefinitionMetadataFromParameters(parameters, out _);

            AssertNonNhsAppParametersRemain(parameters);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void RemoveServiceDefinitionMetadataFromParameters_WhenNhsAppIdParameterIsNullOrWhiteSpace_ThenMetaDataIsNull(
            string idParameter)
        {
            var parameters = GetDefaultParameters()
                .Add("nhsAppServiceDefinitionId", new FhirString(idParameter));

            _systemUnderTest.RemoveServiceDefinitionMetadataFromParameters(parameters, out var metaData);

            Assert.IsNull(metaData);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void RemoveServiceDefinitionMetadataFromParameters_WhenNhsAppIdParameterIsNullOrWhiteSpace_ThenNhsAppParametersAreRemovedFromParameters(
            string idParameter)
        {
            var parameters = GetDefaultParameters()
                .Add("nhsAppServiceDefinitionId", new FhirString(idParameter));

            parameters = _systemUnderTest.RemoveServiceDefinitionMetadataFromParameters(parameters, out _);

            AssertNonNhsAppParametersRemain(parameters);
        }

        [TestMethod]
        public void RemoveServiceDefinitionMetadataFromParameters_WhenNhsAppParametersAreNotFhirStrings_ThenMetaDataIsNull()
        {
            var parameters = GetDefaultParameters()
                .Add("nhsAppServiceDefinitionId", new FhirBoolean(true))
                .Add("nhsAppServiceDefinitionType", new FhirBoolean(false));

            _systemUnderTest.RemoveServiceDefinitionMetadataFromParameters(parameters, out var metaData);

            Assert.IsNull(metaData);
        }

        [TestMethod]
        public void RemoveServiceDefinitionMetadataFromParameters_WhenNhsAppParametersAreNotFhirStrings_ThenNhsAppParametersAreStillRemovedFromParameters()
        {
            var parameters = GetDefaultParameters()
                .Add("nhsAppServiceDefinitionId", new FhirBoolean(true))
                .Add("nhsAppServiceDefinitionType", new FhirBoolean(false));

            parameters = _systemUnderTest.RemoveServiceDefinitionMetadataFromParameters(parameters, out _);

            AssertNonNhsAppParametersRemain(parameters);
        }

        [TestMethod]
        public void RemoveServiceDefinitionMetadataFromParameters_WhenOnlyNhsAppParametersProvided_ThenMetaDataIsNull()
        {
            var parameters = new Parameters()
                .Add("nhsAppServiceDefinitionId", new FhirBoolean(true))
                .Add("nhsAppServiceDefinitionType", new FhirBoolean(false));

            _systemUnderTest.RemoveServiceDefinitionMetadataFromParameters(parameters, out var metaData);

            Assert.IsNull(metaData);
        }

        [TestMethod]
        public void RemoveServiceDefinitionMetadataFromParameters_WhenOnlyNhsAppParametersProvided_ThenEmptyParametersIsReturned()
        {
            var parameters = new Parameters()
                .Add("nhsAppServiceDefinitionId", new FhirBoolean(true))
                .Add("nhsAppServiceDefinitionType", new FhirBoolean(false));

            parameters = _systemUnderTest.RemoveServiceDefinitionMetadataFromParameters(parameters, out _);

            Assert.AreEqual(0, parameters.Parameter.Count);
        }

        [TestMethod]
        public void CreateInitialServiceDefinitionEvaluateParameters_WhenOdsCodeProvided_ThenParametersForInitialServiceDefinitionEvaluationAreReturned()
        {
            var parameters = _systemUnderTest.CreateInitialServiceDefinitionEvaluateParameters("D14562");

            Assert.AreEqual(1, parameters.Parameter.Count);
            AssertOrganizationParameterIsEqual(parameters.Parameter[0], "D14562");
        }

        [TestMethod]
        public void CreateServiceDefinitionIsValidParameters_WhenOdsCodeAndRequestIdAreProvided_ThenParametersForServiceDefinitionIsValidAreReturned()
        {
            var parameters = _systemUnderTest.CreateServiceDefinitionIsValidParameters("D12345", "what-is-a-request-id");

            Assert.AreEqual(2, parameters.Parameter.Count);
            AssertFhirStringParameterIsEqual(parameters.Parameter[0], "ODSCode", "D12345");
            AssertFhirStringParameterIsEqual(parameters.Parameter[1], "requestId", "what-is-a-request-id");
        }

        private static Parameters GetDefaultParameters() =>
            new Parameters().Add("nonNhsAppParameter", new FhirString("non nhs app parameter"));

        private static void AssertNonNhsAppParametersRemain(Parameters parameters)
        {
            Assert.AreEqual(1, parameters.Parameter.Count);
            AssertFhirStringParameterIsEqual(parameters.Parameter[0], "nonNhsAppParameter", "non nhs app parameter");
        }

        private static void AssertFhirStringParameterIsEqual(Parameters.ParameterComponent parameter, string expectedName, string expectedValue)
        {
            Assert.AreEqual(expectedName, parameter.Name);
            Assert.AreEqual(expectedValue, (parameter.Value as FhirString)?.Value);
        }

        private static void AssertOrganizationParameterIsEqual(NhsAppFhir.Parameter parameter, string odsCode)
        {
            Assert.AreEqual("organization", parameter.Name);
            Assert.AreEqual(odsCode, parameter.Resource.Identifier.Value);
        }
    }
}