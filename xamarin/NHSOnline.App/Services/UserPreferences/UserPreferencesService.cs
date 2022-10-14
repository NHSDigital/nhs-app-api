using System.Runtime.CompilerServices;
using NHSOnline.App.Services.UserPreferences.Models;
using Xamarin.Essentials;

namespace NHSOnline.App.Services.UserPreferences
{
    internal sealed class UserPreferencesService: IUserPreferencesService
    {
        private const string LegacyBiometricsKeyIdIos = "keyID";
        private const string LegacyBiometricsKeyIdAndroid = "KeyId";

        private const string BiometricsKeyIdKey = "FidoKeyID";
        private const string FidoUsernameKey = "fidoUsername";

        private const string NotificationsLastPromptedDateTimePrefix = "NotificationsLastPromptedDateTime-";
        private const string NotificationsInstallationIdPrefix = "NotificationsInstallationId-";

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

        public string FidoUsername
        {
            get => Preferences.Get(FidoUsernameKey, string.Empty);
            set => Preferences.Set(FidoUsernameKey, value);
        }

        public NotificationsRegistration GetNotificationsRegistration(string nhsLoginId)
        {
            return new NotificationsRegistration
            {
                LastPromptedDateTime = Preferences.Get($"{NotificationsLastPromptedDateTimePrefix}{nhsLoginId}", string.Empty),
                InstallationId = Preferences.Get($"{NotificationsInstallationIdPrefix}{nhsLoginId}", string.Empty),
            };
        }

        public void SetNotificationsRegistration(string nhsLoginId, NotificationsRegistration registration)
        {
            Preferences.Set($"{NotificationsLastPromptedDateTimePrefix}{nhsLoginId}", registration.LastPromptedDateTime);
            Preferences.Set($"{NotificationsInstallationIdPrefix}{nhsLoginId}", registration.InstallationId);
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
