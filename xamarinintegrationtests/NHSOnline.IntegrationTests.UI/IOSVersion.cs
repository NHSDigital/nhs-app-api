using System;

namespace NHSOnline.IntegrationTests.UI
{
    public enum IOSVersion
    {
        Eleven,
        Twelve,
        Thirteen,
        Fourteen,
        Fifteen
    }

    public static class IOSVersionExtensions
    {
        public static string ToName(this IOSVersion osVersion)
        {
            return osVersion switch
            {
                IOSVersion.Eleven => "11",
                IOSVersion.Twelve => "12",
                IOSVersion.Thirteen => "13",
                IOSVersion.Fourteen => "14",
                IOSVersion.Fifteen => "15",
                _ => throw new ArgumentOutOfRangeException(nameof(osVersion), osVersion, null)
            };
        }
    }
}