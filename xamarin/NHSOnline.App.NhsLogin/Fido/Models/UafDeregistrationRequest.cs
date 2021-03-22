using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.App.NhsLogin.Fido.Models
{
    internal sealed class UafDeregistrationRequest
    {
        [JsonProperty("header")]
        public UafOperationHeader Header { get; set; } = new UafOperationHeader();

        [JsonProperty("authenticators")]
        public List<UafDeregisterAuthenticator> Authenticators { get; set; } = new List<UafDeregisterAuthenticator>();
    }
}