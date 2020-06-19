using System.Text.Json;

namespace NHSOnline.HttpMocks.Domain
{
    public sealed class EmisPatient : Patient, IGpRegistered
    {
        public override string VectorOfTrust => "P9.Cp.Cd";
        public override string ProofingLevel => "P9";

        public string OdsCode => "A29928";
        public string Im1ConnectionToken => JsonSerializer.Serialize(new {Im1CacheKey = "Im1CacheKey", AccessIdentityGuid = Id});

        public string UserPatientLinkToken => $"linktoken_{Id}";
        public string SessionId => $"session{Id}";
    }
}