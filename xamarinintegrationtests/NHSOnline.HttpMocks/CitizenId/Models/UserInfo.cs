using System.Text.Json.Serialization;
using NHSOnline.HttpMocks.Domain;

// ReSharper disable UnusedMember.Global

namespace NHSOnline.HttpMocks.CitizenId.Models
{
    internal sealed class UserInfo
    {
        private readonly Patient _patient;

        public UserInfo(Patient patient)
        {
            _patient = patient;
        }

        [JsonPropertyName("given_name")]
        public string GivenName => _patient.PersonalDetails.Name.GivenName;

        [JsonPropertyName("family_name")]
        public string FamilyName => _patient.PersonalDetails.Name.FamilyName;

        [JsonPropertyName("gp_integration_credentials")]
        public GpIntegrationCredentials? GpIntegrationCredentials =>
            _patient switch
            {
                IGpRegistered registered => new GpIntegrationCredentials(registered.OdsCode),
                _ => null
            };

        [JsonPropertyName("nhs_number")]
        public string NhsNumber => _patient.NhsNumber.FormattedStringValue;

        [JsonPropertyName("im1_token")]
        public string? Im1ConnectionToken =>
            _patient switch
            {
                IGpRegistered registered => registered.Im1ConnectionToken,
                _ => null
            };

        [JsonPropertyName("sub")]
        public string Subject => _patient.Id;

        [JsonPropertyName("birthdate")]
        public string Birthdate => _patient.PersonalDetails.Age.DateOfBirthISO86012004;

        [JsonPropertyName("identity_proofing_level")]
        public string IdentityProofingLevel => _patient.ProofingLevel;
    }
}
