using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.iOS.DependencyServices;
using NHSOnline.App.Logging;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosNativeLog))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public class IosNativeLog: INativeLog
    {
        public void Log(LogLevel logLevel, string context, string message)
        {
            try
            {
                var logLine = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} <{1}> {2}",
                    logLevel,
                    context,
                    message);

                var logLineEscaped = logLine.Replace("%", "%%", StringComparison.Ordinal);

                using var logLineString = new NSString(logLineEscaped);

                NativeMethods.NSLog(logLineString.Handle);
            }
            // Do not allow exceptions from logging to propagate
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Failed to log: {0}", e);
            }
        }

        private static class NativeMethods
        {
            [DllImport(ObjCRuntime.Constants.FoundationLibrary)]
            internal static extern void NSLog(IntPtr format);
        }
    }
}