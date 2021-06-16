using System;

namespace NHSOnline.IntegrationTests.UI
{
    public enum IOSDevice
    {
        iPhone11Pro,
    }

    public static class IOSDeviceExtensions
    {
        public static string ToName(this IOSDevice device)
        {
            return device switch
            {
                IOSDevice.iPhone11Pro => "iPhone 11 Pro",
                _ => throw new ArgumentOutOfRangeException(nameof(device), device, null)
            };
        }
    }
}