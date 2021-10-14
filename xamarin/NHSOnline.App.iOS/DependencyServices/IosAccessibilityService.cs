using Foundation;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.iOS.DependencyServices;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(IosAccessibilityService))]
namespace NHSOnline.App.iOS.DependencyServices
{
    public sealed class IosAccessibilityService: IAccessibilityService
    {
        public void AnnounceText(string text)
        {
            using var nsText = new NSString(text);
            UIAccessibility.PostNotification(UIAccessibilityPostNotification.LayoutChanged, nsText);
        }
    }
}