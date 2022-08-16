using System.Threading.Tasks;
using Foundation;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.DependencyServices;
using NHSOnline.App.Threading;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosUpdateService))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public class IosUpdateService : IUpdateService
    {
        public async Task OpenAppStoreUrl()
        {
            var storeUrl = NSUrl.FromString("https://apps.apple.com/gb/app/nhs-app/id1388411277");
            await UIApplication.SharedApplication.OpenUrlAsync(storeUrl!, new UIApplicationOpenUrlOptions()).ResumeOnThreadPool();
        }
    }
}