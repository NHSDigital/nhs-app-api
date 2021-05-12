using System.Threading.Tasks;
using Foundation;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.DependencyServices;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosSettingsService))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public class IosSettingsService : ISettingsService
    {
        public Task OpenSettings()
        {
            using var settingsUrl = new NSUrl(UIApplication.OpenSettingsUrlString);
            return UIApplication.SharedApplication.OpenUrlAsync(settingsUrl, new UIApplicationOpenUrlOptions());
        }
    }
}