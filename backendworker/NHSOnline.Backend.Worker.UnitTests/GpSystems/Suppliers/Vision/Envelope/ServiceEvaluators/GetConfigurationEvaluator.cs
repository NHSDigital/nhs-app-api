using Microsoft.VisualStudio.TestTools.UnitTesting;

public class GetConfigurationEvaluator : AbstractXmlEvaluator
{
    const string visionRequestXpath = soapBody + "vision:visionRequest/vision:serviceHeader/";
    private const string visionCredentialsXpath = visionRequestXpath + "vision:credentials/";
    private const string visionOpsXpath = visionRequestXpath + "vision:opsReference/";

    public GetConfigurationEvaluator(string envelope) : base(envelope)
    {
    }

    public void Validate(string serviceDefinition, string serviceVersion, string rosuAccountId, string apiKey,
        string odsCode, string providerId)
    {
        ValidateServiceDefinition(serviceDefinition, serviceVersion);
        Assert.IsTrue((bool) nav.Evaluate(
            visionRequestXpath + $"/vision:target/vision:nationalCode[text()='{odsCode}'])",
            manager), $"Failed to find odsCode: {odsCode}");
        Assert.IsTrue((bool) nav.Evaluate(
            visionCredentialsXpath + $"vision:rosuAccountId[text()='{rosuAccountId}'])",
            manager), $"Failed to find rosuAccountId: {rosuAccountId}");
        Assert.IsTrue((bool) nav.Evaluate(visionCredentialsXpath + $"vision:apiKey[text()='{apiKey}'])",
            manager), $"Failed to find apiKey: {apiKey}");
        Assert.IsTrue((bool) nav.Evaluate(visionOpsXpath + $"vision:provider[text()='{providerId}'])",
            manager), $"Failed to find providerId: {providerId}");
        Assert.IsTrue((bool) nav.Evaluate(visionOpsXpath + $"vision:accountId[text()='{providerId}'])",
            manager), $"Failed to find accountId: {providerId}");
    }
}
