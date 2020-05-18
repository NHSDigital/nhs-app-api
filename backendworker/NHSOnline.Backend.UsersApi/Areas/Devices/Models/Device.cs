using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.UsersApi.Areas.Devices.Models
{
    public class Device
    {
        public string DeviceId { get; set; }

        [JsonConverter(typeof(StringEnumConverter), false)]
        public DeviceType? DeviceType { get; set; }
    }
}