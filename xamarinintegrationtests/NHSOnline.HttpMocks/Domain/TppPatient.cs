using System.Text.Json;

namespace NHSOnline.HttpMocks.Domain
{
    public sealed class TppPatient : Patient, IGpRegistered
    {
        public override string VectorOfTrust { get; internal set; } = "P9.Cp.Cd";
        public override string ProofingLevel { get; internal set; } = "P9";

        public string OdsCode => "tpp_with_pkb_and_covid_pass";
        public string Im1ConnectionToken { get; private set; } = string.Empty;

        public void CreateIm1ConnectionToken()
        {
            Im1ConnectionToken = JsonSerializer.Serialize(new { AccountId = Id, ProviderId = "Tpp", Passphrase = "secret"});
        }
    }
}