using System.Text.Json;

namespace NHSOnline.HttpMocks.Domain
{
    public sealed class VisionPatient : Patient, IGpRegistered
    {
        public override string VectorOfTrust { get; internal set; } = "P9.Cp.Cd";
        public override string ProofingLevel { get; internal set; } = "P9";

        public string OdsCode => "vision_with_pkb";
        public string Im1ConnectionToken { get; private set; } = string.Empty;

        public void CreateIm1ConnectionToken()
        {
            Im1ConnectionToken = JsonSerializer.Serialize(new { RosuAccountId = Id, ApiKey = "6efbfb6d60d25e5269850de1f033eb792796e69d0e946248834336cf0f49046d" });
        }
    }
}