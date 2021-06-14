using NHSOnline.App.Api.Configuration;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.DependencyServices;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosNativeMinimumVersionCheck))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public class IosNativeMinimumVersionCheck : INativeMinimumVersionCheck
    {
        public bool MeetsMinimumVersion(VersionConfiguration versionConfiguration)
        {
            return AppInfo.Version >= versionConfiguration.MinimumSupportediOSVersion;
        }
    }
}
