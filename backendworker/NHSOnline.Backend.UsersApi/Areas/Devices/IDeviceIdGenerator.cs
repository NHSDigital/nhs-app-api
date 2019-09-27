using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    internal interface IDeviceIdGenerator
    {
        string Generate(AccessToken accessToken, RegisterDeviceRequest request);
        string Generate(AccessToken accessToken, string devicePns);
    }
}