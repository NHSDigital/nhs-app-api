using System;
using AndroidX.Preference;

namespace NHSOnline.App.Droid.DependencyServices.Biometrics
{
    internal static class BiometricRegistrationState
    {
        private const string FidoRegisteredPreferenceKey = "fidoRegistered";

        internal static void Set(bool registered)
        {
            _ = PreferenceManager
                .GetDefaultSharedPreferences(AndroidBiometrics.MainActivity)
                ?.Edit()
                ?.PutBoolean(FidoRegisteredPreferenceKey, registered)
                ?.Commit() ?? throw new InvalidOperationException("Unable to edit preferences");
        }

        internal static bool Get()
        {
            return PreferenceManager
                .GetDefaultSharedPreferences(AndroidBiometrics.MainActivity)
                .GetBoolean(FidoRegisteredPreferenceKey, false);
        }
    }
}