using NHSOnline.App.Api.Configuration;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidNativeMinimumVersionCheck))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public class AndroidNativeMinimumVersionCheck : INativeMinimumVersionCheck
    {
        public bool MeetsMinimumVersion(VersionConfiguration versionConfiguration)
        {
            return AppInfo.Version >= versionConfiguration.MinimumSupportedAndroidVersion;
        }
    }
}