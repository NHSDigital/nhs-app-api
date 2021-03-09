using System;
using Microsoft.Extensions.Logging;
using NHSOnline.App.iOS.DependencyServices;
using UIKit;

namespace NHSOnline.App.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            try
            {
                // if you want to use a different Application Delegate class from "AppDelegate"
                // you can specify it here.
                UIApplication.Main(args, null, "AppDelegate");
            }
            catch (Exception e)
            {
                new IosNativeLog().Log(LogLevel.Critical, "Application.Main", $"Failed to start: {e}");
                throw;
            }
        }
    }
}
