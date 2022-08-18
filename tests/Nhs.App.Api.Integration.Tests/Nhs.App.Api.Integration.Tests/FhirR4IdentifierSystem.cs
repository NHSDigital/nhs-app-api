namespace Nhs.App.Api.Integration.Tests
{
    public static class FhirR4IdentifierSystem
    {
        // Recipient systems
        public const string NhsNumber = "https://fhir.nhs.uk/Id/nhs-number";

        // Identifier systems
        public const string CampaignId = "https://fhir.nhs.uk/NHSApp/campaign-id";
        public const string RequestReference = "https://fhir.nhs.uk/NHSApp/request-id";
        public const string Requester = "https://fhir.nhs.uk/Id/ods-organization-code";
        public const string CommunicationId = "https://fhir.nhs.uk/Id/nhs-app-communication-id";
    }
}
