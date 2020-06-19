using System.Text.Json;

namespace NHSOnline.HttpMocks.Domain
{
    public sealed class MicrotestPatient : Patient, IGpRegistered
    {
        public override string VectorOfTrust => "P9.Cp.Cd";
        public override string ProofingLevel => "P9";

        public string OdsCode => "A21410";
        public string Im1ConnectionToken => JsonSerializer.Serialize(new { Im1CacheKey = "Im1CacheKey", NhsNumber = NhsNumber.StringValue });
    }
}