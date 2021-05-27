using System;
using Android.App;

namespace NHSOnline.App.Droid.Dialogs
{
    public static class AndroidAlertDialog
    {
        internal static MainActivity? MainActivity { get; set; }

        public static void ShowAlertDialog(
            string title,
            string message,
            Action? positiveAction = null,
            string? positiveButtonText = null,
            string? negativeButtonText = null,
            Action? negativeAction = null)
        {
            using var alert = new AlertDialog.Builder(MainActivity);

            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetCancelable(false);

            if (negativeButtonText != null)
            {
                alert.SetNegativeButton(negativeButtonText, (dialog, args) =>
                {
                    negativeAction?.Invoke();
                    // ReSharper disable AccessToDisposedClosure
                    alert.Dispose();
                    // ReSharper restore AccessToDisposedClosure
                });
            }

            if (positiveButtonText != null)
            {
                alert.SetPositiveButton(positiveButtonText, (dialog, args) =>
                {
                    positiveAction?.Invoke();
                    // ReSharper disable AccessToDisposedClosure
                    alert.Dispose();
                    // ReSharper restore AccessToDisposedClosure
                });
            }

            var dialog = alert.Create();
            dialog?.Show();
        }
    }
}