using System;
using System.Collections.Generic;
using Android.App;

namespace NHSOnline.App.Droid.Dialogs
{
    public static class AlertDialogBox
    {
        internal static MainActivity? MainActivity { get; set; }

        public static Dictionary<string, AlertDialog?> KnownDialogs { get; } = new Dictionary<string, AlertDialog?>();

        public static void CreateAndShowAlertDialog(
            string? title,
            string message,
            string? positiveButtonText = null,
            string? negativeButtonText = null,
            Action? positiveAction = null,
            Action? negativeAction = null)
        {

            if (KnownDialogs.ContainsKey(message))
            {
                var existingDialog = KnownDialogs.GetValueOrDefault(message);
                existingDialog?.Show();
                return;
            }

            using var alert = new AlertDialog.Builder(MainActivity);

            if (title != null)
            {
                alert.SetTitle(title);
            }

            alert.SetMessage(message);
            alert.SetCancelable(false);

            if (negativeButtonText != null)
            {
                alert.SetNegativeButton(negativeButtonText, (sender, args) =>
                {
                    negativeAction?.Invoke();
                });
            }

            if (positiveButtonText != null)
            {
                alert.SetPositiveButton(positiveButtonText, (sender, args) =>
                {
                    positiveAction?.Invoke();
                });
            }

            var dialog = alert.Create();

            KnownDialogs.Add(message, dialog);

            dialog?.Show();
        }
    }
}