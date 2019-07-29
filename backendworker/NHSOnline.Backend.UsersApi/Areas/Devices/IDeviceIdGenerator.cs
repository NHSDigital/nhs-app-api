using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public interface IDeviceIdGenerator
    {
        string Generate(string accessToken, RegisterDeviceRequest request);
    }
}