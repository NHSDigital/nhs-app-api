using System;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Droid.DependencyServices;
using NHSOnline.App.Logging;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidNativeLog))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public sealed class AndroidNativeLog: INativeLog
    {
        public void Log(LogLevel logLevel, string context, string message)
        {
            try
            {
                var logMethod = GetLogMethod(logLevel);
                logMethod(context, message);
            }
            // Do not allow exceptions from logging to propagate
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Failed to log: {0}", e);
            }
        }

        private static Func<string, string, int> GetLogMethod(LogLevel logLevel)
            => logLevel switch
            {
                LogLevel.Trace => Android.Util.Log.Debug,
                LogLevel.Debug => Android.Util.Log.Debug,
                LogLevel.Information => Android.Util.Log.Info,
                LogLevel.Warning => Android.Util.Log.Warn,
                LogLevel.Error => Android.Util.Log.Error,
                LogLevel.Critical => Android.Util.Log.Wtf,
                LogLevel.None => (context, message) => 0,
                _ => Android.Util.Log.Wtf
            };
    }
}