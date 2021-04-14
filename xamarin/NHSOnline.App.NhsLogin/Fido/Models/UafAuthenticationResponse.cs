using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.App.NhsLogin.Fido.Models
{
    internal sealed class UafAuthenticationResponse
    {
        [JsonProperty("header")]
        public UafOperationHeader Header { get; set; } = new UafOperationHeader();

        [JsonProperty("fcParams")]
        public string FcParams { get; set; } = string.Empty;

        [JsonProperty("assertions")]
        public List<UafAuthenticatorAuthenticationAssertion> Assertions { get; set; } = new List<UafAuthenticatorAuthenticationAssertion>();
    }
}