using System;
using AndroidX.Preference;

namespace NHSOnline.App.Droid.DependencyServices.Biometrics
{
    internal static class BiometricRegistrationState
    {
        private const string FidoUsernamePreferenceKey = "fidoUsername";
        private const string FidoRegisteredPreferenceKey = "fidoRegistered";

        public static string FidoUsername
        {
            get => PreferenceManager
                .GetDefaultSharedPreferences(AndroidBiometrics.MainActivity)
                ?.GetString(FidoUsernamePreferenceKey, string.Empty) ?? string.Empty;
            set => _ = PreferenceManager
                .GetDefaultSharedPreferences(AndroidBiometrics.MainActivity)
                ?.Edit()
                ?.PutString(FidoUsernamePreferenceKey, value)
                ?.Commit() ?? throw new InvalidOperationException("Unable to edit preferences");
        }

        public static bool FidoRegistered
        {
            get => PreferenceManager
                .GetDefaultSharedPreferences(AndroidBiometrics.MainActivity)
                .GetBoolean(FidoRegisteredPreferenceKey, false);
            set => _ = PreferenceManager
                .GetDefaultSharedPreferences(AndroidBiometrics.MainActivity)
                ?.Edit()
                ?.PutBoolean(FidoRegisteredPreferenceKey, value)
                ?.Commit() ?? throw new InvalidOperationException("Unable to edit preferences");
        }
    }
}