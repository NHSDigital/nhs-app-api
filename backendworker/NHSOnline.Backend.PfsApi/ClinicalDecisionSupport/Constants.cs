namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport
{
    public static class Constants
    {
        public static class CdsApiEndpoints
        {
            public const string EvaluateServiceDefinitionPathFormat = "fhir/ServiceDefinition/{0}/$evaluate";
        }

        public static class ContentTypes
        {
            public const string ApplicationJsonFhir = "application/json+fhir";
        }
    }
}