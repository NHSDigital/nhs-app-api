using System.Text.Json;

namespace NHSOnline.HttpMocks.Domain
{
    public sealed class VisionPatient : Patient, IGpRegistered
    {
        public override string VectorOfTrust => "P9.Cp.Cd";
        public override string ProofingLevel => "P9";

        public string OdsCode => "X00100";
        public string Im1ConnectionToken => JsonSerializer.Serialize(new { RosuAccountId = Id, ApiKey });

        public string ApiKey { get; } = "6efbfb6d60d25e5269850de1f033eb792796e69d0e946248834336cf0f49046d";
    }
}