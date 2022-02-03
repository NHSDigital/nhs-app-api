using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosPlatformVersion))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public sealed class IosPlatformVersion: IPlatformVersion
    {
        public bool MeetsMinimumPlatformVersion()
        {
            return Compatibility.MinimumRequiredVersion(11, 0);
        }
    }
}