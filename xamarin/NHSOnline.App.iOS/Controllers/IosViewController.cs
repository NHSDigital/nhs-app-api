using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using UIKit;
using Xamarin.Forms;

namespace NHSOnline.App.iOS.Controllers
{
    public abstract class IosViewController
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(IosViewController));

        internal static async Task DisplayController(UIViewController eventController)
        {
            var rootViewController = UIApplication.SharedApplication.KeyWindow?.RootViewController;

            if (rootViewController == null)
            {
                Logger.LogError("Unable to obtain root view controller to present event controller");
            }
            else
            {
                await rootViewController.PresentViewControllerAsync(eventController, true).ResumeOnThreadPool();
            }
        }

        internal static async Task InvokeUIKitOnMainUIThread(Func<Task> action)
        {
            await Device.InvokeOnMainThreadAsync(action).ResumeOnThreadPool();
        }
    }
}