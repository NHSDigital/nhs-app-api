using System;

namespace NHSOnline.IntegrationTests.UI
{
    public enum IOSDevice
    {
        iPhone11Pro,
        iPhone12,
        iPhone8,
        iPhoneX,
        iPadAir4
    }

    public static class IOSDeviceExtensions
    {
        public static string ToName(this IOSDevice device)
        {
            return device switch
            {
                IOSDevice.iPhone11Pro => "iPhone 11 Pro",
                IOSDevice.iPhone12 => "iPhone 12",
                IOSDevice.iPhone8 => "iPhone 8",
                IOSDevice.iPhoneX => "iPhone X",
                IOSDevice.iPadAir4 => "iPad Air 4",
                _ => throw new ArgumentOutOfRangeException(nameof(device), device, null)
            };
        }
    }
}