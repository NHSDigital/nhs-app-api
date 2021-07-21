using System;
using System.Collections.Generic;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.Controllers;
using NHSOnline.App.iOS.DependencyServices.AlertDialog;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosAlertDialog))]
namespace NHSOnline.App.iOS.DependencyServices.AlertDialog
{
    public class IosAlertDialog : IAlertDialog
    {
        public void DisplayAlertDialog(
            string? message,
            string? acceptText,
            string? cancelText,
            Action? acceptAction,
            Action? cancelAction)
        {
            // This is to ensure that session expiry takes priority
            DismissAll();

            AlertDialogBox.ShowAlertPopup(
                ClearingActions.Cancelable,
                null,
                message,
                acceptText,
                cancelText,
                acceptAction,
                cancelAction
            );
        }

        public void DisplayAlertDialog(
            string title,
            string message,
            string acceptText,
            string cancelText,
            Action acceptAction,
            Action cancelAction)
        {
            AlertDialogBox.ShowAlertPopup(
                ClearingActions.Cancelable,
                title,
                message,
                acceptText,
                cancelText,
                acceptAction,
                cancelAction
            );
        }

        public void DismissAll()
        {
            if (UIApplication.SharedApplication.KeyWindow.RootViewController?.ModalViewController == null)
            {
                return;
            }

            var modal = (UIAlertController) UIApplication.SharedApplication.KeyWindow.RootViewController
                ?.ModalViewController!;

            if (AlertDialogBox.AlertCancelActions != null &&
                AlertDialogBox.AlertCancelActions.ContainsKey(modal.Message))
            {
                var cancelAction = Controllers.AlertDialogBox.AlertCancelActions.GetValueOrDefault(modal.Message);

                cancelAction?.Invoke();
                AlertDialogBox.AlertCancelActions.Remove(modal.Message);
            }

            modal.DismissViewControllerAsync(true);
        }
    }
}