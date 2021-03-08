using NHSOnline.App.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(SwipeBackNavigationPageRenderer))]
namespace NHSOnline.App.iOS.Renderers
{
    internal sealed class SwipeBackNavigationPageRenderer : NavigationRenderer
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            InteractivePopGestureRecognizer.Delegate = new GestureDelegate(this);
        }

        private sealed class GestureDelegate : UIGestureRecognizerDelegate
        {
            readonly UINavigationController _parent;

            public GestureDelegate(UINavigationController parent) => _parent = parent;

            public override bool ShouldBegin(UIGestureRecognizer recognizer) => _parent.ViewControllers?.Length > 1;
        }
    }
}