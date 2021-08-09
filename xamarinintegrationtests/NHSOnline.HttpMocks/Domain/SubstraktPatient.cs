using System.Text.Json;

namespace NHSOnline.HttpMocks.Domain
{
    public sealed class SubstraktPatient : Patient, IGpRegistered
    {
        public override string VectorOfTrust { get; internal set; } = "P9.Cp.Cd";
        public override string ProofingLevel { get; internal set; } = "P9";

        public string OdsCode => "fake_with_substrakt";
        public string Im1ConnectionToken { get; private set; } = string.Empty;

        public void CreateIm1ConnectionToken()
        {
            Im1ConnectionToken = JsonSerializer.Serialize(new {Im1CacheKey = "Im1CacheKey", AccessIdentityGuid = Id});
        }
    }
}