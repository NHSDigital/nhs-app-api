using System;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;
using UIKit;
using Xamarin.Forms;

namespace NHSOnline.App.iOS.Controllers
{
    public abstract class IosViewController
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(IosViewController));

        internal static void DisplayController(UIViewController eventController)
        {
            var rootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            if (rootViewController == null)
            {
                Logger.LogError("Unable to obtain root view controller to present event controller");
            }
            else
            {
                rootViewController.PresentViewController(eventController, true, null);
            }
        }

        internal static void InvokeUIKitOnMainUIThread(Action action)
        {
            Device.BeginInvokeOnMainThread(action);
        }
    }
}