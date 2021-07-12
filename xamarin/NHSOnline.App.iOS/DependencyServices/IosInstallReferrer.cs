using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.DependencyServices;

[assembly: Xamarin.Forms.Dependency(typeof(IosInstallReferrer))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public class IosInstallReferrer : IInstallReferrer
    {
        public string Referrer => string.Empty;
    }
}