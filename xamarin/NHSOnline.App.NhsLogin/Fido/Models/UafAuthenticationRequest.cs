using Newtonsoft.Json;

namespace NHSOnline.App.NhsLogin.Fido.Models
{
    internal sealed class UafAuthenticationRequest
    {
        [JsonProperty("header")]
        public UafOperationHeader Header { get; set; } = new UafOperationHeader();

        [JsonProperty("challenge")]
        public string Challenge { get; set; } = string.Empty;
    }
}