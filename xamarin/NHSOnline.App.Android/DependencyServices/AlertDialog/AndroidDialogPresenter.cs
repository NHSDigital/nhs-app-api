using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        private static ConcurrentDictionary<string, DismissibleDialog> KnownDialogs { get; } = new ConcurrentDictionary<string, DismissibleDialog>();

        public async Task DisplayAlertDialog(NhsAppAlertDialog nhsAppAlert)
        {
            await CreateAndShowAlertDialog(nhsAppAlert).ResumeOnThreadPool();
        }

        public async Task DismissAll()
        {
            await DismissAllActiveDialogs().ResumeOnThreadPool();
        }

        internal static Task CreateAndShowAlertDialog(NhsAppAlertDialog nhsAppAlertDialog)
        {
            DismissAllActiveDialogs();

            if (KnownDialogs.ContainsKey(nhsAppAlertDialog.Message))
            {
                var existingDialog = KnownDialogs.GetValueOrDefault(nhsAppAlertDialog.Message);
                existingDialog.Dialog.Show();
                return Task.CompletedTask;
            }

            var dialog = CreateDialog(nhsAppAlertDialog);

            if (dialog == null)
            {
                Logger.LogWarning($"Failed to create {nhsAppAlertDialog.Message} dialog");
                return Task.CompletedTask;
            }

            var newDismissibleDialog = new DismissibleDialog(nhsAppAlertDialog.DismissAction, dialog);

            if (!KnownDialogs.TryAdd(nhsAppAlertDialog.Message, newDismissibleDialog))
            {
                Logger.LogWarning($"Failed to add {nhsAppAlertDialog.Message} dialog to {nameof(KnownDialogs)} collection");
                return Task.CompletedTask;
            }

            dialog.Show();
            return Task.CompletedTask;
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
                (_, __) => { NhsAppResilience.ExecuteOnMainThread(nhsAppAlertDialog.CancelAction); });

            alert.SetPositiveButton(nhsAppAlertDialog.AcceptText,
                (_, __) => { NhsAppResilience.ExecuteOnMainThread(nhsAppAlertDialog.AcceptAction); });

            return alert.Create();
        }

        private static Task DismissAllActiveDialogs()
        {
            foreach (var (_, dialog) in KnownDialogs)
            {
                if (dialog.Dialog is {IsShowing: true})
                {
                    NhsAppResilience.ExecuteOnMainThread(dialog.DismissAction);
                    dialog.Dialog.Dismiss();
                }
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