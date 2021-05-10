using System;
using AndroidX.Preference;

namespace NHSOnline.App.Droid.DependencyServices.Biometrics
{
    internal static class BiometricRegistrationState
    {
        private const string FidoRegisteredPreferenceKey = "fidoRegistered";

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