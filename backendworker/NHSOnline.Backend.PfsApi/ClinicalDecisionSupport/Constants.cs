using System.Collections.Generic;
using System.Collections.Immutable;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport
{
    public static class Constants
    {
        public static class CdsApiEndpoints
        {
            // Ensure CensorFilters are added to/updated, if part of the path could be considered PII, e.g. ServiceDefinition IDs
            public const string EvaluateServiceDefinitionPathFormat = "fhir/ServiceDefinition/{0}/$evaluate";
            public const string ServiceDefinitionIsValidPath = "fhir/ServiceDefinition/$isValid";
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

        public static readonly ImmutableDictionary<ServiceDefinitionType, string> ServiceDefinitionDescriptions =
            new Dictionary<ServiceDefinitionType, string>
            {
                { ServiceDefinitionType.Unknown, "an unknown journey" },
                { ServiceDefinitionType.AdminHelp, "admin help" },
                { ServiceDefinitionType.ConditionList, "choosing a condition" },
                { ServiceDefinitionType.GeneralAdvice, "general Gp advice" },
                { ServiceDefinitionType.ConditionAdvice, "a chosen condition" }
            }.ToImmutableDictionary();
    }
}