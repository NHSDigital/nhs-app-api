using Android.Views;

namespace NHSOnline.App.Droid.Renderers
{
    internal static class KeyEventArgsExtensions
    {
        internal static bool IsEnterKeyReleaseEvent(this View.KeyEventArgs e) => e.IsKeyEvent(Keycode.Enter, KeyEventActions.Up);

        private static bool IsKeyEvent(this View.KeyEventArgs eventArgs, Keycode keycode, KeyEventActions action)
        {
            return
                eventArgs.Event != null &&
                eventArgs.Event.Action == action &&
                eventArgs.KeyCode == keycode;
        }
    }
}