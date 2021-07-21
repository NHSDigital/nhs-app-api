using System;
using NHSOnline.App.DependencyServices;
using Xamarin.Forms;
using AndroidAlertDialog = NHSOnline.App.Droid.DependencyServices.AlertDialog.AndroidAlertDialog;

[assembly: Dependency(typeof(AndroidAlertDialog))]
namespace NHSOnline.App.Droid.DependencyServices.AlertDialog
{
    public class AndroidAlertDialog : IAlertDialog
    {

        public void DisplayAlertDialog(
            string message,
            string acceptText,
            string cancelText,
            Action acceptAction,
            Action cancelAction)
        {
            // This is to ensure that session expiry takes priority
            CancelAll();

            Dialogs.AlertDialogBox.CreateAndShowAlertDialog(
                null,
                message,
                acceptText,
                cancelText,
                acceptAction,
                cancelAction);
        }

        public void DisplayAlertDialog(
            string title,
            string message,
            string acceptText,
            string cancelText,
            Action acceptAction,
            Action cancelAction)
        {
            Dialogs.AlertDialogBox.CreateAndShowAlertDialog(
                title,
                message,
                acceptText,
                cancelText,
                acceptAction,
                cancelAction);
        }

        public void DismissAll()
        {
            foreach (var (_, dialog) in Dialogs.AlertDialogBox.KnownDialogs)
            {
                    if (dialog is {IsShowing: true})
                    {
                        dialog.Dismiss();
                    }
            }
        }

        private static void CancelAll()
        {
            foreach (var (_, dialog) in Dialogs.AlertDialogBox.KnownDialogs)
            {
                if (dialog is {IsShowing: true})
                {
                    dialog.Cancel();
                }
            }
        }
    }
}