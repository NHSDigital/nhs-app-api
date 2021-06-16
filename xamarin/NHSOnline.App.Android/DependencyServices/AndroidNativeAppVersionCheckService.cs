using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Configuration;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using NHSOnline.App.Logging;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidNativeAppVersionCheckService))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public class AndroidNativeAppVersionCheckService : INativeAppVersionCheckService
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidNativeAppVersionCheckService));

        public bool MeetsMinimumVersion(VersionConfiguration versionConfiguration)
        {
            var currentVersion = AppInfo.Version;
            var minimumSupportedVersion = versionConfiguration.MinimumSupportedAndroidVersion;

            Logger.LogInformation( $"Update Required Check. Current Version: {currentVersion}. Minimum Version: {minimumSupportedVersion}.");

            return currentVersion >= minimumSupportedVersion;
        }
    }
}