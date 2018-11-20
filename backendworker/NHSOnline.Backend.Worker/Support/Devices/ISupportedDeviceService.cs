using NHSOnline.Backend.Worker.Areas.Configuration;

namespace NHSOnline.Backend.Worker.Support.Devices
{
    public interface ISupportedDeviceService
    {
        GetConfigurationResult IsDeviceSupported(DeviceDetails device);
    }
}