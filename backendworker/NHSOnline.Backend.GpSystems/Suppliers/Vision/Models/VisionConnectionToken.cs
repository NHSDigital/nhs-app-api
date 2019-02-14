using Newtonsoft.Json;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Models
{
    public class VisionConnectionToken
    {
        public string RosuAccountId { get; set; }
        public string ApiKey { get; set; }

        [JsonProperty(Im1CacheService.Im1ConnectionTokenCacheKeyPropertyName)]
        public string Im1CacheKey { get; set; }
    }
}
