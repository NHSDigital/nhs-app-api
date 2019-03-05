using NHSOnline.Backend.PfsApi.Areas.Configuration;
using NHSOnline.Backend.PfsApi.Devices;

namespace NHSOnline.Backend.PfsApi.Devices
{
    public interface ISupportedDeviceService
    {
        GetConfigurationResult IsDeviceSupported(DeviceDetails device);
    }
}