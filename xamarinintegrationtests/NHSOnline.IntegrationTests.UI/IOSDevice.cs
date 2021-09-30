using System;

namespace NHSOnline.IntegrationTests.UI
{
    public enum IOSDevice
    {
        iPhone11Pro,
        iPhone8,
        iPhoneX,
        iPhoneXR,
        iPadAir4
    }

    public static class IOSDeviceExtensions
    {
        public static string ToName(this IOSDevice device)
        {
            return device switch
            {
                IOSDevice.iPhone11Pro => "iPhone 11 Pro",
                IOSDevice.iPhone8 => "iPhone 8",
                IOSDevice.iPhoneX => "iPhone X",
                IOSDevice.iPhoneXR => "iPhone XR",
                IOSDevice.iPadAir4 => "iPad Air 4",
                _ => throw new ArgumentOutOfRangeException(nameof(device), device, null)
            };
        }
    }
}