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
        
        public static class ErrorCodes
        {
            public const int SessionEndErrorCode = 480;
        }
        
        public static class IssueCodes
        {
            public const string SessionEnd = "SESSION_ENDED";
        }
    }
}