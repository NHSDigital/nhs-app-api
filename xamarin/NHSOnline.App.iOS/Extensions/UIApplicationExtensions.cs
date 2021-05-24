using SafariServices;
using UIKit;

namespace NHSOnline.App.iOS.Extensions
{
    internal static class UIApplicationExtensions
    {
        public static bool IsBrowserOverlayActive(this UIApplication application)
        {
            var viewController = application.KeyWindow.RootViewController;
            while (viewController?.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            return viewController is SFSafariViewController;
        }
    }
}