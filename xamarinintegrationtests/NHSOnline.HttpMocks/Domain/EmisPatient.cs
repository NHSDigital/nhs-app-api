using System;
using System.Text.Json;

namespace NHSOnline.HttpMocks.Domain
{
    public sealed class EmisPatient : Patient, IGpRegistered
    {
        public override string VectorOfTrust { get; internal set; } = "P9.Cp.Cd";
        public override string ProofingLevel { get; internal set; } = "P9";

        public string OdsCode { get; }
        public string Im1ConnectionToken { get; private set; } = string.Empty;

        public string UserPatientLinkToken => $"linktoken_{Id}";
        public string SessionId => $"session{Id}";

        public EmisPatient(EmisPatientOds emisPatientOds = EmisPatientOds.PkbAndEconsult)
        {
            OdsCode = emisPatientOds.ToOdsCodeString();
        }

        public void CreateIm1ConnectionToken()
        {
            Im1ConnectionToken = JsonSerializer.Serialize(new {Im1CacheKey = "Im1CacheKey", AccessIdentityGuid = Id});
        }
    }
}
