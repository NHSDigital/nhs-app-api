using System.Runtime.InteropServices;

namespace NHSOnline.Backend.Worker.UnitTests
{
    public static class TimeZoneResolver
    {
        public static string GetTimeZoneNameForCurrentOS()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "GMT Standard Time" : "Europe/London";
        }
    }
}
