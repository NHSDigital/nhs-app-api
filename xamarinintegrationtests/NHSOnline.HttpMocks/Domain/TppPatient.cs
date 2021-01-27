using System.Text.Json;

namespace NHSOnline.HttpMocks.Domain
{
    public sealed class TppPatient : Patient, IGpRegistered
    {
        public override string VectorOfTrust => "P9.Cp.Cd";
        public override string ProofingLevel => "P9";

        public string OdsCode => "A82648";
        public string Im1ConnectionToken => JsonSerializer.Serialize(new { AccountId = Id, ProviderId, Passphrase});

        public string ProviderId { get; } = "Tpp";
        public string Passphrase { get; } = "secret";
    }
}