using System;

namespace NHSOnline.IntegrationTests.UI
{
    public enum IOSVersion
    {
        Thirteen,
    }

    public static class IOSVersionExtensions
    {
        public static string ToName(this IOSVersion osVersion)
        {
            return osVersion switch
            {
                IOSVersion.Thirteen => "13.0",
                _ => throw new ArgumentOutOfRangeException(nameof(osVersion), osVersion, null)
            };
        }
    }
}