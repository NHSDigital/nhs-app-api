using System;
using System.Threading.Tasks;
using Android.App;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Dialogs;
using NHSOnline.App.Droid.DependencyServices.AlertDialog;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidDialogPresenter))]
namespace NHSOnline.App.Droid.DependencyServices.AlertDialog
{
    public class AndroidDialogPresenter: IDialogPresenter
    {
        internal static MainActivity? MainActivity { get; set; }
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidDialogPresenter));

        private static DismissibleDialog? ActiveDialog { get; set; }

        public async Task DisplayAlertDialog(NhsAppAlertDialog nhsAppAlert)
        {
            await CreateAndShowAlertDialog(nhsAppAlert).PreserveThreadContext();
        }

        public async Task DismissAll()
        {
            await DismissActiveDialog().ResumeOnThreadPool();
        }

        internal static async Task CreateAndShowAlertDialog(NhsAppAlertDialog nhsAppAlertDialog)
        {
            await DismissActiveDialog().PreserveThreadContext();

            var dialog = CreateDialog(nhsAppAlertDialog);

            if (dialog == null)
            {
                Logger.LogWarning($"Failed to create {nhsAppAlertDialog.Message} dialog");
                return;
            }

            var newDismissibleDialog = new DismissibleDialog(nhsAppAlertDialog.DismissAction, dialog);

            dialog.Show();

            ActiveDialog = newDismissibleDialog;
        }

        private static Android.App.AlertDialog? CreateDialog(NhsAppAlertDialog nhsAppAlertDialog)
        {
            using var alert = new Android.App.AlertDialog.Builder(MainActivity);

            if (nhsAppAlertDialog.HasTitleText)
            {
                alert.SetTitle(nhsAppAlertDialog.Title);
            }

            alert.SetMessage(nhsAppAlertDialog.Message);
            alert.SetCancelable(false);

            alert.SetNegativeButton(nhsAppAlertDialog.CancelText,
                (_, __) =>
                {
                    NhsAppResilience.ExecuteOnMainThread(nhsAppAlertDialog.CancelAction);
                    ActiveDialog = null;
                });

            alert.SetPositiveButton(nhsAppAlertDialog.AcceptText,
                (_, __) =>
                {
                    NhsAppResilience.ExecuteOnMainThread(nhsAppAlertDialog.AcceptAction);
                    ActiveDialog = null;
                });

            return alert.Create();
        }

        private static Task DismissActiveDialog()
        {
            if (ActiveDialog != null)
            {
                NhsAppResilience.ExecuteOnMainThread(ActiveDialog.DismissAction);
                ActiveDialog.Dialog.Dismiss();
                ActiveDialog = null;
            }

            return Task.CompletedTask;
        }

        private class DismissibleDialog
        {
            public DismissibleDialog(Func<Task> dismissAction, Dialog dialog)
            {
                DismissAction = dismissAction;
                Dialog = dialog;
            }

            public Func<Task> DismissAction { get; }
            public Dialog Dialog { get; }
        }
    }
}