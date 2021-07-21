using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.App.iOS.DependencyServices.AlertDialog;
using NHSOnline.App.Logging;
using UIKit;

namespace NHSOnline.App.iOS.Controllers
{
    public class AlertDialogBox: IosViewController
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AlertDialogBox));

        public static Dictionary<string?, Action?>? AlertCancelActions { get; } = new Dictionary<string?, Action?>();

        public static void ShowAlertPopup(
            ClearingActions clearingActions,
            string? title,
            string? message,
            string? acceptText,
            string? cancelText,
            Action? acceptAction = null,
            Action? cancelAction = null)
        {
            Logger.LogInformation("Creating and showing alert popup");

            InvokeUIKitOnMainUIThread(() =>
            {
                using var alertController = UIAlertController.Create(
                    title,
                    message,
                    UIAlertControllerStyle.Alert);

                if (cancelText != null)
                {
                    using var alertCancelAction = UIAlertAction.Create(
                        cancelText,
                        UIAlertActionStyle.Default,
                        alert => { cancelAction?.Invoke(); });

                    alertController.AddAction(alertCancelAction);

                    if (clearingActions == ClearingActions.Cancelable && !AlertCancelActions!.ContainsKey(message))
                    {
                        AlertCancelActions.Add(message, cancelAction);
                    }
                }

                if (acceptText != null)
                {
                    using var alertAcceptAction = UIAlertAction.Create(
                        acceptText,
                        UIAlertActionStyle.Default,
                        alert => { acceptAction?.Invoke(); });

                    alertController.AddAction(alertAcceptAction);
                }

                DisplayController(alertController);
            });
        }
    }
}