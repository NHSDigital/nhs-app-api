using System;
using Android.Content;
using Android.Content.PM;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Configuration;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using NHSOnline.App.Logging;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidNativeAppVersionCheckService))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public class AndroidNativeAppVersionCheckService : INativeAppVersionCheckService
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(AndroidNativeAppVersionCheckService));

        public bool MeetsMinimumVersion(VersionConfiguration versionConfiguration)
        {
            Version currentVersion = new (GetPackageVersion() ??
                                 throw new ArgumentNullException("Current Version String is null", nameof(currentVersion)));

            var minimumSupportedVersion = versionConfiguration.MinimumSupportedAndroidVersion;

            Logger.LogInformation( $"Update Required Check. Current Version: {currentVersion}. Minimum Version: {minimumSupportedVersion}.");

            return currentVersion >= minimumSupportedVersion;
        }

        /// <summary>
        /// Using this as opposed to AppInfo.Version due to Xamarin Essentials not being thread safe
        /// https://github.com/xamarin/Essentials/issues/1959
        /// </summary>
        /// <returns></returns>
        private static string? GetPackageVersion()
        {
            Context context = Android.App.Application.Context;
            PackageManager? manager = context.PackageManager;
            if (context.PackageName != null && manager != null)
            {
                var info = manager.GetPackageInfo(context.PackageName, 0);
                return info?.VersionName;
            }
            return null;
        }
    }
}