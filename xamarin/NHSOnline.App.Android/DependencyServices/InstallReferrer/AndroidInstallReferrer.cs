using NHSOnline.App.DependencyServices;
using NHSOnline.App.Droid.DependencyServices.InstallReferrer;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidInstallReferrer))]
namespace NHSOnline.App.Droid.DependencyServices.InstallReferrer
{
    public class AndroidInstallReferrer : IInstallReferrer
    {
        public string Referrer => AndroidAppReferrerState.AppReferrer;
    }
}