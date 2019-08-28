using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Areas.Devices
{
    public interface IDeviceIdGenerator
    {
        string Generate(AccessToken accessToken, RegisterDeviceRequest request);
    }
}