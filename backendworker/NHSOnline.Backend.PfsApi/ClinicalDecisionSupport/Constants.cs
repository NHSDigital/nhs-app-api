namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport
{
    public static class Constants
    {
        public static class CdsApiEndpoints
        {
            public const string ServiceDefinitionPath = "fhir/ServiceDefinition";
            public const string EvaluateServiceDefinitionPathFormat = "fhir/ServiceDefinition/{0}/$evaluate";
            public const string GetServiceDefinitionByIdPathFormat = "fhir/ServiceDefinition?_id={0}";
        }

        public static class ContentTypes
        {
            public const string ApplicationJsonFhir = "application/json+fhir";
        }

        public static class HttpRequestHeaderValues
        {
            public const string AuthorizationFormat = "Bearer {0}";
        }
    }
}