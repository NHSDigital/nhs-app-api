using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Users.Areas.Devices.Models;

namespace NHSOnline.Backend.Users.Repository
{
    public interface IDeviceIdGenerator
    {
        string Generate(AccessToken accessToken, RegisterDeviceRequest request);
        string Generate(AccessToken accessToken, string devicePns);
    }
}