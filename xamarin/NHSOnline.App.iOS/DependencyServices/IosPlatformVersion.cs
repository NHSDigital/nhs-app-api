using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.DependencyServices;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosPlatformVersion))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public sealed class IosPlatformVersion: IPlatformVersion
    {
        public bool MeetsMinimumPlatformVersion()
        {
            return UIDevice.CurrentDevice.CheckSystemVersion(11, 0);
        }

        public string MinimumPlatformVersionDescription()
        {
            return "iOS 11";
        }
    }
}