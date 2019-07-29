using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class DeviceIdGenerator : IDeviceIdGenerator
    {
        public string Generate(string accessToken, RegisterDeviceRequest request)
        {
            return $"{accessToken}-{request?.DevicePns}";
        }
    }
}