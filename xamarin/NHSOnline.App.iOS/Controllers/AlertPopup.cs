using System;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;
using UIKit;

namespace NHSOnline.App.iOS.Controllers
{
    public class AlertPopup: IosViewController
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AlertPopup));

        public static void ShowAlertPopup(
            string alertHeader,
            string alertBody,
            string alertFirstButtonText,
            string alertSecondButtonText,
            Action secondButtonAction)
        {
            Logger.LogInformation("Creating and showing alert popup");

            InvokeUIKitOnMainUIThread(() =>
            {
                using var alertController = UIAlertController.Create(
                    alertHeader,
                    alertBody,
                    UIAlertControllerStyle.Alert);

                using var alertFirstAction = UIAlertAction.Create(
                    alertFirstButtonText,
                    UIAlertActionStyle.Default,
                    null);

                alertController.AddAction(alertFirstAction);

                using var alertSecondAction = UIAlertAction.Create(
                    alertSecondButtonText,
                    UIAlertActionStyle.Default,
                    alert =>
                    {
                        secondButtonAction.Invoke();
                    });

                alertController.AddAction(alertSecondAction);

                DisplayController(alertController);
            });
        }
    }
}