using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Im1Connection
{
    public class MicrotestConnectionToken
    {
        [JsonProperty(Im1CacheService.Im1ConnectionTokenCacheKeyPropertyName)]
        public string Im1CacheKey { get; set; }
        public string NhsNumber {get; set; }
    }
}
