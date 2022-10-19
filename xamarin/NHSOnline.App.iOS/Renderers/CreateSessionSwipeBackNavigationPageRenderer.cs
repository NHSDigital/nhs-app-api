using NHSOnline.App.Areas.LoggedOut.Views;
using NHSOnline.App.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CreateSessionPage), typeof(CreateSessionSwipeBackNavigationPageRenderer))]
namespace NHSOnline.App.iOS.Renderers
{
    internal sealed class CreateSessionSwipeBackNavigationPageRenderer : PageRenderer
    {
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (ViewController.NavigationController != null)
            {
                ViewController.NavigationController.InteractivePopGestureRecognizer.Enabled = false;
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (ViewController.NavigationController != null)
            {
                ViewController.NavigationController.InteractivePopGestureRecognizer.Enabled = true;
            }
        }
    }
}