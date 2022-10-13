using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.Users.Areas.Devices.Models
{
    public class Device
    {
        public string DeviceId { get; set; }

        [JsonConverter(typeof(StringEnumConverter), false)]
        public DeviceType? DeviceType { get; set; }
        public string InstallationId { get; set; }
    }
}