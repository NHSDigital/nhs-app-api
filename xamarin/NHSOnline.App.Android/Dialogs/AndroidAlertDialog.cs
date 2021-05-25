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
            Action? negativeAction,
            Action? positiveAction,
            string positiveButtonText,
            string negativeButtonText)
        {
            using var alert = new AlertDialog.Builder(MainActivity);

            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetCancelable(false);

            alert.SetNegativeButton(negativeButtonText, (_, _) =>
            {
                negativeAction?.Invoke();
                // ReSharper disable AccessToDisposedClosure
                alert.Dispose();
                // ReSharper restore AccessToDisposedClosure
            });

            alert.SetPositiveButton(positiveButtonText, (_, _) =>
            {
                positiveAction?.Invoke();
                // ReSharper disable AccessToDisposedClosure
                alert.Dispose();
                // ReSharper restore AccessToDisposedClosure
            });

            var dialog = alert.Create();
            dialog?.Show();
        }
    }
}