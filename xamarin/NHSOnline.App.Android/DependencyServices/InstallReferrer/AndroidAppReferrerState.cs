using AndroidX.Preference;

namespace NHSOnline.App.Droid.DependencyServices.InstallReferrer
{
    internal static class AndroidAppReferrerState
    {
        private const string AppReferrerKey = "appReferrer";

        public static string AppReferrer
        {
            get => PreferenceManager
                .GetDefaultSharedPreferences(AndroidInstallReferrerService.MainActivity)
                .GetString(AppReferrerKey, string.Empty) ?? string.Empty;
            set => _ = PreferenceManager
                .GetDefaultSharedPreferences(AndroidInstallReferrerService.MainActivity)
                .Edit()?
                .PutString(AppReferrerKey, value)?
                .Commit();
        }
    }
}