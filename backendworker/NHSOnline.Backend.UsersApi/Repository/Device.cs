using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Repository
{
    public class Device
    {
        public string DeviceId { get; set; }

        [JsonConverter(typeof(StringEnumConverter), false)]
        public DeviceType? DeviceType { get; set; }
    }
}