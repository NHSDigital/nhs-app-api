using NHSOnline.App.Areas.LoggedOut.Views;
using NHSOnline.App.Areas.WebIntegration.Views;
using NHSOnline.App.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer (typeof(BeforeYouStartPage), typeof(SwipeBackPageRenderer))]
[assembly:ExportRenderer (typeof(NhsLoginUpliftPage), typeof(SwipeBackPageRenderer))]
namespace NHSOnline.App.iOS.Renderers
{
    internal class SwipeBackPageRenderer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ViewController.NavigationController.InteractivePopGestureRecognizer.Enabled = true;
            ViewController.NavigationController.InteractivePopGestureRecognizer.Delegate = new UIGestureRecognizerDelegate();
        }
    }
}