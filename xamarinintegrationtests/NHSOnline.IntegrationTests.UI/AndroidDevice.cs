using System;

namespace NHSOnline.IntegrationTests.UI
{
    public enum AndroidDevice
    {
        Pixel3,
        Pixel4,
        Pixel6,
        GalaxyS9,
        GalaxyS20Ultra,
        GalaxyS21
    }

    public static class AndroidDeviceExtensions
    {
        public static string ToName(this AndroidDevice device)
        {
            return device switch
            {
                AndroidDevice.Pixel3 => "Google Pixel 3",
                AndroidDevice.Pixel4 => "Google Pixel 4",
                AndroidDevice.Pixel6 => "Google Pixel 6",
                AndroidDevice.GalaxyS9 => "Samsung Galaxy S9",
                AndroidDevice.GalaxyS20Ultra => "Samsung Galaxy S20 Ultra",
                AndroidDevice.GalaxyS21 => "Samsung Galaxy S21",
                _ => throw new ArgumentOutOfRangeException(nameof(device), device, null)
            };
        }
    }
}