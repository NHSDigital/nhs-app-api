using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;
using UIKit;

namespace NHSOnline.App.iOS.Controllers
{
    public class AlertPopup: IosViewController
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AlertPopup));

        public static void ShowAlertPopup(string alertHeader, string alertBody, string alertButtonText)
        {
            Logger.LogInformation("Creating alert popup");

            InvokeUIKitOnMainUIThread(() =>
            {
                using var alertController = UIAlertController.Create(
                    alertHeader,
                    alertBody,
                    UIAlertControllerStyle.Alert);

                using var alertAction = UIAlertAction.Create(
                    alertButtonText,
                    UIAlertActionStyle.Default,
                    null);

                alertController.AddAction(alertAction);

                DisplayController(alertController);
            });
        }
    }
}