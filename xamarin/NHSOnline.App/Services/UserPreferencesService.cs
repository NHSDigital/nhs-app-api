using System.Runtime.CompilerServices;
using Xamarin.Essentials;

namespace NHSOnline.App.Services
{
    internal sealed class UserPreferencesService: IUserPreferencesService
    {
        private const string BiometricsKeyIdKey = "keyID";

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
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }
                return value;
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
