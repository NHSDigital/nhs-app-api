using Android.OS;
using Android.Views;

namespace NHSOnline.App.Droid.Extensions
{
    public static class WindowExtensions
    {
        /*
         * The Secure flag cannot be modified after the Window has been created, so it is set as a default on Android N
         * and below. As this prevents taking screenshots, it is added/cleared when the app is paused/resumed respectively
         * on Android O and above.
         *
         * https://stackoverflow.com/questions/52186726/allow-screenshots-using-flag-secure
         */
        private static bool OnOreoOrAbove => Build.VERSION.SdkInt >= BuildVersionCodes.O;

        public static void SetDefaultFlags(this Window? window)
        {
#if !SECURE_FLAG_DISABLED
            if (!OnOreoOrAbove)
            {
                window?.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);
            }
#endif
        }

        public static void AddSecureFlag(this Window? window)
        {
            if (OnOreoOrAbove)
            {
                window?.AddFlags(WindowManagerFlags.Secure);
            }
        }

        public static void ClearSecureFlag(this Window? window)
        {

            if (OnOreoOrAbove)
            {
                window?.ClearFlags(WindowManagerFlags.Secure);
            }
        }
    }
}