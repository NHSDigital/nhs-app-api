using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public class DeviceIdGenerator : IDeviceIdGenerator
    {
        public string Generate(AccessToken accessToken, RegisterDeviceRequest request)
        {
            return Generate(accessToken, request?.DevicePns);
        }
        
        public string Generate(AccessToken accessToken, string devicePns)
        {
            return $"{accessToken.Subject}-{devicePns}";
        }
    }
}