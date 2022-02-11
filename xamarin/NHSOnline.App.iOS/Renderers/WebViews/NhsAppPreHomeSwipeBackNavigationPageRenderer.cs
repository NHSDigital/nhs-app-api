using NHSOnline.App.Areas.PreHome.Views;
using NHSOnline.App.iOS.Renderers.WebViews;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NhsAppPreHomeScreenWebPage), typeof(NhsAppPreHomeSwipeBackNavigationPageRenderer))]
namespace NHSOnline.App.iOS.Renderers.WebViews
{
    internal sealed class NhsAppPreHomeSwipeBackNavigationPageRenderer: PageRenderer
    {
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ViewController.NavigationController.InteractivePopGestureRecognizer.Enabled = false;
        }
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ViewController.NavigationController.InteractivePopGestureRecognizer.Enabled = true;
        }
    }
}