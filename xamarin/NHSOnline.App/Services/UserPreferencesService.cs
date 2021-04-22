using System.Runtime.CompilerServices;
using Xamarin.Essentials;

namespace NHSOnline.App.Services
{
    internal sealed class UserPreferencesService: IUserPreferencesService
    {
        private const string LegacyBiometricsKeyIdIos = "keyID";
        private const string LegacyBiometricsKeyIdAndroid = "KeyId";

        private const string BiometricsKeyIdKey = "FidoKeyID";

        public bool ShowGettingStarted
        {
            get => GetPreference().ValueOr(true);
            set => GetPreference().Set(value);
        }

        public string? BiometricsKeyId
        {
            get
            {
                var value = Preferences.Get(BiometricsKeyIdKey, string.Empty);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }

                var legacyValue = GetLegacyBiometricsKeyId();

                if (string.IsNullOrWhiteSpace(legacyValue))
                {
                    return null;
                }

                CleanupLegacyBiometricsKeyId(legacyValue);

                return legacyValue;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Preferences.Remove(BiometricsKeyIdKey);
                }
                else
                {
                    Preferences.Set(BiometricsKeyIdKey, value);
                }
            }
        }

        private static string? GetLegacyBiometricsKeyId()
        {
            var legacyValue =
                Preferences.Get(LegacyBiometricsKeyIdAndroid, null) ??
                Preferences.Get(LegacyBiometricsKeyIdIos, string.Empty);

            if (string.IsNullOrWhiteSpace(legacyValue))
            {
                return null;
            }

            return legacyValue;
        }

        private static void CleanupLegacyBiometricsKeyId(string legacyValue)
        {
            Preferences.Set(BiometricsKeyIdKey, legacyValue);
            Preferences.Remove(LegacyBiometricsKeyIdAndroid);
            Preferences.Remove(LegacyBiometricsKeyIdIos);
        }

        private static UserPreference GetPreference([CallerMemberName] string key = "") => new UserPreference(key);

        private sealed class UserPreference
        {
            private readonly string _key;

            internal UserPreference(string key) => _key = key;

            internal bool ValueOr(bool defaultValue) => Preferences.Get(_key, defaultValue);

            internal void Set(bool value) => Preferences.Set(_key, value);
        }
    }
}
