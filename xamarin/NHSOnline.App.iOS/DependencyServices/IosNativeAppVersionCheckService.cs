using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Configuration;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.DependencyServices;
using NHSOnline.App.Logging;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosNativeAppVersionCheckService))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public class IosNativeAppVersionCheckService : INativeAppVersionCheckService
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(IosNativeAppVersionCheckService));

        public bool MeetsMinimumVersion(VersionConfiguration versionConfiguration)
        {
            var currentVersion = AppInfo.Version;
            var minimumSupportedVersion = versionConfiguration.MinimumSupportedAndroidVersion;

            Logger.LogInformation( $"Update Required Check. Current Version: {currentVersion}. Minimum Version: {minimumSupportedVersion}.");

            return currentVersion >= minimumSupportedVersion;
        }
    }
}
