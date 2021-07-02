using System.Text.Json;

namespace NHSOnline.HttpMocks.Domain
{
    public sealed class KeyboardPatient : Patient, IGpRegistered
    {
        public override string VectorOfTrust => "P9.Cp.Cd";
        public override string ProofingLevel => "P9";

        public string OdsCode => "emis_with_all_silvers";
        public string Im1ConnectionToken => JsonSerializer.Serialize(new {Im1CacheKey = "Im1CacheKey", AccessIdentityGuid = Id});
    }
}