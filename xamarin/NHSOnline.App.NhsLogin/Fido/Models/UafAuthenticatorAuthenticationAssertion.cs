using Newtonsoft.Json;

namespace NHSOnline.App.NhsLogin.Fido.Models
{
    internal sealed class UafAuthenticatorAuthenticationAssertion
    {
        [JsonProperty("assertionScheme")]
        public string AssertionScheme { get; set; } = "UAFV1TLV";

        [JsonProperty("assertion")]
        public string Assertion { get; set; } = string.Empty;
    }
}