using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.App.NhsLogin.Fido.Models
{
    internal sealed class UafOperationHeader
    {
        [JsonProperty("upv")]
        public UafProtocolVersion? Upv { get; set; }

        [JsonProperty("op")]
        [JsonConverter(typeof(StringEnumConverter))]
        public UafOperation Op { get; set; }

        [JsonProperty("AppID")]
        public string AppId { get; set; } = string.Empty;

        [JsonProperty("serverData")]
        public string ServerData { get; set; } = string.Empty;

        internal UafOperationHeader DeepClone()
            => new UafOperationHeader
            {
                Upv = Upv?.DeepClone(),
                Op = Op,
                AppId = AppId,
                ServerData = ServerData
            };
    }
}