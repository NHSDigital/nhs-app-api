using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidPlatformVersion))]
namespace NHSOnline.App.Droid.DependencyServices
{
    public sealed class AndroidPlatformVersion: IPlatformVersion
    {
        public bool MeetsMinimumPlatformVersion()
        {
            return true;
        }

        public string MinimumPlatformVersionDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}