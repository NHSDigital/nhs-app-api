using Newtonsoft.Json;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    public class TppConnectionToken
    {
        public string AccountId { get; set; }
        public string Passphrase { get; set; }
        public string ProviderId { get; set; }

        [JsonProperty(Im1CacheService.Im1ConnectionTokenCacheKeyPropertyName)]
        public string Im1CacheKey { get; set; }
    }
}