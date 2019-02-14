using NHSOnline.Backend.Worker.Areas.Configuration;
using NHSOnline.Backend.Worker.Devices;

namespace NHSOnline.Backend.Worker.Devices
{
    public interface ISupportedDeviceService
    {
        GetConfigurationResult IsDeviceSupported(DeviceDetails device);
    }
}