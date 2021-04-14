using Newtonsoft.Json;

namespace NHSOnline.App.NhsLogin.Fido.Models
{
    internal sealed class UafDeregisterAuthenticator
    {
        [JsonProperty("aaid")]
        public string Aaid { get; set; } = "EBA0#0001";

        [JsonProperty("keyID")]
        public string KeyId { get; set; } = string.Empty;
    }
}