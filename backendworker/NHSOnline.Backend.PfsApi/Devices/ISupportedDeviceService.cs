using NHSOnline.Backend.PfsApi.Areas.Configuration;

namespace NHSOnline.Backend.PfsApi.Devices
{
    public interface ISupportedDeviceService
    {
        GetConfigurationResult IsDeviceSupported(DeviceDetails device);
    }
}