using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Dialogs;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using UIKit;

namespace NHSOnline.App.iOS.Controllers
{
    public class AlertDialogBoxController: IosViewController
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AlertDialogBoxController));

        private static ConcurrentDictionary<string, Func<Task>> AlertDismissActions { get; } = new ConcurrentDictionary<string, Func<Task>>();

        public static async Task ShowAlertPopup(
            NhsAppAlertDialog nhsAppAlert)
        {
            Logger.LogInformation("Creating and showing alert popup");

            await InvokeUIKitOnMainUIThread(async () =>
            {
                if (AlertIsShowing(nhsAppAlert))
                {
                    return;
                }

                using var alertController = UIAlertController.Create(
                    nhsAppAlert.Title,
                    nhsAppAlert.Message,
                    UIAlertControllerStyle.Alert);

                using var alertCancelAction = CreateAlertAction(nhsAppAlert.CancelText, nhsAppAlert.CancelAction);
                alertController.AddAction(alertCancelAction);

                using var alertAcceptAction = CreateAlertAction(nhsAppAlert.AcceptText, nhsAppAlert.AcceptAction);
                alertController.AddAction(alertAcceptAction);


                if (!AlertDismissActions.ContainsKey(nhsAppAlert.Message))
                {
                    if (!AlertDismissActions.TryAdd(nhsAppAlert.Message, nhsAppAlert.DismissAction))
                    {
                        Logger.LogWarning("Unable to add dismiss action for {Message} dialog", nhsAppAlert.Message);
                    }
                }

                await ShowDialog(alertController).PreserveThreadContext();
            }).PreserveThreadContext();
        }

        public static async Task DismissAll()
        {
            await InvokeUIKitOnMainUIThread(async () =>
            {
                var modal = GetAlertController();

                if (modal == null)
                {
                    return;
                }

                await DismissModal(modal).PreserveThreadContext();
            }).PreserveThreadContext();
        }

        private static UIAlertController? GetAlertController() =>
            (UIAlertController?)UIApplication.SharedApplication.KeyWindow.RootViewController?.ModalViewController;

        private static UIAlertAction CreateAlertAction(string text, Func<Task> action) =>
         UIAlertAction.Create(text, UIAlertActionStyle.Default, alert =>
         {
                 NhsAppResilience.ExecuteOnMainThread(action);
         });

        private static bool AlertIsShowing(NhsAppAlertDialog nhsAppAlertDialog)
        {
            var modal = GetAlertController();

            if (modal == null)
            {
                return false;
            }

            return modal.Message == nhsAppAlertDialog.Message;
        }

        private static async Task DismissModal(UIAlertController modal)
        {
            if (modal.Message != null &&
                AlertDismissActions.TryGetValue(modal.Message, out var dismissAction))
            {
                if (dismissAction != null)
                {
                    await dismissAction().PreserveThreadContext();
                }

                if (!AlertDismissActions.TryRemove(modal.Message, out _))
                {
                    Logger.LogWarning("Unable to remove dismiss action for {Message} dialog", modal.Message);
                }
            }

            await modal.DismissViewControllerAsync(true).PreserveThreadContext();
        }

        private static async Task ShowDialog(UIAlertController alertController)
        {
            var modal = GetAlertController();

            if (modal != null)
            {
                await DismissModal(modal).PreserveThreadContext();
            }

            await DisplayController(alertController).PreserveThreadContext();
        }
    }
}