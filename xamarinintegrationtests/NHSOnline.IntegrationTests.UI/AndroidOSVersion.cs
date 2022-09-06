using System;

namespace NHSOnline.IntegrationTests.UI
{
    public enum AndroidOSVersion
    {
        Eight,
        Nine,
        Ten,
        Eleven,
        Twelve,
        Thirteen
    }

    public static class AndroidOsVersionExtensions
    {
        public static string ToName(this AndroidOSVersion osVersion)
        {
            return osVersion switch
            {
                AndroidOSVersion.Eight => "8.0",
                AndroidOSVersion.Nine => "9.0",
                AndroidOSVersion.Ten => "10.0",
                AndroidOSVersion.Eleven => "11.0",
                AndroidOSVersion.Twelve => "12.0",
                AndroidOSVersion.Thirteen => "13.0",

                _ => throw new ArgumentOutOfRangeException(nameof(osVersion), osVersion, null)
            };
        }
    }
}