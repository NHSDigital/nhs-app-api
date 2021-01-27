using System.Runtime.CompilerServices;
using Xamarin.Essentials;

namespace NHSOnline.App.Services
{
    internal sealed class UserPreferencesService: IUserPreferencesService
    {
        public bool ShowBeforeYouStart
        {
            get => GetPreference().ValueOr(true);
            set => GetPreference().Set(value);
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
