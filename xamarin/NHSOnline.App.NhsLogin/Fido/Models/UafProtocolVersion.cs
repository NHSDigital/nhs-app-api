using Newtonsoft.Json;

namespace NHSOnline.App.NhsLogin.Fido.Models
{
    internal sealed class UafProtocolVersion
    {
        [JsonProperty("major")]
        public ushort Major { get; set; }

        [JsonProperty("minor")]
        public ushort Minor { get; set; }

        public UafProtocolVersion DeepClone()
            => new UafProtocolVersion
            {
                Major = Major,
                Minor = Minor
            };
    }
}