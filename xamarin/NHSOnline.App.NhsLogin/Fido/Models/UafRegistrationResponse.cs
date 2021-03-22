using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.App.NhsLogin.Fido.Models
{
    internal sealed class UafRegistrationResponse
    {
        [JsonProperty("header")]
        public UafOperationHeader? Header { get; set; }

        [JsonProperty("fcParams")]
        public string? FcParams { get; set; }

        [JsonProperty("assertions")]
        public List<UafAuthenticatorRegistrationAssertion> Assertions { get; } = new List<UafAuthenticatorRegistrationAssertion>();
    }
}