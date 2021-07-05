using System;
using System.Text.Json;

namespace NHSOnline.HttpMocks.Domain
{
    public sealed class EmisPatient : Patient, IGpRegistered
    {
        public override string VectorOfTrust => "P9.Cp.Cd";
        public override string ProofingLevel => "P9";

        public string OdsCode { get; }

        public string Im1ConnectionToken => JsonSerializer.Serialize(new {Im1CacheKey = "Im1CacheKey", AccessIdentityGuid = Id});

        public string UserPatientLinkToken => $"linktoken_{Id}";
        public string SessionId => $"session{Id}";

        public EmisPatient(EmisPatientOds emisPatientOds = EmisPatientOds.PkbAndEconsult)
        {
            OdsCode = emisPatientOds switch
            {
                EmisPatientOds.NotificationsPromptEnabled => "emis_with_notifications_prompt_enabled",
                EmisPatientOds.PkbAndEconsult => "emis_with_pkb_and_econsult",
                EmisPatientOds.AllSilversEnabled => "emis_with_all_silvers",
                _ => throw new ArgumentOutOfRangeException(nameof(emisPatientOds), emisPatientOds, null)
            };
        }


    }
}
