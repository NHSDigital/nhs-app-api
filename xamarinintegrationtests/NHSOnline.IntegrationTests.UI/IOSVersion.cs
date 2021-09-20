using System;

namespace NHSOnline.IntegrationTests.UI
{
    public enum IOSVersion
    {
        Eleven,
        Twelve,
        Thirteen,
    }

    public static class IOSVersionExtensions
    {
        public static string ToName(this IOSVersion osVersion)
        {
            return osVersion switch
            {
                IOSVersion.Thirteen => "13.0",
                IOSVersion.Eleven => "11",
                IOSVersion.Twelve => "12",
                _ => throw new ArgumentOutOfRangeException(nameof(osVersion), osVersion, null)
            };
        }
    }
}