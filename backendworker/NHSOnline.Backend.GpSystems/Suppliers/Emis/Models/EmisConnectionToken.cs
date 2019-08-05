using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class EmisConnectionToken
    {
        [JsonProperty(Im1CacheService.Im1ConnectionTokenCacheKeyPropertyName)]
        public string Im1CacheKey { get; set; }
        public string AccessIdentityGuid { get; set; }
    }
}
