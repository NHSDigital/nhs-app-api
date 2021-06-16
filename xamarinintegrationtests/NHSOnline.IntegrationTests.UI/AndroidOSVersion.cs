using System;
using System.ComponentModel;

namespace NHSOnline.IntegrationTests.UI
{
    public enum AndroidOSVersion
    {
        Nine,
        Ten
    }

    public static class AndroidOsVersionExtensions
    {
        public static string ToName(this AndroidOSVersion osVersion)
        {
            return osVersion switch
            {
                AndroidOSVersion.Nine => "9.0",
                AndroidOSVersion.Ten => "10.0",
                _ => throw new ArgumentOutOfRangeException(nameof(osVersion), osVersion, null)
            };
        }
    }
}