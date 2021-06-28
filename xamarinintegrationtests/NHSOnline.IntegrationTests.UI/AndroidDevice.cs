using System;

namespace NHSOnline.IntegrationTests.UI
{
    public enum AndroidDevice
    {
        Pixel3,
        Pixel4,
    }

    public static class AndroidDeviceExtensions
    {
        public static string ToName(this AndroidDevice device)
        {
            return device switch
            {
                AndroidDevice.Pixel3 => "Google Pixel 3",
                AndroidDevice.Pixel4 => "Google Pixel 4",
                _ => throw new ArgumentOutOfRangeException(nameof(device), device, null)
            };
        }
    }
}