using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Envelope.ServiceEvaluators
{
    public class GetConfigurationEvaluator : AbstractXmlEvaluator
    {
        private const string VisionRequestXpath = SoapBody + "vision:visionRequest/vision:serviceHeader/";
        private const string VisionCredentialsXpath = VisionRequestXpath + "vision:credentials/";
        private const string VisionOpsXpath = VisionRequestXpath + "vision:opsReference/";

        public GetConfigurationEvaluator(string envelope) : base(envelope)
        {
        }

        public void Validate(string serviceDefinition, string serviceVersion, string rosuAccountId, string apiKey,
            string odsCode, string providerId)
        {
            ValidateServiceDefinition(serviceDefinition, serviceVersion);
            Assert.IsTrue((bool) Nav.Evaluate(
                VisionRequestXpath + $"/vision:target/vision:nationalCode[text()='{odsCode}'])",
                Manager), $"Failed to find odsCode: {odsCode}");
            Assert.IsTrue((bool) Nav.Evaluate(
                VisionCredentialsXpath + $"vision:rosuAccountId[text()='{rosuAccountId}'])",
                Manager), $"Failed to find rosuAccountId: {rosuAccountId}");
            Assert.IsTrue((bool) Nav.Evaluate(VisionCredentialsXpath + $"vision:apiKey[text()='{apiKey}'])",
                Manager), $"Failed to find apiKey: {apiKey}");
            Assert.IsTrue((bool) Nav.Evaluate(VisionOpsXpath + $"vision:provider[text()='{providerId}'])",
                Manager), $"Failed to find providerId: {providerId}");
            Assert.IsTrue((bool) Nav.Evaluate(VisionOpsXpath + $"vision:accountId[text()='{providerId}'])",
                Manager), $"Failed to find accountId: {providerId}");
        }
    }

}
