using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NHSOnline.Backend.Worker.UnitTests
{
    public static class TimeZoneResolver
    {
        public static string GetTimeZoneNameForCurrentOS()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "Europe/London" : "GMT Standard Time";
        }
    }
}
